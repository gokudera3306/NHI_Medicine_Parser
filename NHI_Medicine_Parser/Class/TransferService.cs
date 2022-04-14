using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using NHI_Medicine_Parser.Database;
using NHI_Medicine_Parser.ViewModel;

namespace NHI_Medicine_Parser.Class
{
    public class TransferService : ObservableObject
    {
        #region ----- Define Variables -----

        public static BackgroundWorker TransferBackgroundWorker = new BackgroundWorker();

        #region ///// BackgroundWorker Variables /////
        private bool isBusy;
        private int progress;
        private string progressMessage;
        
        public bool IsBusy
        {
            get => isBusy;
            set { Set(() => IsBusy, ref isBusy, value); }
        }
        public int Progress
        {
            get => progress;
            set { Set(() => Progress, ref progress, value); }
        }
        public string ProgressMessage
        {
            get => progressMessage;
            set { Set(() => ProgressMessage, ref progressMessage, value); }
        }
        #endregion

        private bool isSuccess;
        private string sourceFolder;
        private string destinationFolder;
        private List<string> sourceFiles;
        
        public NHIMedicineParser MedicineParser { get; set; } = new NHIMedicineParser();
        #endregion

        public TransferService()
        {
            InitBackgroundWorker();
        }

        #region ----- Define Functions -----
        private void InitServiceStatus()
        {
            sourceFolder = "";
            destinationFolder = "";

            Progress = 0;
            ProgressMessage = "初始化...";
            sourceFiles = new List<string>();
        }

        #region ///// Check Folder Functions /////
        private bool IsFolderValid()
        {
            if (sourceFolder.Equals(String.Empty))
            {
                MessageWindow.ShowMessage("請選擇檔案來源資料夾", MessageType.ERROR);
                return false;
            }

            if (destinationFolder.Equals(String.Empty))
            {
                MessageWindow.ShowMessage("請選擇完成檔案資料夾", MessageType.ERROR);
                return false;
            }

            return true;
        }
        private bool IsSourceFileValid()
        {
            string[] files = Directory.GetFiles(sourceFolder);

            Regex fileRegex = new Regex("all1_[0-9]{7}\\([12]\\)\\.(txt|b5)$");
            Match match;

            foreach (string file in files)
            {
                match = fileRegex.Match(file.Split('\\').Last());
                if (match.Success)
                    sourceFiles.Add(file);
            }

            switch (sourceFiles.Count)
            {
                case 0:
                    MessageWindow.ShowMessage("來源資料夾中無符合條件檔案。", MessageType.ERROR);
                    break;
                case 1:
                    MessageWindow.ShowMessage("來源資料夾中符合條件檔案不足。", MessageType.ERROR);
                    break;
                case 2:
                    return true;
                default:
                    MessageWindow.ShowMessage("來源資料夾符合條件檔案過多。", MessageType.ERROR);
                    break;
            }

            return false;
        }
        #endregion

        #region ///// BackgroundWorker Functions /////
        private void InitBackgroundWorker()
        {
            TransferBackgroundWorker = new BackgroundWorker();
            TransferBackgroundWorker.WorkerReportsProgress = true;
            TransferBackgroundWorker.DoWork += TransferBackgroundWorkerOnDoWork;
            TransferBackgroundWorker.RunWorkerCompleted += TransferBackgroundWorkerOnRunWorkerCompleted;
            TransferBackgroundWorker.ProgressChanged += TransferBackgroundWorkerOnProgressChanged;
        }
        private void TransferBackgroundWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressMessage = e.UserState.ToString();
            Progress += e.ProgressPercentage;
        }
        private void TransferBackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;

            if(isSuccess)
                MessageWindow.ShowMessage("轉換成功", MessageType.SUCCESS);
            else
                MessageWindow.ShowMessage("轉換失敗", MessageType.ERROR);
        }
        private void TransferBackgroundWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            IsBusy = true;

            foreach (string file in sourceFiles)
            {
                bool isValid = MedicineParser.CheckFileFormat(file);
                
                if (!isValid)
                {
                    TransferBackgroundWorker.ReportProgress(0, $"找尋檔案 {file.Split('\\').Last()} 錯誤資料位置...");
                    MedicineParser.FindFileError(file);
                    return;
                }

                TransferBackgroundWorker.ReportProgress(5, $"找尋檔案 {file.Split('\\').Last()} 錯誤資料位置...");
            }

            File.Delete(destinationFolder + "NewMedicine.txt");

            foreach (string file in sourceFiles)
            {
                if(!MedicineParser.TransferFile(file, destinationFolder))
                    return;
            }

            TransferBackgroundWorker.ReportProgress(5, $"更新伺服器藥品資料...");

            MainViewModel.connection.OpenConnection();
            DataTable dataTable = TransferDB.UpdateServerMedicineData();

            if (dataTable.Rows.Count > 0 && dataTable.Rows[0].Field<string>("RESULT").Equals("SUCCESS"))
            {
                TransferBackgroundWorker.ReportProgress(10, $"更新各店藥品資料...");
                if (TransferDB.UpdateChildMedicineData())
                {
                    TransferBackgroundWorker.ReportProgress(5, $"更新完成!");
                    isSuccess = true;
                }
            }
            MainViewModel.connection.CloseConnection();
        }
        #endregion

        internal void StartTransfer(string source, string destination)
        {
            InitServiceStatus();

            sourceFolder = source;
            destinationFolder = destination;

            if (!IsFolderValid()) return;
            if (!IsSourceFileValid()) return;

            TransferBackgroundWorker.RunWorkerAsync();
        }
        #endregion
    }
}
