﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
        public RelayCommand GenerateKKCommand { get; set; }
        public RelayCommand GenerateXLSCommand { get; set; }

        private System.Threading.Timer _autoSaveTimer;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IKKFile dataService)
        {
            _dataService = dataService;
            WelcomeTitle = "Do it for her.";

            SaveCommand = new RelayCommand(Save);
            GenerateKKCommand = new RelayCommand(GenerateKK);
            GenerateXLSCommand = new RelayCommand(GenerateXLS);
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
                _dataService.Save(@"D:\autosave.kkt.bak");
                _dataService.Save(@"D:\autosave.kkt");
            }
        }
        

        private void Save()
        {
            _dataService.Save(@"D:\data.kkt.bak");
            _dataService.Save(@"D:\data.kkt");
        }

        private void GenerateKK()
        {
            _dataService.SaveToKK("newgame.ks");
        }

        private void GenerateXLS()
        {
            _dataService.ExportToXLS(@"Yukikaze_script.xlsx");
        }


    }
}