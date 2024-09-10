using AppServices;
using AppServices.Control;
using DataFieldsSelect.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows;
using static AppServices.ClassApplicationService;

namespace ModSetup.ViewModels
{
    public class SetUpViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator = new EventAggregator();
        public int Id { get; set; }

        private FieldSelectView MyDataItems;

        private string CurrentBaleTable { get; set; }
        private string PreviousBaleTable { get; set; }

        private ClassSqlHandler _sqlhandler;// = ClassSqlHandler.Instance;

        private string _sQLStatusMsg;
        public string SQLStatusMsg
        {
            get { return _sQLStatusMsg; }
            set { SetProperty(ref _sQLStatusMsg, value); }
        }

        public static string ModuleName = "Setup SQL Server and Application Parameters";



        private bool _localChecked;
        public bool LocalChecked
        {
            get => _localChecked; 
            set { SetProperty(ref _localChecked, value); }
        }

        private bool _remoteChecked;
        public bool RemoteChecked
        {
            get => _remoteChecked;
            set { SetProperty(ref _remoteChecked, value); }
        }


        // 600000 mSec = 600 Sec. = 10 minutes. Max.
        // Default 5 sec., MIn 1 Sec.
        private int _scanrate = ClassCommon.ScanSec;
        public int ScanRate
        {
            get { return _scanrate; }
            set
            {
                if ((value > 0) & (value < 601))
                    SetProperty(ref _scanrate, value);
                else
                    SetProperty(ref _scanrate, 5);
            }
        }

        private int _bsample = ClassCommon.ComBineSample;
        public int BSamples
        {
            get => _bsample;
            set
            {
                if ((value > 0) & (value < 5001))
                    SetProperty(ref _bsample, value);
                else
                    SetProperty(ref _bsample, 100);

                ClassCommon.ComBineSample = _bsample;
            }
        }


        private bool _mcChecked;
        public bool MCChecked
        {
            get => _mcChecked;
            set => SetProperty(ref _mcChecked, value);
        }

        private bool _mrChecked;
        public bool MRChecked
        {
            get { return _mrChecked; }
            set { SetProperty(ref _mrChecked, value); }
        }

        private bool _adChecked;
        public bool ADChecked
        {
            get { return _adChecked; }
            set { SetProperty(ref _adChecked, value); }
        }

        private bool _bdChecked;
        public bool BDChecked
        {
            get { return _bdChecked; }
            set { SetProperty(ref _bdChecked, value); }
        }


        private DateTime _prodDayEnd = ClassCommon.ProdDayEnd;   
        public DateTime ProdDayEnd
        {
            get { return _prodDayEnd; }
            set
            {
                SetProperty(ref _prodDayEnd, value);
                ClassCommon.ProdDayEnd = value;
            }
        }

        private bool _kgChecked;
        public bool KGChecked
        {
            get { return _kgChecked; }
            set 
            { 
                SetProperty(ref _kgChecked, value); 
                if(value == true)
                {
                    ClassCommon.WeightUnit = 0;
                }
            }
        }

        private bool _lbChecked;
        public bool LBChecked
        {
            get { return _lbChecked; }
            set 
            { 
                SetProperty(ref _lbChecked, value);
                if (value == true)
                {
                    ClassCommon.WeightUnit = 1;
                }
            }
        }


        private bool _bModify = false;
        public bool BModify
        {
            get => _bModify; 
            set 
            { 
                SetProperty(ref _bModify, value); 
                if(value == true)
                {
                    ShowOPC = "1";
                }
                else
                {
                    ShowOPC = "0.3";
                }
        
            }
        }

        private bool _bTesting = false;
        public bool BTesting
        {
            get { return _bTesting; }
            set { SetProperty(ref _bTesting, value); }
        }

        private string _LocalHost = Environment.MachineName;
        public string LocalHost
        {
            get { return _LocalHost; }
            set { SetProperty(ref _LocalHost, value); }
        }

        private string _host ;
        public string Host
        {
            get { return _host; }
            set { SetProperty(ref _host, value); }
        }

        private string _instance;
        public string Instance
        {
            get { return _instance; }
            set { SetProperty(ref _instance, value); }
        }

