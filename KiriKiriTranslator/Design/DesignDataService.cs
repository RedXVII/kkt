﻿using System;
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
    }
}