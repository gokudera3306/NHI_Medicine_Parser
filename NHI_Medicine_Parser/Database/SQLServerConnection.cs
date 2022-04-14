using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHI_Medicine_Parser.Class;

namespace NHI_Medicine_Parser.Database
{
    public class SQLServerConnection
    {
        private SqlConnection connection = new SqlConnection("Data Source=59.124.201.229,59087;Persist Security Info=True;User ID=HISPOSUser;Password=HISPOSPassword");
        private bool isBusy = false;

        public bool CheckConnection()
        {
            try
            {
                connection.Open();
            }
            catch (SqlException e)
            {
                return false;
            }

            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
                return true;
            }

            return false;
        }

        public void OpenConnection()
        {
            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
            }
            catch (Exception e)
            {
                MessageWindow.ShowMessage("網路異常 無法連線到資料庫", MessageType.ERROR);
            }
        }

        public void CloseConnection()
        {
            if (connection.State != ConnectionState.Executing && connection.State != ConnectionState.Fetching && connection.State == ConnectionState.Open)
                connection.Close();
        }

        public DataTable ExecuteProc(string procName, List<SqlParameter> parameterList = null)
        {
            while (isBusy)
                Thread.Sleep(500);

            isBusy = true;

            var table = new DataTable();
            try
            {
                SqlCommand myCommand = new SqlCommand("[HIS_POS_Server]." + procName, connection);

                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.CommandTimeout = 120;
                if (parameterList != null)
                    foreach (var param in parameterList)
                    {
                        myCommand.Parameters.Add(param);
                    }

                var sqlDapter = new SqlDataAdapter(myCommand);
                table.Locale = CultureInfo.InvariantCulture;
                sqlDapter.Fill(table);
            }
            catch (SqlException sqlException)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageWindow.ShowMessage(procName + sqlException.Message, MessageType.ERROR);
                });
            }
            catch (Exception ex)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate {
                    MessageWindow.ShowMessage("預存程序 " + procName + "執行失敗\r\n原因:" + ex.Message, MessageType.ERROR);
                });
            }

            isBusy = false;

            return table;
        }
        public DataTable ExecuteProcBySchema(string schema, string procName, List<SqlParameter> parameterList = null)
        {
            while (isBusy)
                Thread.Sleep(500);

            isBusy = true;

            var table = new DataTable();
            try
            {
                SqlCommand myCommand = new SqlCommand("[" + schema + "]." + procName, connection);

                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.CommandTimeout = 120;
                if (parameterList != null)
                    foreach (var param in parameterList)
                    {
                        myCommand.Parameters.Add(param);
                    }

                var sqlDapter = new SqlDataAdapter(myCommand);
                table.Locale = CultureInfo.InvariantCulture;
                sqlDapter.Fill(table);
            }
            catch (SqlException sqlException)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageWindow.ShowMessage(procName + sqlException.Message, MessageType.ERROR);
                });
            }
            catch (Exception ex)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate {
                    MessageWindow.ShowMessage("預存程序 " + procName + "執行失敗\r\n原因:" + ex.Message, MessageType.ERROR);
                });
            }

            isBusy = false;

            return table;
        }
    }
}
