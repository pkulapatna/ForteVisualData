using System;
using System.Data;
using System.Windows;
using System.Data.Odbc;

namespace AppServices
{
    public class ClassAccessHandler
    {
        private static readonly object padlock = new object();
        private static ClassAccessHandler? _instance = null;

        private readonly string dbProvider = "PROVIDER=Microsoft.ACE.OLEDB.12.0;";
        private readonly string DB_REPJ4 = @"Data Source=C:\\ForteSystem\Reports\ReportsJ4.mdb;Persist Security Info=True;";

        public DataTable BaleArchiveTable { get; set; }

        public static ClassAccessHandler Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new ClassAccessHandler();
                    }
                    return _instance;
                }
            }
        }

    

        public ClassAccessHandler()
        {
            ClsSerilog.LogMessage(ClsSerilog.INFO, $"Config Access Database ......................");
        }

        public DataTable GetAccessArchiveTable() 
        {
            DataTable mytable = new DataTable();
            string strQuery = "SELECT * FROM [BaleArchive]";
            string connectionString = dbProvider + DB_REPJ4;

 
            try
            {       /*      
                    using (OleDbConnection connection = new OleDbConnection(connectionString))
                    {

                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(strQuery, connection))
                        {
                            adapter.Fill(mytable);
                        }
                    }
                    */

                }
            catch (Exception ex )
            {
                MessageBox.Show("EROR in GetAccessArchiveTable " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"EROR in GetLVHdrFmtBaleTable -> {ex.Message}");
            }
            return mytable;
        }


        public DataTable GetAccessArchiveTable2()
        {
            DataTable mytable = new DataTable();
            string strQuery = "SELECT * FROM [BaleArchive]";
           

        //    string ConnectionString = "Driver={Microsoft Access Driver (*.mdb)}; Dbq=C:\\ForteSystem\\Reports\\ReportsJ4.mdb; ";

            string ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\ForteSystem\\Reports\\ReportsJ4.mdb;";


            try
            {
                
                using (OdbcConnection connection = new OdbcConnection(ConnectionString))
                {
                    connection?.Open();

                    using (var command = new OdbcCommand(strQuery))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    if (reader.HasRows)
                                        mytable.Load(reader);
                                }
                        }
                    }
                    connection?.Close();
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("EROR in GetAccessArchiveTable " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"EROR in GetLVHdrFmtBaleTable -> {ex.Message}");
            }
            return mytable;
        }

    }
}