        private string _userid;
        public string Userid
        {
            get { return _userid; }
            set
            {
                SetProperty(ref _userid, value);
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private string _database;
        public string Database
        {
            get { return _database; }
            set { SetProperty(ref _database, value); }
        }

        private bool _blocal;
        public bool BLocal
        {
            get { return _blocal; }
            set { SetProperty(ref _blocal, value); }
        }

        private bool _bremote;
        public bool BRemote
        {
            get { return _bremote; }
            set { SetProperty(ref _bremote, value); }
        }

        private List<string> _servercomboList = new List<string>();
        public List<string> ServercomboList
        {
            get =>_servercomboList; 
            set => SetProperty(ref _servercomboList, value); 
        }


        private bool _SearchDone = false;
        public bool SearchDone
        {
            get => _SearchDone; 
            set => SetProperty(ref _SearchDone, value); 
        }

        private int _selectedServerCombo;
        public int SelectedServerCombo
        {
            get => _selectedServerCombo;
            set
            {
                SetProperty(ref _selectedServerCombo, value);
                char[] separators = { '\\' };  //Host\\Instant
                string strNewHost = ServercomboList[SelectedServerCombo].ToString();
                string[] words = strNewHost.Split(separators);
                Host = words[0];
                Instance = words[1];
            }
        }

        private int _baleinDrop = ClassCommon.BaleInDrop;
        public int BaleinDrop
        {
            get { return _baleinDrop; }
            set { SetProperty(ref _baleinDrop, value); }
        }
        
        #region Active Modules

        //1
        private bool _realTimeCheck = ClassCommon.RealTimeCheck;
        public bool RealTimeCheck
        {
            get => _realTimeCheck;
            set 
            { 
                SetProperty(ref _realTimeCheck, value);
            }
        }

        //2
        private bool _graphCheck = true; // ClassCommon.GraphCheck;
        public bool GraphCheck
        {
            get => _graphCheck;
            set 
            { 
                SetProperty(ref _graphCheck, value);
               // ClassCommon.GraphCheck = value ? true : false;
            }
        }
        //3
        private bool _combineCheck = ClassCommon.CombineCheck;
        public bool CombineCheck
        {
            get => _combineCheck;
            set 
            { 
                SetProperty(ref _combineCheck, value);
               // ClassCommon.CombineCheck = value ? true : false;
            }
        }
        //4
        private bool _dualGraphCheck = ClassCommon.DualGraphCheck;
        public bool DualGraphCheck
        {
            get => _dualGraphCheck;
            set
            {
                SetProperty(ref _dualGraphCheck, value);
               // ClassCommon.DualGraphCheck = value ? true : false;
            }
        }

        //5
        private bool _archiveCheck = ClassCommon.ArchiveCheck;
        public bool ArchiveCheck
        {
            get => _archiveCheck;
            set
            {
                SetProperty(ref _archiveCheck, value);
               // ClassCommon.ArchiveCheck = value ? true : false;
            }
        }

        //6
        private bool _wetLayerCheck = ClassCommon.WetLayerCheck;
        public bool WetLayerCheck
        {
            get => _wetLayerCheck;
            set
            {
                SetProperty(ref _wetLayerCheck, value);
                //ClassCommon.WetLayerCheck = value ? true : false;
            }
        }

        //7
        private bool _wLTrendCheck = ClassCommon.WLTrendCheck;
        public bool WLTrendCheck
        {
            get => _wLTrendCheck;
            set{SetProperty(ref _wLTrendCheck, value);}
        }

        //8
        private Visibility _showWetLayer = Visibility.Hidden;
        public Visibility ShowWetLayer
        {
            get => _showWetLayer;
            set
            {
                SetProperty(ref _showWetLayer, value);
            }
        }

        private Visibility _showWLTrend = Visibility.Hidden;
        public Visibility ShowWLTrend
        {
            get => _showWLTrend;
            set
            {
                SetProperty(ref _showWLTrend, value);
            }
        }



        private Visibility _showDrop = Visibility.Hidden;
        public Visibility ShowDrop
        {
            get => _showDrop;
            set
            {
                SetProperty(ref _showDrop, value);
            }
        }


        private bool _graphDarkMode = ClassCommon.GraphDarkMode;
        public bool GraphDarkMode
        {
            get => _graphDarkMode;
            set
            {
                SetProperty(ref _graphDarkMode, value);
                ClassCommon.GraphDarkMode = value ? true : false;
            }
        }


        #endregion Active Modules


        #region DropOption


        private bool _dropHitoLow = ClassCommon.DropHitoLow;
        public bool DropHitoLow
        {
            get => _dropHitoLow;
            set
            {
                SetProperty(ref _dropHitoLow, value);
                //ClassCommon.DropHitoLow = value ? true : false;
            }
        }
        private bool _dpProfileChecked = ClassCommon.DpProfileChecked;
        public bool DpProfileChecked
        {
            get => _dpProfileChecked;
            set
            {
                SetProperty(ref _dpProfileChecked, value);
                //ClassCommon.DpProfileChecked = value ? true : false;
            }
        }
        private bool _dpPostionChecked = ClassCommon.DpPostionChecked;
        public bool DpPostionChecked
        {
            get => _dpPostionChecked;
            set
            {
                SetProperty(ref _dpPostionChecked, value);
                //ClassCommon.DpPostionChecked = value ? true : false;
            }
        }

        #endregion DropOPtion

        private string _showOPC = "0.3";
        public string ShowOPC
        {
            get { return _showOPC; }
            set { SetProperty(ref _showOPC, value); }
        }

        /// <summary>
        /// 1. Check for Network connection for remote connection
        /// 2. Check for Sql connection to BaleArchive database
        /// 3. Check Wet Layer database, if using for the 2760 (TextTile)
        /// 4. Check for Drop option, 
        /// </summary>
        public async void LoadPage()
        {

            Host = _sqlhandler.Host;
            Instance = _sqlhandler.SqlInstance;
            Userid = _sqlhandler.UserName;
            Password = _sqlhandler.Password;
            Database = _sqlhandler.Database;

            ClsSerilog.LogMessage(ClsSerilog.INFO, $"Host to connect = {Host}");

            RealTimeCheck = ClassCommon.RealTimeCheck;
            GraphCheck = ClassCommon.GraphCheck;
            CombineCheck= ClassCommon.CombineCheck;
            DualGraphCheck = ClassCommon.DualGraphCheck;

            ArchiveCheck = ClassCommon.ArchiveCheck;

            if (LocalHost == Host)
            {
                LocalChecked = true;
                ClassCommon.LocalChecked = true;
            }
            else 
            {
                RemoteChecked = true;
                ClassCommon.LocalChecked = false;
            }
            


            if (ClassCommon.WLOptions)
            {
                WetLayerCheck = ClassCommon.WetLayerCheck;
                WLTrendCheck = ClassCommon.WLTrendCheck;
            }
            else
            {
                WetLayerCheck = false;
                WLTrendCheck = false;
            }
               

            if (ClassCommon.DropOption == true)
            {
                DropHitoLow = ClassCommon.DropHitoLow;
                DpProfileChecked = ClassCommon.DpProfileChecked;
                DpPostionChecked = ClassCommon.DpPostionChecked;
            }
            else
            {
                DropHitoLow = false;
                DpProfileChecked = false;
                DpPostionChecked = false;
            }

            //ClsSerilog.LogMessage(ClsSerilog.INFO, $"Network Available? = {System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()}");

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                SQLStatusMsg = $"Set Connection to {Host} => SQL Server";
                CurrentBaleTable = await _sqlhandler.GetCurrentTableName(ClassSqlHandler.BALE_ARCHIVE);

                if (ClassCommon.WeightUnit == 0) KGChecked = true;
                else if (ClassCommon.WeightUnit == 1) LBChecked = true;

                //Bale Archives always be there but only the 2760 has Wet Layer, check here!
                ClassCommon.WLOptions = _sqlhandler.FindSqlDatabase("ForteLayer");
                ShowWetLayer = ClassCommon.WLOptions ? Visibility.Visible : Visibility.Collapsed;
                ShowWLTrend = ClassCommon.WLOptions ? Visibility.Visible : Visibility.Collapsed;

                //Check for Drop Option
                ClassCommon.DropOption = _sqlhandler.CheckDropOption();
                ShowDrop = ClassCommon.DropOption ? Visibility.Visible : Visibility.Collapsed;

                if (ClassCommon.DropOption == true)
                    BaleinDrop = ClassCommon.BaleInDrop;


                /*
                bool bSQLChecked = _sqlhandler.TestSqlConnection(Host, Instance, Database, Userid, Password);

                ClsSerilog.LogMessage(ClsSerilog.INFO, $"bSQLChecked= {bSQLChecked}");

                if (bSQLChecked)
                {
                    SQLStatusMsg = $"Set Connection to {Host} => SQL Server";
                    CurrentBaleTable = await _sqlhandler.GetCurrentTableName(ClassSqlHandler.BALE_ARCHIVE);

                    if (ClassCommon.WeightUnit == 0) KGChecked = true;
                    else if (ClassCommon.WeightUnit == 1) LBChecked = true;

                    //Bale Archives always be there but only the 2760 has Wet Layer, check here!
                    ClassCommon.WLOptions = _sqlhandler.FindSqlDatabase("ForteLayer");
                    ShowWetLayer = ClassCommon.WLOptions ? Visibility.Visible : Visibility.Collapsed;

                    //Check for Drop Option
                    ClassCommon.DropOption = _sqlhandler.CheckDropOption();
                    ShowDrop = ClassCommon.DropOption ? Visibility.Visible : Visibility.Collapsed;

                    BaleinDrop = ClassCommon.BaleInDrop;

                }
                else
                {
                    SQLStatusMsg = $"NO SQL Connection!!";
                    ClsSerilog.LogMessage(ClsSerilog.INFO, $"No SQL Connections");
                }*/
            }

          



            switch (ClassCommon.MoistureType) 
            {
                case 0:
                    MCChecked = true;
                    break;
                case 1:
                    MRChecked = true;
                    break;
                case 2:
                    ADChecked = true;
                    break;
                case 3:
                    BDChecked = true;
                    break;
                default: 
                    break;
            }

            //ClsSerilog.LogMessage(ClsSerilog.INFO, $"---------- Load Page Setup");
        }

        /// <summary>
        /// Leave Page Clean Up
        /// </summary>
        public void UnloadPage()
        {

        }

        public SetUpViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            
            if(_sqlhandler == null)
                _sqlhandler = ClassSqlHandler.Instance;
            
            SQLStatusMsg = "from SetUpViewModel Module";

         
            LoadPage();
        }

