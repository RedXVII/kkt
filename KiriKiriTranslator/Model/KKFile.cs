using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace KiriKiriTranslator.Model
{
    public class KKFile : IKKFile
    {
        private string _currentFilePath;
        public string CurrentFilePath
        {
            get { return _currentFilePath; }
            set { _currentFilePath = value; }
        }

        private List<KKLine> KKLines { get; set; }
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

        public bool LoadFromKK(string filePath)
        {
            KKLines = new List<KKLine>();
            _KKLabelGroupsToTranslateCache = null;
            KKLabelGroups = new List<KKLabelGroup>();

                

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
                        KKLabelGroups.Add(currentGroup);
                    }
                    currentGroup = new KKLabelGroup(newLine.Label);
                }

                if (currentGroup != null)
                {
                    currentGroup.Append(newLine);
                }
                
                KKLines.Add(newLine);
                lineNumber++;
            }
            if (currentGroup != null)
            {
                currentGroup.Compute();
                KKLabelGroups.Add(currentGroup);
            }

            return true;

        }
        public bool Load(string filePath)
        {
            string serializedJson = File.ReadAllText(filePath);

            KKLabelGroups = JsonConvert.DeserializeObject<List<KKLabelGroup>>(serializedJson);
            _KKLabelGroupsToTranslateCache = null;

            return true;
        }

        public bool SaveToKK(string filePath)
        {
            using (var sw = new StreamWriter(filePath, false, System.Text.Encoding.GetEncoding(932)))
            {
                foreach (var labelGroup in KKLabelGroups)
                {
                    labelGroup.WriteToKK(sw);
                }
            }

            return true;
        }

        public bool Save(string filePath)
        {
            string serializedJson = JsonConvert.SerializeObject(KKLabelGroups, Formatting.Indented);

            File.WriteAllText(filePath, serializedJson);

            return true;
        }
    }
}