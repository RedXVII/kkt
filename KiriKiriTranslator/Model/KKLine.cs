using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiriKiriTranslator.Model
{
    public class KKLine
    {
        public string Label { get; set; }

        public string PrintedText { get; set; }

        public string Instruction { get; set; }

        public int LineNumber { get; set; }

        public string RawText { get; set; }

        public KKLine()
        {

        }

        public KKLine(int lineNumber, string rawText)
        {
            LineNumber = lineNumber;
            RawText = rawText;

            if (rawText.StartsWith("*"))
            {
                parseLabel(rawText);
            }
            else
            {
                parseCommand(rawText);
            }
        }


        private void parseLabel(string rawText)
        {
            Label = rawText;
        }

        private void parseCommand(string rawText)
        {
            int instructionFlag = 0;
            bool inhibitor = false;
            var printedText = new StringBuilder();
            var instruction = new StringBuilder();

            foreach (var character in rawText.ToCharArray())
            {
                bool isText = false;
                if (character == ';')
                {
                    inhibitor = true;
                    break;
                }
                else if (inhibitor)
                {
                    inhibitor = false;
                }
                else if (character == '[')
                {
                    instructionFlag++;
                }
                if (character == ']')
                {
                    instructionFlag--;
                }
                else if (instructionFlag == 0)
                {
                    if (character == '\\')
                    {
                        inhibitor = true;
                    }
                    else
                    {
                        isText = true;
                    }
                }
                if (isText)
                {
                    printedText.Append(character);
                }
                else
                {
                    instruction.Append(character);
                }
            }
            if (!inhibitor)
            {
                printedText.AppendLine();
            }
            PrintedText = printedText.ToString();
            Instruction = instruction.ToString();
        }
    }
}
