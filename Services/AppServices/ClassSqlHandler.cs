using AppServices.Properties;
using Microsoft.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows;

namespace AppServices
{
    public class ClassSqlHandler
    {
        private static readonly object padlock = new object();

        private readonly ClassXml MyXml = new ClassXml();

        private static ClassSqlHandler? _instance = null;

        public const string MASTER_DB = "Master";

        public List<string> RemoveFieldsList = null;
        public List<string>? CustomFieldsList { get; set; }

        public const int BALE_ARCHIVE = 0;
        public const int LOT_ARCHIVE = 1;
        public const int UNIT_ARCHIVE = 2;
        public const int WET_LAYER = 3;
        public const int QULITY_ARCHIVE = 4;

        public static ClassSqlHandler? Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new ClassSqlHandler();
                    }
                    return _instance;
                }
            }
        }

        public string LocalWorkStationID { get; set; } =  Settings.Default.LocalHost;
        public string TargetWorkStationID { get; set; } = Settings.Default.LocalHost;

        public string Host
        {
            get => Settings.Default.Host;
            set
            {
                Settings.Default.Host = value;
                Settings.Default.Save();
            }
        }
        public string SqlInstance
        {
            get => Settings.Default.Instance;
            set
            {
                Settings.Default.Instance = value;
                Settings.Default.Save();
            }
        }

        public string Database
        {
            get => Settings.Default.Database;
            set
            {
                Settings.Default.Database = value;
                Settings.Default.Save();
            }
        }

        public string StrWLDatabase = "ForteLayer";

        public string StrDataSource { get; set; }
        public string ConString { get; set; }
        public string MasterConStr { get; set; }
        public string WLConStr { get; set; }

        public string UserName
        {
            get => Settings.Default.UserName;
            set
            {
                Settings.Default.UserName = value;
                Settings.Default.Save();
            }
        }

        public string Password
        {
            get => Settings.Default.PassWord;
            set
            {
                Settings.Default.PassWord = value;
                Settings.Default.Save();
            }
        }
        public string LocalHost
        {
            get => Settings.Default.LocalHost;
            set
            {
                Settings.Default.LocalHost = value;
                Settings.Default.Save();
                ClassCommon.HostName = value;   
            }
        }

        public bool BLocal
        {
            get => Settings.Default.bLocal;
            set
            {
                Settings.Default.bLocal = value;
                Settings.Default.Save();
            }
        }

        public bool BSerRemote
        {
            get => Settings.Default.bSerRemote;
            set
            {
                Settings.Default.bSerRemote = value;
                Settings.Default.Save();
            }
        }

        public int CbServerSelect
        {
            get => Settings.Default.CbServerSelect;
            set
            {
                Settings.Default.CbServerSelect = value;
                Settings.Default.Save();
            }
        }

        public int MoistureUnit
        {
            get => Settings.Default.MoistureUnit;
            set
            {
                Settings.Default.MoistureUnit = value;
                Settings.Default.Save();
            }
        }

        public int WeightUnit
        {
            get => Settings.Default.WeightUnit;
            set
            {
                Settings.Default.WeightUnit = value;
                Settings.Default.Save();
            }
        }

        public string CurrentBaleTable { get; set; }
        public string PreviousBaleTable { get; set; }



        /// <summary>
        /// First call from SetUpViewModel
        /// </summary>
        public ClassSqlHandler()
        {
            SetUpSql();
        }

        public void  SetUpSql()
        {

            SetSqlParams();
            SetConnectionString();

            ClsSerilog.LogMessage(ClsSerilog.INFO, $"Setup - SQL Parameters");
        }

        private void SetSqlParams()
        {
            LocalWorkStationID = Settings.Default.LocalHost;
            TargetWorkStationID = Settings.Default.Host;

            if (Settings.Default.Instance != null)
                SqlInstance = Settings.Default.Instance;
            else
                SqlInstance = "SQLEXPRESS";

            if (Settings.Default.UserName != null)
                UserName = Settings.Default.UserName;
            else
                UserName = "forte";

            if (Settings.Default.PassWord != null)
                Password = Settings.Default.PassWord;
            else
                Password = "etrof";

           // if (Settings.Default.Database != null)
           //     Database = Settings.Default.Database;
          //  else
          //      Database = "fortedata";

            Database = "fortedata";
        }


        private void SetConnectionString()
        {

            LocalWorkStationID = Settings.Default.LocalHost;
            TargetWorkStationID = Settings.Default.Host;

            if (Settings.Default.bSerRemote)
                StrDataSource = TargetWorkStationID + @"\" + SqlInstance;
            else
                StrDataSource = LocalWorkStationID + @"\" + SqlInstance;

            //Realtime db, SQL authentication connection string
            ConString = SetSqlAuConnString(Database);

            //Wetlayer db, SQL authentication connection string
            WLConStr = SetSqlAuConnString(StrWLDatabase);


            //Master db, SQL authentication connection string 
            MasterConStr = SetSqlAuConnString(MASTER_DB);
        }


        public Task<DataTable> GetServers()
        {
            return Task.Run(() =>
            {
                return SqlDataSourceEnumerator.Instance.GetDataSources();
            });
        }


        private string SetSqlAuConnString(string SqlDatabase)
        {
            return "Data Source ='" + StrDataSource + "'; Database = "
                + SqlDatabase + "; User id= '" + UserName + "'; Password = '"
                + Password + "'; connection timeout=30;Persist Security Info=True;";
        }

        public bool TestSqlConnection(string horst, string strInstance, string database, string username, string passwd)
        {
            bool bConnected = false;

            string Source = horst + @"\" + strInstance;
            string constring = "Data Source = '" + Source + "'; Database = " + database + "; user id = '" + username +
                               "'; Password = '" + passwd + "'; connection timeout=30;Persist Security Info=True;";

            try
            {

                using (var sqlConnection = new SqlConnection(constring))
                {

                    sqlConnection?.Open();
                    bConnected = true;
                    sqlConnection?.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in TestSqlConnection " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in TestSqlConnection < {ex.Message}");
            }
            return bConnected;
        }


        public bool CheckRealTimeDatabase()
        {
            bool bCheck = false;
            try
            {
                bCheck = TestSqlConnection(Host, SqlInstance, Database, UserName, Password);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in MAinWindowViewModel CheckRealTimeDatabase {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in CheckRealTimeDatabase < {ex.Message}");
            }
            return bCheck;
        }



        public Task<string> GetCurrentTableName(int iArchivetype)
        {
            string curTableName = string.Empty;
            List<string> tablelist = new List<string>();
            string strquery = string.Empty;

            switch (iArchivetype)    
            {
                case BALE_ARCHIVE:
                    strquery = "SELECT top 2 [name],create_date FROM sys.tables WHERE [name] LIKE '%BaleArchive%' ORDER BY create_date DESC";
                    break;

                case LOT_ARCHIVE:
                    strquery = "SELECT top 2 [name],create_date FROM sys.tables WHERE [name] LIKE '%BaleArchive%' ORDER BY create_date DESC";
                    break;
            }

     
            return Task.Run(() =>
            {
                try
                {
                    using (var sqlConnection = new SqlConnection(ConString))
                    {
                        sqlConnection?.Open();

                        using (var command = new SqlCommand(strquery, sqlConnection))
                        {
                            using (var reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                    while (reader.Read())
                                    {
                                        if (reader.GetString(0) != null)
                                            tablelist.Add(reader.GetString(0));
                                    }
                            }
                        }
                        sqlConnection?.Close();
                    }
                    curTableName = tablelist[0].ToString();
                    if (tablelist.Count > 1) PreviousBaleTable = tablelist[1].ToString();
                    else PreviousBaleTable = tablelist[0].ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in GetCurrentTableName " + ex.Message);
                    ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GetCurrentTableName < {ex.Message}");
                }
                    return curTableName;
            });
        }


        public async Task<DataTable> GetCurrentDataTableAsyn(string selfield, int items ,string cond)
        {

            var MydataTable = new DataTable();

            await Task.Run(async () =>
            {
                string curTable = await GetCurrentTableName(BALE_ARCHIVE);
                string strquery = $"SELECT TOP {items} {selfield} FROM [{Database}].[dbo].[{curTable}] {cond} ORDER BY [TimeComplete]  DESC";
                
                try
                {
                    using (SqlConnection sqlConnection = new (ConString))
                    {
                        sqlConnection?.Open();
                        using (SqlCommand comm = new (strquery, sqlConnection))
                        {
                            using (SqlDataReader reader = comm.ExecuteReader())
                            {
                                if (reader.HasRows)
                                    MydataTable.Load(reader);
                            }
                        }
                        sqlConnection?.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in GetCurrentDataTableAsyn " + ex.Message);
                    ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GetCurrentDataTableAsyn < {ex.Message}");
                }
            });

            return MydataTable;
        }



        public string GetCurrentBaleTableName()
        {
            List<string> tablelist = new List<string>();
            string strquery = "SELECT top 2 [name],create_date FROM sys.tables WHERE [name] LIKE '%BaleArchive%' ORDER BY create_date DESC";

            try
            {
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strquery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    if (reader.GetString(0) != null)
                                        tablelist.Add(reader.GetString(0));
                                }
                        }
                    }
                    sqlConnection?.Close();
                }

                CurrentBaleTable = tablelist[0].ToString();
                if (tablelist.Count > 1) PreviousBaleTable = tablelist[1].ToString();
                else PreviousBaleTable = tablelist[0].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in GetCurrentBaleTableName " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"EROR in GetCurrentBaleTableName -> {ex.Message}");
            }
            return CurrentBaleTable;
        }


        public DataTable GetBaleArchiveDataTable(string strClause)
        {
            DataTable mytable = new DataTable();

            try
            {
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection?.Open();
                    using (SqlCommand comm = new SqlCommand(strClause, sqlConnection))
                    {
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                mytable.Load(reader);
                        }
                    }
                    sqlConnection?.Close();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error in GetBaleArchiveDataTable " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GetBaleArchiveDataTable < {ex.Message}");
            }

            return mytable;
        }

        public async Task<DataTable> GetBaleArchiveDataTableAsyn(string strquery)
        {
            DataTable mytable = new DataTable();

            try
            {
                await Task.Run(() =>
                {
                    using (SqlConnection sqlConnection = new SqlConnection(ConString))
                    {
                        sqlConnection?.Open();
                        using (SqlCommand comm = new SqlCommand(strquery, sqlConnection))
                        {
                            using (SqlDataReader reader = comm.ExecuteReader())
                            {
                                if (reader.HasRows)
                                    mytable.Load(reader);
                            }
                        }
                        sqlConnection?.Close();
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in GetBaleArchiveDataTable " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GetBaleArchiveDataTable < {ex.Message}");
            }
            return mytable;
        }


        public List<string> GetBigBoxHdrList()
        {
            List<string> hdrList = new List<string>();
            DataTable tempTable = GetSqlScema(GetCurrentRtTableName(ClassSqlHandler.BALE_ARCHIVE));

            foreach (DataRow item in tempTable.Rows)
            {
                hdrList.Add(item[1].ToString());
            }
            //if (hdrList.Contains("UpCount"))
            //    hdrList.Remove("UpCount");

            // if (hdrList.Contains("DownCount"))
            //     hdrList.Remove("DownCount");

            if (hdrList.Contains("SpareSngFld1"))
                hdrList.Remove("SpareSngFld1");

            if (hdrList.Contains("SpareSngFld2"))
                hdrList.Remove("SpareSngFld2");

            /*
             if (hdrList.Contains("SpareSngFld3"))
             {
                 hdrList.Remove("SpareSngFld3");
             //    hdrList.Add("%CV");
             }
             */

            if (hdrList.Contains("AsciiFld1"))
                hdrList.Remove("AsciiFld1");

            if (hdrList.Contains("AsciiFld2"))
                hdrList.Remove("AsciiFld2");

            // if (hdrList.Contains("FC_LotIdentString"))
            //     hdrList.Remove("FC_LotIdentString");

            for (int i = 0; i < hdrList.Count; i++)
            {
                if (hdrList[i].Contains("Finish"))
                    hdrList[i] = "Viscosity";

                if (hdrList[i].Contains("FC_LotIdentString"))
                    hdrList[i] = "CusLotNumber";

                if (hdrList[i].Contains("SpareSngFld3"))
                    hdrList[i] = "%CV";



                if (hdrList[i].Contains("Moisture"))
                {
                    hdrList[i] = ClassCommon.MoistureTypeLst[ClassCommon.MoistureType];
                }
                    
            }

            
           // hdrList.Add("ADWeight");
          //  hdrList.Add("BoneDry%");
          //  hdrList.Add("AirDry%");
          //  hdrList.Add("Dirt_mm2/kg2");
          //  hdrList.Add("Regain%");

            return hdrList;   
        }

        public DataTable GetSqlScema(string curtable)
        {
            DataTable dx = new DataTable();



            string strQuery = $"SELECT ORDINAL_POSITION,COLUMN_NAME,DATA_TYPE FROM ForteData.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{curtable}'";

            try
            {
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection.Open();
                    using (SqlCommand comm = new SqlCommand(strQuery, sqlConnection))
                    {
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                dx.Load(reader);
                        }
                    }
                    sqlConnection?.Close();
                }
                SetRemoveFields();

                foreach (var item in RemoveFieldsList)
                {
                    RemoveHrdItem(dx, item);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in GetSqlScema " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GetSqlScema < {ex.Message}");
            }
            return dx;
        }

        private void RemoveHrdItem(DataTable dx, string stritem)
        {
            foreach (DataRow item in dx.Rows)
            {
                if (item[1].ToString() == stritem)
                {
                    item.Delete();
                }
            }
            dx.AcceptChanges();
        }

        public string GetCurrentRtTableName( int iTableType)
        {
            List<string> tablelist = new List<string>();
            string strquery = string.Empty;

            switch (iTableType)
            {

                case BALE_ARCHIVE:
                    strquery = "SELECT top 2 [name],create_date FROM sys.tables WHERE [name] LIKE '%BaleArchive%' ORDER BY create_date DESC";
                    break;
                    
                    case LOT_ARCHIVE:
                    strquery = "SELECT top 2 [name],create_date FROM sys.tables WHERE [name] LIKE '%LotArchive%' ORDER BY create_date DESC";
                    break;
            }

            try
            {

                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strquery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    if (reader.GetString(0) != null)
                                        tablelist.Add(reader.GetString(0));
                                }
                        }
                    }
                    sqlConnection?.Close();
                }

                if (tablelist.Count > 0) 
                {
                    CurrentBaleTable = tablelist[0].ToString();
                    if (tablelist.Count > 1) PreviousBaleTable = tablelist[1].ToString();
                    else PreviousBaleTable = tablelist[0].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in GetCurrentRtTableName " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GetCuttentRtTableName < {ex.Message}");
            }
            return CurrentBaleTable;
        }


        private void SetRemoveFields()
        {
            if (RemoveFieldsList == null)
            {
                RemoveFieldsList = new List<string>
                {
                    "Index",
                    "Empty",
                    "QualityUID",
                    "AsciiFld1",
                    "AsciiFld2",
                    "OrderStr",
                    "QualityName",
                    "GradeLabel1",
                    "StockLabel1",
                    "StockLabel2",
                    "StockLabel3",
                    "StockLabel4",
                    "JobNum",
                    "Forte1",
                    "Forte2",
                    "ForteAveraging",
                    //RemoveFieldsList.Add("UpCount");
                    //RemoveFieldsList.Add("DownCount");
                    "DownCount2",
                    //RemoveFieldsList.Add("Brightness");
                    "BaleHeight",
                    "SourceId",
                    //RemoveFieldsList.Add("Finish");
                    "SheetArea",
                    "SheetCount",
                    "CalibrationID",
                    "PkgMoistMethod",
                    "SpareSngFld1",
                    "SpareSngFld2",
                    //RemoveFieldsList.Add("SpareSngFld3");
                    "LastInGroup",
                    //"MoistMes",
                    "ProdDayStart",
                    "ProdDayEnd",
                    "SourceID",
                    //RemoveFieldsList.Add("LineID");
                    "StockID",
                    "GradeID",
                    //"WtMes",
                    "AsciiFld3",
                    "AsciiFld4",
                    "SR",
                    "UID",
                    // RemoveFieldsList.Add("Package");
                    "ResultDesc",
                    "GradeLabel2",
                    "WLAlarm",
                    "WLAStatus",
                    //
                    "Status",
                    "WeightStatus",
                    "TemperatureStatus",
                    "OrigWeightStatus",
                    "ForteStatus",
                    "Forte1Status",
                    "Forte2Status",
                    "UpCountStatus",
                    "DownCountStatus",
                    "DownCount2Status",
                    "BrightnessStatus",
                    "TimeStartStatus",
                    "BaleHeightStatus",
                    "TimeStartStatus",
                    "TimeCompleteStatus",
                    "SourceIDStatus",
                    "StockIDStatus",
                    "GradeIDStatus",
                    "TareWeightStatus",
                    "AllowanceStatus",
                    "SheetCountStatus",
                    "MoistureStatus",
                    "NetWeightStatus",
                    "CalibrationIDStatus",
                    "SeriAlNumberStatus",
                    "LotNumberStatus",
                    "TemperatureStatus",
                    "UnitNumberStatus",
                    "UnitIdent",
                    "Temperature"
                };


                //DownCount
                //RemoveFieldsList.Add("FC_IdentString");
                //RemoveFieldsList.Add("Dirt");
                //ItemRemoveLst.Add("BasisWeight")
            }
        }



        public void SetMoistureType(DataTable rDt)
        {
            DataColumnCollection columns = rDt.Columns;
            DataRow[] rows = rDt.Select();

            for (int i = 0; i < rows.Length; i++)
            {
                if (columns.Contains("Moisture"))
                {
                    if (rows[i]["Moisture"] != null)
                        rows[i]["Moisture"] = ClassCommon.CalulateMoisture(rows[i]["Moisture"].ToString(), ClassCommon.MoistureType);
                }
            }
            rDt.AcceptChanges();
        }



        public void SetWeightType(DataTable rDt)
        {
            DataColumnCollection columns = rDt.Columns;
            DataRow[] rows = rDt.Select();

            for (int i = 0; i < rows.Length; i++)
            {
                if (columns.Contains("Weight"))
                {
                    if (rows[i]["Weight"] != null)
                        rows[i]["Weight"] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("Weight"), ClassCommon.WeightUnit);
                }
                if(columns.Contains("OrigWeight"))
                {
                    if (rows[i]["OrigWeight"] != null)
                        rows[i]["OrigWeight"] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("OrigWeight"), ClassCommon.WeightUnit);
                }

                if (columns.Contains("TareWeight"))
                {
                    if (rows[i]["TareWeight"] != null)
                        rows[i]["TareWeight"] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("TareWeight"), ClassCommon.WeightUnit);
                }

                if (columns.Contains("BasisWeight"))
                {
                    if (rows[i]["BasisWeight"] != null)
                        rows[i]["BasisWeight"] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("BasisWeight"), ClassCommon.WeightUnit);
                }

                if (columns.Contains("MinNW"))
                {
                    if (rows[i]["MinNW"] != null)
                        rows[i]["MinNW"] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("MinNW"), ClassCommon.WeightUnit);
                }
                if (columns.Contains("MaxNW"))
                {
                    if (rows[i]["MaxNW"] != null)
                        rows[i]["MaxNW"] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("MaxNW"), ClassCommon.WeightUnit);
                }
               

                /*
                //Lot
                if (columns.Contains("TotNW"))
                {
                    if (rows[i]["TotNW"] != null)
                        rows[i]["TotNW"] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("TotNW"), ClassCommon.WeightUnit);
                }

                if (columns.Contains("MinNW"))
                {
                    if (rows[i]["MinNW"] != null)
                        rows[i]["MinNW"] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("MinNW"), ClassCommon.WeightUnit);
                }

                if (columns.Contains("MinNWBale"))
                {
                    if (rows[i]["MinNWBale"] != null)
                        rows[i]["MinNWBale"] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("MinNWBale"), ClassCommon.WeightUnit);
                }

                if (columns.Contains("MaxNW"))
                {
                    if (rows[i]["MaxNW"] != null)
                        rows[i]["MaxNW"] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("MaxNW"), ClassCommon.WeightUnit);
                }
                if (columns.Contains("MaxNWBale"))
                {
                    if (rows[i]["MaxNWBale"] != null)
                        rows[i]["MaxNWBale"] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("MaxNWBale"), ClassCommon.WeightUnit);
                }

                if (columns.Contains("NW2N"))
                {
                    if (rows[i]["NW2N"] != null)
                        rows[i]["NW2N"] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("NW2N"), ClassCommon.WeightUnit);
                }
                */




            }
            rDt.AcceptChanges();
        }




        public DataTable SetDataFields(DataTable rDt, bool cfChecked)
        {

            if (cfChecked)
            {
                String[] fldary = new String[CustomFieldsList.Count];
                for (int i = 0; i < CustomFieldsList.Count; i++)
                {
                    fldary[i] = CustomFieldsList[i].ToString();
                }
                rDt = new DataView(rDt).ToTable(false, fldary);
            }
            return rDt;
        }

        public void GetCustomFieldsList()
        {
            CustomFieldsList = MyXml.ReadXmlGridView(MyXml.XMLHdrFilePath);
        }

        public List<string> GetLineList()
        {
            return GetUniquIntitemlist("LineName", GetCurrentRtTableName(BALE_ARCHIVE));
        }

        public List<string> GetSourceList()
        {
            return GetUniquIntitemlist("SourceName", GetCurrentRtTableName(ClassSqlHandler.BALE_ARCHIVE));
        }


        /// <summary>
        /// Need to check if the strTable excist
        /// </summary>
        /// <param name="strItem"></param>
        /// <param name="strTable"></param>
        /// <returns></returns>
        public List<string> GetUniquIntitemlist(string strItem, string strTable)
        {
            List<string> itemList = new List<string>();
            string strQuery = "SELECT DISTINCT " + strItem + " From " + strTable + " WHERE " + strItem + " is not null ORDER BY "  + strItem +";";

            try
            {
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strQuery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    if (reader != null)
                                    {
                                       itemList.Add(reader.GetString(0).ToString());
                                    }
                                }
                        }
                    }
                    sqlConnection?.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in GetUniquIntitemlist " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GetUniquInitemlist < {ex.Message}");
            }
            return itemList;
        }


        public  List<string> GettableList(int tabletype) 
        {
            List<string> tablelist = new List<string>();
            string strquery = string.Empty;
            string MyConStr = string.Empty;

            switch (tabletype)
            {
                case BALE_ARCHIVE:
                    strquery = "SELECT [name],create_date FROM sys.tables WHERE [name] LIKE '%BaleArchive%' ORDER BY create_date DESC";
                    MyConStr = ConString;
                    break;
                case LOT_ARCHIVE:
                    strquery = "SELECT [name],create_date FROM sys.tables WHERE [name] LIKE '%LotArchive%' ORDER BY create_date DESC";
                    MyConStr = ConString;
                    break;
                case UNIT_ARCHIVE:
                    strquery = "SELECT [name],create_date FROM sys.tables WHERE [name] LIKE '%UnitArchive%' ORDER BY create_date DESC";
                    MyConStr = ConString;
                    break;
                case WET_LAYER:
                    strquery = "SELECT [name],create_date FROM sys.tables WHERE [name] LIKE '%FValueReadings%' ORDER BY create_date DESC";
                    MyConStr = WLConStr;
                    break;

                case QULITY_ARCHIVE:
                    strquery = "SELECT [name],create_date FROM sys.tables WHERE [name] LIKE '%QualityArchive%' ORDER BY create_date DESC";
                    MyConStr = ConString;
                    break;

                default:

                    break;
            }

            try
            {
                using (var sqlConnection = new SqlConnection(MyConStr))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strquery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    if (reader.GetString(0) != null)
                                        tablelist.Add(reader.GetString(0));
                                }
                        }
                    }
                    sqlConnection?.Close();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in GetTableList " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GettableList < {ex.Message}");
            }
            return tablelist;
        }




        public string GetAllFieldName(int fieldType)
        {
            string fields = string.Empty;
            List<string> listX = new List<string>();

            DataTable HdrTable = GetSqlScema(GetCurrentRtTableName(fieldType));

            if(HdrTable?.Rows.Count > 0)
            {
                listX.Clear();
                for (int i = 0; i < HdrTable?.Rows.Count; i++)
                {
                    listX.Add(HdrTable.Rows[i]["COLUMN_NAME"].ToString());
                }

                if(listX.Count > 0) 
                {
                    string AllList = string.Empty;

                    foreach (var item in listX)
                    {
                        AllList = AllList + item + ",";
                    }
                    AllList = AllList.Remove(AllList.Length - 1);

                    fields = AllList + " ";
                }
            }
            return fields;
        }

        public DataTable GetTableByLotNum(string selectedLot, string strItems, DateTime datestart, DateTime dateEnd, string lotid, string strMonth)
        {
            DataTable LotTable = new DataTable();
            string ArchiveMonth = "BaleArchive" + strMonth;
            string MyQueryString = "SELECT " + strItems + " FROM [ForteData].[dbo].[" + ArchiveMonth + "] with (NOLOCK) WHERE LotNumber = "
                   + selectedLot + " AND TimeStart BETWEEN '" + datestart.AddSeconds(-10) + "' AND '" + dateEnd.AddSeconds(10) + "' ORDER BY [TimeStart] ASC; ";

            try
            {
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection?.Open();
                    using (SqlCommand comm = new SqlCommand(MyQueryString, sqlConnection))
                    {
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                LotTable.Load(reader);
                        }
                    }
                    sqlConnection?.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in GetTableByLotNum" + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GettableByLotNum < {ex.Message}");
            }
            return LotTable;
        }

        public List<string> GetAllWLTables()
        {
            throw new NotImplementedException();
        }

        public List<string> GetUniqueStrItemlist(string strItem, string strTable)
        {

            string constr = ConString;
            List<string> TabList = new List<string>();
            string strQuery =  "SELECT DISTINCT " + strItem + " From " + strTable + " WHERE " + strItem + " is not null ORDER BY " + strItem + ";";
            if (strItem == "BalerID") constr = WLConStr;

            try
            {
                using (var sqlConnection = new SqlConnection(constr))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strQuery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader != null)
                            {
                                if (reader.HasRows)
                                    while (reader.Read())
                                    {
                                        if (reader != null)
                                            TabList.Add(reader[0].ToString());
                                    }
                            }       
                        }
                    }
                    sqlConnection?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in GetUniqueStrItemlist" + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GetUniqueStrItemList < {ex.Message}");
            }
            return TabList;
        }

        public IDisposable GetWetLayerDataTable(string strMonth, string strWetQuery)
        {
           DataTable mytable = new DataTable();

            try
            {
                using (var sqlConnection = new SqlConnection(WLConStr))
                {

                    sqlConnection.Open();
                    using (SqlCommand comm = new SqlCommand(strWetQuery, sqlConnection))
                    {
                        using (SqlDataReader reader = comm.ExecuteReader())
                        {
                            if (reader.HasRows)
                                mytable.Load(reader);
                        }
                    }
                    sqlConnection?.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in GetWetLayerDataTable" + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GetWetLayerDataTable < {ex.Message}");
            }
            return mytable;
        }

        public string GetCurrentWLTable()
        {
            string curTableName = string.Empty;

            List<string> tablelist = new List<string>();
            string strquery = "SELECT TOP 2 [name],create_date FROM sys.tables WHERE [name] LIKE '%FValueReadings%' ORDER BY create_date DESC";

            try
            {

                using (var sqlConnection = new SqlConnection(WLConStr))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strquery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    if (reader.GetString(0) != null)
                                        tablelist.Add(reader.GetString(0));
                                }
                        }
                    }
                    sqlConnection?.Close();

                    curTableName = tablelist[0].ToString();
                    //if (tablelist.Count > 1) PreviousWLTable = tablelist[1].ToString();
                   // else PreviousWLTable = tablelist[0].ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in GetCurrentWLTable" + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GetCuttentWLTable < {ex.Message}");
            }
            return curTableName;
        }

        public bool FindSqlDatabase(string StrTable)
        {
            bool bFoundTable = false;

            string strQuery = "SELECT * FROM sys.databases d WHERE d.database_id>4";

            try
            {

                using (var sqlConnection = new SqlConnection(ConString))
                {
                    if (sqlConnection != null) sqlConnection?.Open();
                    else return false;
                    using (var command = new SqlCommand(strQuery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (reader != null)
                                    {
                                        if (reader[0].ToString() == StrTable)
                                            bFoundTable = true;
                                    }
                                }
                            }
                        }
                    }
                    sqlConnection?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in FindSqlDatabase" + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in FineSqlDatabase < {ex.Message}");
            }
            return bFoundTable;
        }

        public bool CheckDropOption()
        {
            bool bIsDrop = false;
            List<string> DropNumberList = new List<string>();
            List<string> PositionList = new List<string>();

            string strQuery = $"SELECT DISTINCT DropNumber  From {GetCurrentRtTableName(ClassSqlHandler.BALE_ARCHIVE)} WHERE  DropNumber is not null;";
            string strQuery2 = $"SELECT DISTINCT Position  From {GetCurrentRtTableName(ClassSqlHandler.BALE_ARCHIVE)} WHERE  Position is not null;";


            try
            {
                
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection?.Open();

                    //Drop Numbers list
                    using (var command = new SqlCommand(strQuery, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {

                            if(reader != null)
                            {
                                if (reader.HasRows)
                                    while (reader.Read())
                                    {
                                        if (reader != null)
                                            DropNumberList.Add(reader[0].ToString());
                                    }
                            }
                        }
                    }

                    //Positions list
                    using (var command = new SqlCommand(strQuery2, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    if (reader != null)
                                        if (reader[0].ToString() != "0")
                                            PositionList.Add(reader[0].ToString());
                                }
                        }
                    }
                    ClassCommon.BaleInDrop = PositionList.Count;

                    if (DropNumberList.Count > 1) bIsDrop = true;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in CheckDropOption" + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in CheckDropOptino < {ex.Message}");
            }
            return bIsDrop;
          
        }

        public int GetIntNewItemData(string strQueryString)
        {
            int strTarget = 0;

            try
            {
                using (var sqlConnection = new SqlConnection(ConString))
                {
                    sqlConnection?.Open();
                    using (var command = new SqlCommand(strQueryString, sqlConnection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                while (reader.Read())
                                {
                                    if (reader != null)
                                        strTarget = Convert.ToInt32(reader[0]);
                                }
                        }
                    }
                    sqlConnection?.Close();
                }
                return strTarget;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in GetIntNewItemData -> " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"EROR in GetIntNewItemData -> {ex.Message}");
                return 0;
            }
        }

        public DataTable GetSqlScema()
        {
            {
                DataTable dx = new DataTable();
                string strQuery = "SELECT ORDINAL_POSITION,COLUMN_NAME,DATA_TYPE FROM ForteData.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + GetCurrentBaleTableName() + "'";

                try
                {
                    using (var sqlConnection = new SqlConnection(ConString))
                    {

                        sqlConnection.Open();
                        using (SqlCommand comm = new SqlCommand(strQuery, sqlConnection))
                        {
                            using (SqlDataReader reader = comm.ExecuteReader())
                            {
                                if (reader.HasRows)
                                    dx.Load(reader);
                            }
                        }
                        sqlConnection?.Close();
                    }
                    SetRemoveFields();

                    foreach (var item in RemoveFieldsList)
                    {
                        RemoveHrdItem(dx, item);
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Error in GetSqlScema -> " + ex.Message);
                    ClsSerilog.LogMessage(ClsSerilog.ERROR, $"EROR in GetSqlScema -> {ex.Message}");
                }
                return dx;
            }
        }

      
    }
}
