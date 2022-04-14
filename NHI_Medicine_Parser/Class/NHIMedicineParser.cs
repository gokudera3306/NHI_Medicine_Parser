using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using Microsoft.VisualBasic;
using NHI_Medicine_Parser.Database;

namespace NHI_Medicine_Parser.Class
{
    public class NHIMedicineParser : ObservableObject
    {
        #region ----- Define Variables -----

        #region ///// File Statistics /////
        private const int SINGLE_DATA_LENGTH = 1861;

        private NHIDataStruct[] dataStructs = new[]
        {
            //new NHIDataStruct("New_Mark", 1, 2, DataFormat.CHINESE),
            //new NHIDataStruct("口服錠註記", 4, 13, DataFormat.CHINESE),
            //new NHIDataStruct("空白", 446, 603, DataFormat.CHINESE),
            //new NHIDataStruct("藥商名稱", 605, 624, DataFormat.CHINESE),
            //new NHIDataStruct("空白", 626, 766, DataFormat.CHINESE),
            //new NHIDataStruct("藥品分類", 768, 768, DataFormat.NUMBER),
            //new NHIDataStruct("品質分類碼", 770, 770, DataFormat.ENGLISH),
            //new NHIDataStruct("分類分組名稱", 901, 1200, DataFormat.CHINESE),
            //new NHIDataStruct("未生產或未輸入達五年", 1859, 1859, DataFormat.ENGLISH)

            new NHIDataStruct("單複方註記", 15, 16, DataFormat.ENGLISH),
            new NHIDataStruct("|$$|", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("藥品代碼", 18, 27, DataFormat.ENGLISH),
            new NHIDataStruct("|$$|", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("藥價參考金額", 29, 37, DataFormat.NUMBER),
            new NHIDataStruct("|$$|", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("藥價參考日期", 39, 45, DataFormat.DATE),
            new NHIDataStruct("|$$|", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("藥價參考截止日期", 47, 53, DataFormat.DATE),
            new NHIDataStruct("|$$|", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("藥品英文名稱", 55, 174, DataFormat.CAPITAL_ENGLISH),
            new NHIDataStruct("|$$|", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("藥品規格量", 176, 182, DataFormat.NUMBER),
            new NHIDataStruct("|$$|", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("藥品規格單位", 184, 235, DataFormat.ENGLISH),
            new NHIDataStruct("|$$|", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("成分名稱", 237, 292, DataFormat.CAPITAL_ENGLISH),
            new NHIDataStruct(" ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("成分含量", 294, 305, DataFormat.NUMBER),
            new NHIDataStruct(" ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("成分含量單位", 307, 357, DataFormat.ENGLISH),
            new NHIDataStruct(" + ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方1成分名稱", 1201, 1256, DataFormat.CAPITAL_ENGLISH),
            new NHIDataStruct(" ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方1成分含量", 1259, 1269, DataFormat.NUMBER),
            new NHIDataStruct(" ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方1成分含量單位", 1271, 1321, DataFormat.ENGLISH),
            new NHIDataStruct(" + ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方2成分名稱", 1323, 1378, DataFormat.CAPITAL_ENGLISH),
            new NHIDataStruct(" ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方2成分含量", 1380, 1390, DataFormat.NUMBER),
            new NHIDataStruct(" ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方2成分含量單位", 1392, 1442, DataFormat.ENGLISH),
            new NHIDataStruct(" + ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方3成分名稱", 1444, 1499, DataFormat.CAPITAL_ENGLISH),
            new NHIDataStruct(" ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方3成分含量", 1501, 1511, DataFormat.NUMBER),
            new NHIDataStruct(" ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方3成分含量單位", 1513, 1563, DataFormat.ENGLISH),
            new NHIDataStruct(" + ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方4成分名稱", 1565, 1620, DataFormat.CAPITAL_ENGLISH),
            new NHIDataStruct(" ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方4成分含量", 1622, 1632, DataFormat.NUMBER),
            new NHIDataStruct(" ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方4成分含量單位", 1634, 1684, DataFormat.ENGLISH),
            new NHIDataStruct(" + ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方5成分名稱", 1686, 1741, DataFormat.CAPITAL_ENGLISH),
            new NHIDataStruct(" ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方5成分含量", 1743, 1753, DataFormat.NUMBER),
            new NHIDataStruct(" ", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("複方5成分含量單位", 1755, 1805, DataFormat.ENGLISH),
            new NHIDataStruct("|$$|", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("藥品劑型", 359, 444, DataFormat.CHINESE),
            new NHIDataStruct("|$$|", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("藥品中文名稱", 772, 899, DataFormat.CHINESE),
            new NHIDataStruct("|$$|", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("製造商名稱", 1807, 1848, DataFormat.CHINESE),
            new NHIDataStruct("|$$|", 0, 0, DataFormat.SPLIT),
            new NHIDataStruct("ATC_CODE", 1850, 1857, DataFormat.ENGLISH),
        };
        #endregion

        #endregion

        #region ----- Define Functions -----

        #region ///// Public Transfer Functions /////
        public bool CheckFileFormat(string file)
        {
            TransferService.TransferBackgroundWorker.ReportProgress(5, "檢查檔案 " + file.Split('\\').Last() + " ...");

            byte[] data = File.ReadAllBytes(file);
            
            if (data.Length % SINGLE_DATA_LENGTH != 0)
                return false;

            return true;
        }
        public void FindFileError(string file)
        {
            byte[] data = File.ReadAllBytes(file);

            string errorRows = $"檔案 {file.Split('\\').Last()} 錯誤:\n";
            int currentIndex = 0;
            int endRowIndex = 0;
            int rowCount = 0;

            while (currentIndex < data.Length)
            {
                rowCount++;

                endRowIndex = Array.IndexOf(data, (byte)10, currentIndex);

                if (endRowIndex - currentIndex + 1 != 1861)
                {
                    errorRows += rowCount;
                    errorRows += (endRowIndex - currentIndex + 1 < 1861) ? "行過短\n" : "行過長\n";
                }

                currentIndex = endRowIndex + 1;
            }

            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageWindow.ShowMessage(errorRows, MessageType.ERROR);
            }));
        }
        public bool TransferFile(string file, string destinationFolder)
        {
            TransferService.TransferBackgroundWorker.ReportProgress(0, "轉換檔案 " + file.Split('\\').Last() + " ...");

            byte[] data = File.ReadAllBytes(file);
            string errorRows = $"檔案 {file.Split('\\').Last()} 錯誤:\n";

            using (StreamWriter writer = new StreamWriter(destinationFolder + "NewMedicine.txt", true))
            {
                for (int x = 0; x < data.Length; x += SINGLE_DATA_LENGTH)
                {
                    string transferData = "";
                    byte[] rowBytes = new byte[SINGLE_DATA_LENGTH];

                    Array.Copy(data, x, rowBytes, 0, SINGLE_DATA_LENGTH);

                    bool isSuccess = NHIRowParser(rowBytes, out transferData);

                    if (!isSuccess)
                    {
                        errorRows += $"{x / SINGLE_DATA_LENGTH + 1}行 - {transferData}";

                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            MessageWindow.ShowMessage(errorRows, MessageType.ERROR);
                        }));

                        return false;
                    }

                    TransferDB.AddDataToTransferMedicineTable(transferData);
                    writer.WriteLine(transferData);

                    if( (x / SINGLE_DATA_LENGTH) % (data.Length / SINGLE_DATA_LENGTH / 10) == 0)
                        TransferService.TransferBackgroundWorker.ReportProgress(3, "轉換檔案 " + file.Split('\\').Last() + " ...");
                }
            }

            return true;
        }
        #endregion

        private bool NHIRowParser(byte[] rowBytes, out string transferData)
        {
            transferData = "";

            foreach (NHIDataStruct dataStruct in dataStructs)
            {
                if (dataStruct.Format == DataFormat.SPLIT)
                {
                    transferData += dataStruct.Name;
                    continue;
                }

                byte[] columnBytes = new byte[dataStruct.DataLength];

                Array.Copy(rowBytes, dataStruct.StartIndex - 1, columnBytes, 0, dataStruct.DataLength);

                string tempData = Encoding.GetEncoding(950).GetString(columnBytes).Trim();

                if (!CheckFormat(ref tempData, dataStruct.Format))
                {
                    transferData = tempData;
                    return false;
                }

                transferData += tempData;
            }

            for (int i = 0; i < 5; i++)
            {
                transferData = transferData.Replace(" +   |$$|", "|$$|");
            }

            return true;
        }

        private bool CheckFormat(ref string tempData, DataFormat format)
        {
            switch (format)
            {
                case DataFormat.CAPITAL_ENGLISH:
                    string[] tempStrings = tempData.Split(' ');
                    Regex englishRegex = new Regex("[A-Za-z]");
                    tempData = "";

                    foreach (string s in tempStrings)
                    {
                        if(s.Equals(String.Empty)) continue;

                        Match match = englishRegex.Match(s.Substring(0, 1));

                        if (!match.Success && s.Length > 1)
                            tempData += s.Substring(0, 2).ToUpper() + s.Substring(2).ToLower() + " ";
                        else
                            tempData += s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower() + " ";
                    }

                    tempData = tempData.Trim();
                    break;
                case DataFormat.CHINESE:
                    tempData = Strings.StrConv(tempData, VbStrConv.Narrow, 0);
                    break;
                case DataFormat.ENGLISH:
                    break;
                case DataFormat.DATE:
                    if (tempData.Length != 7)
                    {
                        tempData = "日期長度錯誤!";
                        return false;
                    }
                    else
                    {
                        string originalDate = tempData; 
                        tempData = (int.Parse(tempData.Substring(0, 3)) + 1911) + "/" + tempData.Substring(3, 2) + "/" + tempData.Substring(5, 2);

                        DateTime tempDateTime = new DateTime();

                        if (!DateTime.TryParse(tempData, out tempDateTime))
                        {
                            tempData = $"日期資料錯誤!({originalDate})";
                            return false;
                        }
                    }
                    break;
                case DataFormat.NUMBER:
                    double tempDouble = 0;

                    if (tempData.Equals("-"))
                        tempData = "0";
                    else if (!tempData.Equals(String.Empty) && !double.TryParse(tempData, out tempDouble))
                    {
                        tempData = $"數字資料錯誤!({tempData})";
                        return false;
                    }
                    break;
            }

            return true;
        }

        #endregion
    }
}
