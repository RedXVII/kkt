using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiriKiriTranslator.Model
{
    public class KKLabelGroup
    {
        public string Name { get; set; }
        public string PrintedText { get; set; }
        public string TranslatedText { get; set; }
        public string PreInstruction { get; set; }
        public string PostInstruction { get; set; }

        private List<KKLine> Lines { get; set; }

        public KKLabelGroup()
        {

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

        public void WriteToKK(StreamWriter sw)
        {
            sw.WriteLine(this.Name);
            sw.Write(PreInstruction);

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