        private DelegateCommand _searchComand;
        public DelegateCommand SearchCommand =>
            _searchComand ?? (_searchComand = new DelegateCommand(SearchExecute).ObservesCanExecute(() => BModify));
        private void SearchExecute()
        {
            SQLStatusMsg = "Searching for SQL Server";

            ClsSerilog.LogMessage(ClsSerilog.INFO, $"Searching for SQL Server");

            SearchSqlServers();
            
        }

        private DelegateCommand _modifyCommand;
        public DelegateCommand ModifyCommand =>
        _modifyCommand ?? (_modifyCommand = new DelegateCommand(ModifyExecute));
        private void ModifyExecute()
        {
            BModify = true;

            ClsSerilog.LogMessage(ClsSerilog.INFO, $"Modify = {BModify}");
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
        _cancelCommand ?? (_cancelCommand = new DelegateCommand(CancelExecute).ObservesCanExecute(() => BModify));

        private void CancelExecute()
        {
            BModify = false;
            LoadPage();
        }

        private DelegateCommand _applyCommand;
        public DelegateCommand ApplyCommand =>
        _applyCommand ?? (_applyCommand = new DelegateCommand(ApplyExecute).ObservesCanExecute(() => BModify));

        private void ApplyExecute()
        {
            BModify = false;
            SQLStatusMsg = "Saving this SQL Connection!";

            if (ScanRate > 0 & ScanRate < 601) { ClassCommon.ScanSec = ScanRate; }

            SaveSetting();
            SaveAllSetting();
            ClsSerilog.LogMessage(ClsSerilog.INFO, $"Saved - New Settings");

            _eventAggregator.GetEvent<SettingsChangedEvents>().Publish(true);

            _eventAggregator.GetEvent<RestartAppEvents>().Publish(true);

            if (LocalHost == Host) LocalChecked = true;
            else RemoteChecked = true;


        }

        private DelegateCommand _testComand;
        public DelegateCommand TestCommand =>
       _testComand ?? (_testComand = new DelegateCommand(TestExecute).ObservesCanExecute(() => SearchDone).ObservesCanExecute(() => BModify));
        private void TestExecute()
        {
            BTesting = _sqlhandler.TestSqlConnection(Host, Instance, Database, Userid, Password);

            if(BTesting) SQLStatusMsg = "Connection Good";
            else SQLStatusMsg = "No Connection";
        }


        private DelegateCommand _showSQLHelpCommand;
        public DelegateCommand ShowSQLHelpCommand =>
       _showSQLHelpCommand ?? (_showSQLHelpCommand = new DelegateCommand(ShowSQLHelpExecute));

        private void ShowSQLHelpExecute()
        {
            
        }



        private void 
            SaveSetting()
        {

            if (_sqlhandler.LocalHost == Host)
            {
                _sqlhandler.BLocal = true;
                _sqlhandler.BSerRemote = false;
                BLocal = true;
                BRemote = false;
            }
            else
            {
                _sqlhandler.BLocal = false;
                _sqlhandler.BSerRemote = true;
                BLocal = false;
                BRemote = true;
            }

            _sqlhandler.Host = Host;
            _sqlhandler.SqlInstance = Instance;
            _sqlhandler.Database = Database;
            _sqlhandler.UserName = Userid;
            _sqlhandler.Password = Password;
            _sqlhandler.CbServerSelect = SelectedServerCombo;

            SQLStatusMsg = "This SQL Connection Saved!";
            ClsSerilog.LogMessage(ClsSerilog.INFO, $"Set - SQL Server to => {Host}");
        }


        private void SaveAllSetting()
        {
            //modules
            ClassCommon.RealTimeCheck = true;
            ClassCommon.GraphCheck = GraphCheck;
            ClassCommon.CombineCheck = CombineCheck;
            ClassCommon.ArchiveCheck = ArchiveCheck;
            ClassCommon.DualGraphCheck = DualGraphCheck;

            if (ClassCommon.WLOptions)
            {
                ClassCommon.WetLayerCheck = WetLayerCheck;
                ClassCommon.WLTrendCheck = WLTrendCheck;
            }
            else
            {
                ClassCommon.WetLayerCheck = false;
                ClassCommon.WLTrendCheck = false;
            }
               

            if (ClassCommon.DropOption == true)
            {
                ClassCommon.DropHitoLow = DropHitoLow;
                ClassCommon.DpProfileChecked = DpProfileChecked;
                ClassCommon.DpPostionChecked = DpPostionChecked;
            }
            else
            {
                ClassCommon.DropHitoLow = false;
                ClassCommon.DpProfileChecked = false;
                ClassCommon.DpPostionChecked = false;
            }

            //Units
            if (MCChecked)
            {
                ClassCommon.MoistureType = 0;
            }
            if (MRChecked)
            {
                ClassCommon.MoistureType = 1;
            }
            if (ADChecked)
            {
                ClassCommon.MoistureType = 2;
            }
            if (BDChecked)
            {
                ClassCommon.MoistureType = 3;
            }
            if (KGChecked)
            {
                ClassCommon.WeightUnit = 0;
            }
            if (LBChecked)
            {
                ClassCommon.WeightUnit = 1;
            }
        }

        private async void SearchSqlServers()
        {
            LoadingWindow tempWindow = new LoadingWindow();
            tempWindow.Show();

            try
            {
                DataTable dt = await _sqlhandler.GetServers();

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["InstanceName"].ToString() == "")
                            ServercomboList.Add(row["ServerName"].ToString());
                        else
                            ServercomboList.Add(row["ServerName"].ToString() + @"\" + row["InstanceName"].ToString());
                    }

                    SearchDone = true;
                    SQLStatusMsg = "Search Done!";
                    SelectedServerCombo = 1;
                }
                dt = null;
            }
            catch (Exception ex)
            {
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in SearchSQLServer <> {ex.Message}");
            }
            tempWindow.Close();
            if (tempWindow != null) tempWindow = null;
        }
    }
}
