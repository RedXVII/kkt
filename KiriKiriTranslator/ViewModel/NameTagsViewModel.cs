using GalaSoft.MvvmLight;
using KiriKiriTranslator.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace KiriKiriTranslator.ViewModel
{

    public class NameTagsViewModel : ViewModelBase
    {
        private readonly IKKFile _dataService;

        public CollectionViewSource ViewList { get; set; }

        private ObservableCollection<KKNameTag> nameTagList = new ObservableCollection<KKNameTag>();
        public ReadOnlyObservableCollection<KKNameTag> NameTagList
        {
            get { return new ReadOnlyObservableCollection<KKNameTag>(nameTagList); }
        }

        public List<KKNameTag> NameTags { get; set; }

        /// <summary>
        /// Initializes a new instance of the NameTagsViewModel class.
        /// </summary>
        public NameTagsViewModel(IKKFile dataService)
        {
            _dataService = dataService;

            ViewList = new CollectionViewSource();
            ViewList.Source = nameTagList;


            RefreshView();

        }

        public void RefreshView()
        {
            nameTagList.Clear();
            foreach (KKNameTag nameTag in _dataService.KKNameTags)
            {
                nameTagList.Add(nameTag);
            }

        }
    }
}