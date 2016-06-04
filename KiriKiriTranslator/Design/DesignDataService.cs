using System;
using KiriKiriTranslator.Model;
using System.Collections.Generic;
using System.Linq;

namespace KiriKiriTranslator.Design
{
    public class DesignDataService : IKKFile
    {
        public string CurrentFilePath { get { return "FakePathYo"; } }

        public bool LoadFromKK(string filePath)
        {
            return true;
        }

        public bool Load(string filePath)
        {
            return true;
        }

        public bool SaveToKK(string filePath)
        {

            return true;
        }

        public bool Save(string filePath)
        {

            return true;
        }

        public bool ExportToXLS(string filePath)
        {
            return true;
        }

        public bool CreateLabelAlias(string labelToAlias, string labelDestination, int aliasLength)
        {
            return true;
        }

        public bool DestroyLabelAlias(string labelToAlias, int aliasLength)
        {

            return true;
        }


        public List<KKLabelGroup> KKLabelGroups
        {
            get
            {
                var res = new List<KKLabelGroup>();
                res.Add(new KKLabelGroup { Name = "label1", PrintedText = "moonrunes", TranslatedText = "englishtext" });
                res.Add(new KKLabelGroup { Name = "label2", PrintedText = "moonrunes", TranslatedText = "englishtext" });
                res.Add(new KKLabelGroup { Name = "label3", PrintedText = "moonrunes", TranslatedText = "englishtext" });
                return res;
            }
        }

        public List<KKLabelGroup> KKLabelGroupsToTranslate
        {
            get
            {
                return KKLabelGroups.Where(lg => String.IsNullOrEmpty(lg.PrintedText)).ToList();
            }
        }

        public List<KKNameTag> KKNameTags
        {
            get
            {
                var res = new List<KKNameTag>();
                res.Add(new KKNameTag { Original = "japanese", Translated = "english" });
                res.Add(new KKNameTag { Original = "japanese", Translated = "english" });
                res.Add(new KKNameTag { Original = "japanese", Translated = "english" });
                return res;
            }
        }

        public List<KKChoice> KKChoices
        {
            get
            {
                var res = new List<KKChoice>();
                res.Add(new KKChoice { OriginalText = "japanese", TranslatedText = "english" });
                res.Add(new KKChoice { OriginalText = "japanese", TranslatedText = "english" });
                res.Add(new KKChoice { OriginalText = "japanese", TranslatedText = "english" });
                return res;
            }
        }

        public List<KKChapterName> KKChapterNames
        {
            get
            {
                var res = new List<KKChapterName>();
                res.Add(new KKChapterName { Original = "japanese", Translated = "english" });
                res.Add(new KKChapterName { Original = "japanese", Translated = "english" });
                res.Add(new KKChapterName { Original = "japanese", Translated = "english" });
                return res;
            }
        }


    }
}