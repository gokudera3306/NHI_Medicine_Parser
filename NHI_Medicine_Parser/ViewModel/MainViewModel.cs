using System.ComponentModel;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NHI_Medicine_Parser.Class;
using NHI_Medicine_Parser.Database;

namespace NHI_Medicine_Parser.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public static SQLServerConnection connection = new SQLServerConnection();

        #region ----- Define Commands -----
        public RelayCommand GetSourceFolderCommand { get; set; }
        public RelayCommand GetDestinationFolderCommand { get; set; }
        public RelayCommand StartTransferCommand { get; set; }
        #endregion

        #region ----- Define Variables -----
        private string sourceFolder = "";
        private string destinationFolder = "";

        public string SourceFolder
        {
            get => sourceFolder;
            set { Set(() => SourceFolder, ref sourceFolder, value); }
        }
        public string DestinationFolder
        {
            get => destinationFolder;
            set { Set(() => DestinationFolder, ref destinationFolder, value); }
        }
        public TransferService Service { get; set; }
        #endregion

        public MainViewModel()
        {
            InitCommands();
            Service = new TransferService();
        }

        #region ----- Define Actions -----
        private void GetSourceFolderAction()
        {
            SourceFolder = GetFolder();
        }
        private void GetDestinationFolderAction()
        {
            DestinationFolder = GetFolder();
        }
        private void StartTransferAction()
        {
            Service.StartTransfer(SourceFolder, DestinationFolder);
        }
        #endregion

        #region ----- Define Functions -----
        private void InitCommands()
        {
            GetSourceFolderCommand = new RelayCommand(GetSourceFolderAction);
            GetDestinationFolderCommand = new RelayCommand(GetDestinationFolderAction);
            StartTransferCommand = new RelayCommand(StartTransferAction);
        }
        private string GetFolder()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    return dialog.SelectedPath;
                }
            }

            return "";
        }
        #endregion
    }
}