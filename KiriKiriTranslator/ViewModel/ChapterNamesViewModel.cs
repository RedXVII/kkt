using GalaSoft.MvvmLight;
using KiriKiriTranslator.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace KiriKiriTranslator.ViewModel
{

    public class ChapterNamesViewModel : ViewModelBase
    {
        private readonly IKKFile _dataService;

        public CollectionViewSource ViewList { get; set; }

        private ObservableCollection<KKChapterName> chapterNameList = new ObservableCollection<KKChapterName>();
        public ReadOnlyObservableCollection<KKChapterName> ChapterNameList
        {
            get { return new ReadOnlyObservableCollection<KKChapterName>(chapterNameList); }
        }

        public List<KKNameTag> NameTags { get; set; }

        /// <summary>
        /// Initializes a new instance of the NameTagsViewModel class.
        /// </summary>
        public ChapterNamesViewModel(IKKFile dataService)
        {
            _dataService = dataService;

            ViewList = new CollectionViewSource();
            ViewList.Source = chapterNameList;

            RefreshView();
        }

        public void RefreshView()
        {
            chapterNameList.Clear();
            foreach (KKChapterName chapterName in _dataService.KKChapterNames)
            {
                chapterNameList.Add(chapterName);
            }
        }
    }
}