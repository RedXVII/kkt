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

        bool CreateLabelAlias(string labelToAlias, string labelDestination, int aliasLength);
        bool DestroyLabelAlias(string labelToAlias, int aliasLength);

        List<KKLabelGroup> KKLabelGroups { get; }
        List<KKLabelGroup> KKLabelGroupsToTranslate { get; }
        List<KKNameTag> KKNameTags { get; }
        List<KKChoice> KKChoices { get; }
        List<KKChapterName> KKChapterNames { get; }
    }
}
