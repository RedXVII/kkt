using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using System.Text.RegularExpressions;

namespace KiriKiriTranslator.Model
{
    public class KKFile : IKKFile
    {
        private object _lockObject = new object();

        private string _currentFilePath;
        public string CurrentFilePath
        {
            get { return _currentFilePath; }
            set { _currentFilePath = value; }
        }

        public List<KKLabelGroup> KKLabelGroups { get; private set; }

        private List<KKLabelGroup> _KKLabelGroupsToTranslateCache;

        public List<KKLabelGroup> KKLabelGroupsToTranslate
        {
            get
            {
                if (_KKLabelGroupsToTranslateCache == null)
                {
                    _KKLabelGroupsToTranslateCache = KKLabelGroups.Where(lg => !String.IsNullOrEmpty(lg.PrintedText)).ToList();
                }
                return _KKLabelGroupsToTranslateCache;
            }
        }


        public List<KKNameTag> KKNameTags { get; set; }

        public List<KKOutputFile> KKOutputFiles { get; set; }

        public List<KKChoice> KKChoices { get; set; }

        public List<KKChapterName> KKChapterNames { get; set; }

        public KKFile()
        {
            this.Load(@"D:\data.kkt"); //let's never do this again.
        }

        public bool LoadFromKK(string filePath)
        {
            if (KKLabelGroups == null)
            {
                KKLabelGroups = new List<KKLabelGroup>();
            }

            KKOutputFile outputFile = new KKOutputFile() { FileName = System.IO.Path.GetFileName(filePath) };
            outputFile.Labels = new List<KKLabelGroup>();

            var lines = File.ReadLines(filePath, System.Text.Encoding.GetEncoding(932)); //SHIFT-JIS
            int lineNumber = 1;

            KKLabelGroup currentGroup = null;


            foreach (var rawLine in lines)
            {
                string line = rawLine;

                KKLine newLine = new KKLine(lineNumber, line);

                if (!string.IsNullOrEmpty(newLine.Label))
                {
                    if (currentGroup != null)
                    {
                        currentGroup.Compute();
                        outputFile.Labels.Add(currentGroup);
                    }
                    currentGroup = new KKLabelGroup(newLine.Label);
                }

                if (currentGroup != null)
                {
                    currentGroup.Append(newLine);
                }

                lineNumber++;
            }
            if (currentGroup != null)
            {
                currentGroup.Compute();
                outputFile.Labels.Add(currentGroup);
            }

            KKOutputFiles.Add(outputFile);
            KKLabelGroups.AddRange(outputFile.Labels);
            _KKLabelGroupsToTranslateCache = null;
            RefreshChapterNames();

            return true;

        }
        public bool Load(string filePath)
        {
            string serializedJson = File.ReadAllText(filePath);

            var JsonFile = JsonConvert.DeserializeObject<KKJsonFile>(serializedJson);


            KKOutputFiles = JsonFile.OutputFiles;

            KKLabelGroups = KKOutputFiles.SelectMany(file => file.Labels).ToList();
            _KKLabelGroupsToTranslateCache = null;
            this.RefreshAliasText();

            KKNameTags = JsonFile.NameTags;

            KKChoices = JsonFile.Choices;

            KKChapterNames = JsonFile.ChapterNames;

            return true;
        }

  

        public bool SaveToKK(string folderPath)
        {
            var nameTagDict = new Dictionary<string ,string>();
            foreach (var nameTag in KKNameTags)
            {
                nameTagDict.Add(nameTag.Original, nameTag.Translated);
            }

            var chapterNameDict = new Dictionary<string, string>();
            foreach (var chapter in KKChapterNames)
            {
                chapterNameDict.Add(chapter.Original, chapter.Translated);
            }

            foreach (KKOutputFile file in KKOutputFiles)
            {
                using (var sw = new StreamWriter( System.IO.Path.Combine(folderPath, file.FileName), false, System.Text.Encoding.GetEncoding(932)))
                {
                    foreach (var labelGroup in file.Labels)
                    {
                        labelGroup.WriteToKK(sw, nameTagDict, this.KKChoices, chapterNameDict);
                    }
                }
            }

            

            return true;
        }

        public bool Save(string filePath)
        {
            lock (_lockObject)
            {
                KKJsonFile JsonFile = new KKJsonFile { OutputFiles = KKOutputFiles, NameTags = KKNameTags, Choices = KKChoices, ChapterNames = KKChapterNames };
                string serializedJson = JsonConvert.SerializeObject(JsonFile, Formatting.Indented);
                File.WriteAllText(filePath, serializedJson);

                return true;
            }

        }

