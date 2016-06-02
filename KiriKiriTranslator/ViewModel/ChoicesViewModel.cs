using KiriKiriTranslator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KiriKiriTranslator.ViewModel
{
    public class ChoicesViewModel
    {
        private readonly IKKFile _dataService;

        public CollectionViewSource ViewList { get; set; }

        private ObservableCollection<KKChoice> choicesList = new ObservableCollection<KKChoice>();
        public ReadOnlyObservableCollection<KKChoice> ChoiceList
        {
            get { return new ReadOnlyObservableCollection<KKChoice>(choicesList); }
        }

        /// <summary>
        /// Initializes a new instance of the NameTagsViewModel class.
        /// </summary>
        public ChoicesViewModel(IKKFile dataService)
        {
            _dataService = dataService;

            ViewList = new CollectionViewSource();
            ViewList.Source = choicesList;


            foreach (KKChoice choice in _dataService.KKChoices)
            {
                choicesList.Add(choice);
            }


        }
    }
}
