using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using KiriKiriTranslator.Model;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace KiriKiriTranslator.ViewModel
{
    public class LineListViewModel : ViewModelBase
    {
        private readonly IKKFile _dataService;

        public CollectionViewSource ViewList { get; set; }

        private ObservableCollection<KKLabelGroup> labelGroupList = new ObservableCollection<KKLabelGroup>();
        public ReadOnlyObservableCollection<KKLabelGroup> LabelGroupList
        {
            get { return new ReadOnlyObservableCollection<KKLabelGroup>(labelGroupList); }
        }

        private int _pageSize = 20;
        public int MaxPage { get; private set; }

        private int currentPage;



        private int _requestedPage;
        public int RequestedPage
        {
            get
            {
                return _requestedPage;
            }
            set
            {
                Set(ref _requestedPage, value);
            }
        }

        public RelayCommand PreviousPageCommand { get; private set; }
        public RelayCommand NextPageCommand { get; private set; }
        public RelayCommand JumpToPageCommand { get; private set; }
        public RelayCommand<string> CopyToClipboardCommand { get; private set; }


        public LineListViewModel(IKKFile dataService)
        {

            _dataService = dataService;
            _dataService.Load(@"D:\data.kkt");

            ViewList = new CollectionViewSource();
            ViewList.Source = labelGroupList;

            MaxPage = _dataService.KKLabelGroupsToTranslate.Count / _pageSize + 1;

            
            NextPageCommand = new RelayCommand(NextPage, CanNextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage, CanPreviousPage);
            JumpToPageCommand = new RelayCommand(JumpToPage, CanJumpToPage);
            CopyToClipboardCommand = new RelayCommand<string>(CopyToClipboard);

            SetPage(1);
        }

        private void CopyToClipboard(string text)
        {
            System.Windows.Clipboard.SetText(text);
        }

        private bool CanNextPage()
        {
            return currentPage < MaxPage;
        }

        private void NextPage()
        {
            SetPage(currentPage + 1);
        }

        private bool CanPreviousPage()
        {
            return currentPage > 1;
        }

        private void PreviousPage()
        {
            SetPage(currentPage - 1);
        }

        private bool CanJumpToPage()
        {
            return RequestedPage >= 1 && RequestedPage <= MaxPage;
        }

        private void JumpToPage()
        {
            SetPage(RequestedPage);
        }

        private void SetPage(int page)
        {
            var source = _dataService.KKLabelGroupsToTranslate;

            int start = (page - 1) * _pageSize;
            int range = System.Math.Min(_pageSize, source.Count - start);

            labelGroupList.Clear();
            foreach (var labelGroup in source.GetRange(start, range))
            {
                labelGroupList.Add(labelGroup);
            }
            currentPage = page;
            RequestedPage = page;

            PreviousPageCommand.RaiseCanExecuteChanged();
            NextPageCommand.RaiseCanExecuteChanged();
            JumpToPageCommand.RaiseCanExecuteChanged();
        }
    }
}