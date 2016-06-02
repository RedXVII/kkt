using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiriKiriTranslator.Model
{
    public class KKOutputFile
    {
        public string FileName { get; set; }

        public List<KKLabelGroup> Labels { get; set; }
    }
}