        public bool ExportToXLS(string filePath)
        {
            var workBook = new XSSFWorkbook();
            var ch = workBook.GetCreationHelper();
            var sheet = workBook.CreateSheet();
            var cs = workBook.CreateCellStyle();
            cs.WrapText = true;
            
            int rowNumber = 0;

            sheet.SetColumnWidth(0, 256 * 40);
            sheet.SetColumnWidth(1, 256 * 10);
            sheet.SetColumnWidth(2, 256 * 90);
            sheet.SetColumnWidth(3, 256 * 90);

            foreach (var labelGroup in KKLabelGroupsToTranslate)
            {
                var row = sheet.CreateRow(rowNumber);

                // Label Name
                var labelCell = row.CreateCell(0);
                labelCell.SetCellValue(labelGroup.Name);

                // Nametag Text
                var nametagCell = row.CreateCell(1);
                nametagCell.SetCellValue(labelGroup.NameTag);

                // English text
                var engCell = row.CreateCell(2);
                engCell.SetCellValue(ch.CreateRichTextString(labelGroup.TranslatedText));
                engCell.CellStyle = cs;

                // Japanese text
                var jpCell = row.CreateCell(3);
                jpCell.SetCellValue(ch.CreateRichTextString(labelGroup.PrintedText));
                jpCell.CellStyle = cs;

                //row.Height = 256 * 4;



                rowNumber++;
            }
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    workBook.Write(stream);
                }
            }
            catch (IOException e)
            {
                return false;
            }
            

            return true;
        }

        public bool CreateLabelAlias(string labelToAlias, string labelDestination, int aliasLength)
        {
            int toAliasIndex = KKLabelGroups.FindIndex(label => label.NameWithoutChapter == labelToAlias);
            int destinationIndex = KKLabelGroups.FindIndex(label => label.NameWithoutChapter == labelDestination);

            if (toAliasIndex == -1 || destinationIndex == -1)
                return false;

            for (int i = 0; i < aliasLength; i++)
            {
                while (string.IsNullOrEmpty(KKLabelGroups[toAliasIndex].PrintedText))
                    toAliasIndex++;
                while (string.IsNullOrEmpty(KKLabelGroups[destinationIndex].PrintedText))
                    destinationIndex++;

                KKLabelGroups[toAliasIndex].Alias = KKLabelGroups[destinationIndex].NameWithoutChapter;

                toAliasIndex++;
                destinationIndex++;
            }

            RefreshAliasText();
            return true;
        }

        public bool DestroyLabelAlias(string labelToAlias, int aliasLength)
        {

            return true;
        }


        private void RefreshAliasText()
        {
            foreach (var label in KKLabelGroups)
            {
                if (!string.IsNullOrEmpty(label.Alias))
                {
                    var originalLabel = KKLabelGroups.Where(l => l.NameWithoutChapter == label.Alias).FirstOrDefault();
                    if (originalLabel != null)
                    {
                        label.AliasedText = originalLabel.TranslatedText;
                    }
                }
            }
        }

        private void RefreshChapterNames()
        {
            if (KKChapterNames == null)
            {
                KKChapterNames = new List<KKChapterName>();
            }
            foreach (KKLabelGroup label in KKLabelGroups)
            {
                if (!string.IsNullOrEmpty(label.Chapter) && !KKChapterNames.Exists(ch => ch.Original == label.Chapter))
                {
                    KKChapterNames.Add(new KKChapterName() { Original = label.Chapter });
                }
            }
        }


        private List<KKNameTag> LoadNamesFromDictionary(Dictionary<string, string> dict)
        {
            var res = new List<KKNameTag>();

            foreach (var pair in dict)
            {
                res.Add(new KKNameTag { Original = pair.Key, Translated = pair.Value });
            }

            return res;
        }

        private Dictionary<string, string> LoadNamesFromLabels(List<KKLabelGroup> labelGroups)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            foreach (var labelGroup in labelGroups)
            {
                if (!String.IsNullOrEmpty(labelGroup.NameTag) && !res.ContainsKey(labelGroup.NameTag))
                {

                    res.Add(labelGroup.NameTag, null);
                }
            }

            return res;
        }

        private static Regex ChoiceRegex = new Regex(@"\[SELECT_CENTER text=""([^""]*)""(?:.*?)sel_1=""([^""]*)""(?:.*?)sel_2=""([^""]*)""(?:.*?)(?:sel_3=""([^""]*)""(?:.*?))?\]");

        private List<KKChoice> LoadChoicesFromLabels(List<KKLabelGroup> labelGroups)
        {
            List<KKChoice> res = new List<KKChoice>();
            foreach (var labelGroup in labelGroups)
            {
                if (!String.IsNullOrEmpty(labelGroup.PreInstruction))
                {
                    Match match = ChoiceRegex.Match(labelGroup.PreInstruction);
                    if (match.Success)
                    {
                        KKChoice newChoice = new KKChoice { Label = labelGroup.Name, OriginalText = match.Groups[1].Value };
                        newChoice.AddSubChoice(match.Groups[2].Value);
                        newChoice.AddSubChoice(match.Groups[3].Value);
                        if (match.Groups[4].Success)
                        {
                            newChoice.AddSubChoice(match.Groups[4].Value);
                        }
                        res.Add(newChoice);
                    }
                }
            }
            return res;
        }

        public class KKJsonFile
        {
            public List<KKOutputFile> OutputFiles { get; set; }
            public List<KKNameTag> NameTags { get; set; }
            public List<KKChoice> Choices { get; set; }
            public List<KKChapterName> ChapterNames { get; set; }
        }
    }
}