using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using KiriKiriTranslator.Model;

namespace KiriKiriTranslator.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IKKFile _dataService;

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }
            set
            {
                Set(ref _welcomeTitle, value);
            }
        }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand AddKKFileCommand { get; set; }
        public RelayCommand GenerateKKCommand { get; set; }
        public RelayCommand GenerateXLSCommand { get; set; }
        public RelayCommand CreateLabelAliasCommand { get; set; }
        public RelayCommand DestroyLabelAliasCommand { get; set; }

        private System.Threading.Timer _autoSaveTimer;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IKKFile dataService)
        {
            _dataService = dataService;
            WelcomeTitle = "One step at a time.";

            SaveCommand = new RelayCommand(Save);
            AddKKFileCommand = new RelayCommand(AddKKFile);
            GenerateKKCommand = new RelayCommand(GenerateKK);
            GenerateXLSCommand = new RelayCommand(GenerateXLS);
            CreateLabelAliasCommand = new RelayCommand(CreateLabelAlias);
            DestroyLabelAliasCommand = new RelayCommand(DestroyLabelAlias);
            _autoSaveTimer = new System.Threading.Timer(AutoSave, null, 300000, 300000);
        }

        public override void Cleanup()
        {
            _autoSaveTimer.Dispose();
            base.Cleanup();
        }


        private void AutoSave(object state)
        {
            if (_dataService != null)
            {
                _dataService.Save(@"autosave.kkt.bak");
                _dataService.Save(@"autosave.kkt");
            }
        }
        

        private void Save()
        {
            _dataService.Save(@"data.kkt.bak");
            _dataService.Save(@"data.kkt");
        }

        private void AddKKFile()
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.DefaultExt = ".ks";
            fileDialog.Filter = "Kirikiri script (*.ks)|*.ks";

            var result = fileDialog.ShowDialog();
            if (result == true)
            {
                if (_dataService.LoadFromKK(fileDialog.FileName))
                {
                    RefreshAllTabs();
                }
            }
        }

        private void GenerateKK()
        {
            _dataService.SaveToKK("export");
        }

        private void GenerateXLS()
        {
            _dataService.ExportToXLS(@"Yukikaze_script.xlsx");
        }

        private void CreateLabelAlias()
        {
            CreateAliasDialog inputDialog = new CreateAliasDialog();
            if (inputDialog.ShowDialog() == true)
            {
                _dataService.CreateLabelAlias(inputDialog.LabelToAlias, inputDialog.LabelDestination, inputDialog.LabelAliasLength);
            }
        }

        private void DestroyLabelAlias()
        {

        }

        private void RefreshAllTabs()
        {
            SimpleIoc.Default.GetInstance<LineListViewModel>().RefreshView();
            SimpleIoc.Default.GetInstance<ChoicesViewModel>().RefreshView();
            SimpleIoc.Default.GetInstance<NameTagsViewModel>().RefreshView();
            SimpleIoc.Default.GetInstance<ChapterNamesViewModel>().RefreshView();
        }

    }
}