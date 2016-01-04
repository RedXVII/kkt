using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KiriKiriTranslator.Model
{
    public interface IKKFile
    {
        string CurrentFilePath { get; }

        bool LoadFromKK(string filePath);

        bool Load(string filePath);

        bool SaveToKK(string filePath);

        bool Save(string filePath);

        bool ExportToXLS(string filePath);

        List<KKLabelGroup> KKLabelGroups { get; }
        List<KKLabelGroup> KKLabelGroupsToTranslate { get; }
        List<KKNameTag> KKNameTags { get; }
    }
}
