using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiriKiriTranslator.Model
{
    public class KKChoice
    {
        public string Label { get; set; }

        public string OriginalText { get; set; }

        public string TranslatedText { get; set; }

        public List<KKSubChoice> SubChoices { get; set; }

        public void AddSubChoice(string original)
        {
            if (SubChoices == null)
            {
                SubChoices = new List<KKSubChoice>();
            }
            SubChoices.Add(new KKSubChoice { Original = original });
        }
    }

    public class KKSubChoice
    {
        public string Original { get; set; }
        public string Translated { get; set; }
    }
}
