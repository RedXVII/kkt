using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KiriKiriTranslator.Model
{
    public class KKLabelGroup
    {

        private static Regex NameRegex = new Regex(@"(\[NAME_\w n="")(.+)(""\])");

        public string Name { get; set; }
        public string PrintedText { get; set; }
        public string TranslatedText { get; set; }
        public string PreInstruction { get; set; }
        public string PostInstruction { get; set; }

        private string _nameTag;

        [Newtonsoft.Json.JsonIgnore]
        public string NameTag
        {
            get
            {
                if (_nameTag == null)
                {
                    var match = NameRegex.Match(PreInstruction);
                    if (match.Success)
                    {
                        _nameTag = match.Groups[2].Value;
                        
                    }
                    else
                    {
                        _nameTag = "";
                    }
                }

                return _nameTag;
            }
        }

        private List<KKLine> Lines { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public GalaSoft.MvvmLight.Command.RelayCommand CopyToClipboardCommand { get; private set; }

        public KKLabelGroup()
        {
            CopyToClipboardCommand = new GalaSoft.MvvmLight.Command.RelayCommand(() =>
            {
                //System.Windows.Clipboard.Clear();

                System.Windows.Clipboard.SetDataObject (PrintedText);

            });
        }

        public KKLabelGroup(string name)
        {
            this.Name = name;
            this.Lines = new List<KKLine>();
        }

        public void Append(KKLine line)
        {
            Lines.Add(line);
        }

        public void Compute()
        {
            var printedText = new StringBuilder();
            var preInstruction = new StringBuilder();
            var postInstruction = new StringBuilder();

            foreach (var line in Lines)
            {
                if (!String.IsNullOrEmpty(line.PrintedText))
                {
                    printedText.Append(line.PrintedText);
                }
                if (!String.IsNullOrEmpty(line.Instruction))
                {
                    if (printedText.Length == 0)
                    {
                        preInstruction.AppendLine(line.Instruction);
                    }
                    else
                    {
                        postInstruction.AppendLine(line.Instruction);
                    }
                }
            }

            PrintedText = printedText.ToString();
            PreInstruction = preInstruction.ToString();
            PostInstruction = postInstruction.ToString();
        }

        public void WriteToKK(StreamWriter sw, Dictionary<string, string> nameTags, List<KKChoice> choices)
        {
            sw.WriteLine(this.Name);

            string preInstruction = PreInstruction;
            if (!String.IsNullOrEmpty(this.NameTag) && !String.IsNullOrEmpty(nameTags[NameTag]))
            {
                preInstruction = NameRegex.Replace(preInstruction, "$1" + nameTags[NameTag] + "$3");
            }

            var choice = choices.Where(c => c.Label == this.Name).FirstOrDefault();
            if (choice != null)
            {
                
                preInstruction = preInstruction.Replace("\"" + choice.OriginalText + "\"", "\"" + choice.TranslatedText + "\"");
                foreach (var subchoice in choice.SubChoices)
                {
                    preInstruction = preInstruction.Replace("\"" + subchoice.Original + "\"", "\"" + subchoice.Translated + "\"");
                }
            }
            sw.Write(preInstruction);
            

            if (!String.IsNullOrEmpty(this.PrintedText))
            {
                if (!String.IsNullOrEmpty(this.TranslatedText))
                {
                    sw.Write(TranslatedText);
                }
                else
                {
                    sw.Write(PrintedText);
                }
                
            }
            
            sw.Write(PostInstruction);
        }

    }
}
