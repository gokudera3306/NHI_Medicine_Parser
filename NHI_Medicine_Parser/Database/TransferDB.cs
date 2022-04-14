using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHI_Medicine_Parser.ViewModel;

namespace NHI_Medicine_Parser.Database
{
    public static class TransferDB
    {
        #region ----- DataTable Functions -----
        private static DataTable TransferMedicineTable()
        {
            DataTable masterTable = new DataTable();
            masterTable.Columns.Add("Med_SC", typeof(string));
            masterTable.Columns.Add("Med_ID", typeof(string));
            masterTable.Columns.Add("Med_Price", typeof(double));
            masterTable.Columns.Add("Med_SDate", typeof(DateTime));
            masterTable.Columns.Add("Med_EDate", typeof(DateTime));
            masterTable.Columns.Add("Med_EnglishName", typeof(string));
            masterTable.Columns.Add("Med_UnitAmount", typeof(double));
            masterTable.Columns.Add("Med_Unit", typeof(string));
            masterTable.Columns.Add("Med_Ingredient", typeof(string));
            masterTable.Columns.Add("Med_Form", typeof(string));
            masterTable.Columns.Add("Med_ChineseName", typeof(string));
            masterTable.Columns.Add("Med_Manufactory", typeof(string));
            masterTable.Columns.Add("Med_ATC", typeof(string));
            return masterTable;
        }
        private static void AddColumnValue(DataRow row, string column, Object value)
        {
            if (value is null || string.IsNullOrEmpty(value.ToString()))
                row[column] = DBNull.Value;
            else
                row[column] = value;
        }
        #endregion

        #region ----- Define Variables -----
        private static DataTable storeOrderMasterTable = TransferMedicineTable();
        #endregion

        #region ----- Define Functions -----
        public static void AddDataToTransferMedicineTable(string rowData)
        {
            var rowDatas = rowData.Split(new []{"|$$|"}, StringSplitOptions.None);

            DataRow newRow = storeOrderMasterTable.NewRow();
            AddColumnValue(newRow, "Med_SC", rowDatas[0]);
            AddColumnValue(newRow, "Med_ID", rowDatas[1]);
            AddColumnValue(newRow, "Med_Price", double.Parse(rowDatas[2]));
            AddColumnValue(newRow, "Med_SDate", DateTime.Parse(rowDatas[3]));
            AddColumnValue(newRow, "Med_EDate", DateTime.Parse(rowDatas[4]));
            AddColumnValue(newRow, "Med_EnglishName", rowDatas[5]);
            AddColumnValue(newRow, "Med_UnitAmount", double.Parse(rowDatas[6].Equals("")? "0" : rowDatas[6]));
            AddColumnValue(newRow, "Med_Unit", rowDatas[7]);
            AddColumnValue(newRow, "Med_Ingredient", rowDatas[8]);
            AddColumnValue(newRow, "Med_Form", rowDatas[9]);
            AddColumnValue(newRow, "Med_ChineseName", rowDatas[10]);
            AddColumnValue(newRow, "Med_Manufactory", rowDatas[11]);
            AddColumnValue(newRow, "Med_ATC", rowDatas[12]);
            storeOrderMasterTable.Rows.Add(newRow);
        }

        internal static DataTable UpdateServerMedicineData()
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("NEW_MED", storeOrderMasterTable));

            return MainViewModel.connection.ExecuteProc("[Set].[UpdateNHIMedicine]", parameters);
        }

        internal static bool UpdateChildMedicineData()
        {
            string[] schemas = {"SD_XingChang", "SD_HongChang", "SD_YoChang", "SD_YoDong", "SD_MingChang", "SD_HeChang"};

            foreach (var schema in schemas)
            {
                DataTable dataTable = MainViewModel.connection.ExecuteProcBySchema(schema, "[Set].[GetNewNHIMedicines]");
                if (dataTable.Rows.Count == 0 || dataTable.Rows[0].Field<string>("RESULT").Equals("FAIL"))
                    return false;
            }

            return true;
        }
        #endregion
    }
}
