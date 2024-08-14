using AppServices;
using CSVReports.Views;
using ModWetLayer.Properties;
using ModWetLayer.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.DataVisualization;
using System.Windows.Media;
using UcGraph.Views;
using static AppServices.ClassApplicationService;


namespace ModWetLayer.ViewModels
{
    public class WetLayerViewModel : BindableBase
    {

        public static string ModuleName = "Wet Layer Archive and Realtime";

        private readonly ClassSqlHandler _sqlhandler;
        protected readonly IEventAggregator _eventAggregator;

        private Task _timerTask;
        private PeriodicTimer _timer;
        private CancellationTokenSource _cts;

        private bool _pageloaded = false;

        string ChartTitle =  "WetLayer Data";
        string YTitle = string.Empty;   

        private double[] DataX;
        private double[] DataY;

        private DateTime PreBaleReadtime = DateTime.Now;
        private DateTime CurBaleReadtime = DateTime.Now;

        private bool _MonthListEnable;
        public bool MonthListEnable
        {
            get { return _MonthListEnable; }
            set { SetProperty(ref _MonthListEnable, value); }
        }

        private List<string> _wlmonthTableList;
        public List<string> WLMonthTableList
        {
            get { return _wlmonthTableList; }
            set { SetProperty(ref _wlmonthTableList, value); }
        }


        private bool _dayCheck;
        public bool DayCheck
        {
            get { return _dayCheck; }
            set
            {
                if (value == false) SelectOCRIndex = 0;
                SetProperty(ref _dayCheck, value);
            }
        }

        private string _selectTableValue;
        public string SelectTableValue
        {
            get { return _selectTableValue; }
            set
            {
                SetProperty(ref _selectTableValue, value);
                StrFileName = value;
            }
        }

        private int _selecttableindex;
        public int SelectTableIndex
        {
            get { return _selecttableindex; }
            set { SetProperty(ref _selecttableindex, value); }
        }

        private string _selectWLmonth;
        public string SelectWLmonth
        {
            get { return _selectWLmonth; }
            set { SetProperty(ref _selectWLmonth, value); }
        }

        private int _selectOCRindex;
        public int SelectOCRIndex
        {
            get { return _selectOCRindex; }
            set { SetProperty(ref _selectOCRindex, value); }
        }

        private double _opac = 1.0;
        public double Opac
        {
            get { return _opac; }
            set { SetProperty(ref _opac, value); }
        }

        private int _btnZidxRt;
        public int BtnZidxRt
        {
            get { return _btnZidxRt; }
            set { SetProperty(ref _btnZidxRt, value); }
        }

        private double _rtopac = 1.0;
        public double RTOpac
        {
            get { return _rtopac; }
            set { SetProperty(ref _rtopac, value); }
        }

        //Balers////////////////////////////////////////////////
        private bool _BalerCheck;
        public bool BalerCheck
        {
            get { return _BalerCheck; }
            set
            {
                SetProperty(ref _BalerCheck, value);
                if (value)
                {
                    SelectBalerIndex = 0;
                }
                else
                    SelectBalerValue = "ALL";
            }
        }
        private List<string> _BalerList;
        public List<string> BalerList
        {
            get { return _BalerList; }
            set { SetProperty(ref _BalerList, value); }
        }
        private string _SelectBalerValue;
        public string SelectBalerValue
        {
            get { return _SelectBalerValue; }
            set { SetProperty(ref _SelectBalerValue, value); }
        }
        private int _SelectBalerIndex;
        public int SelectBalerIndex
        {
            get { return _SelectBalerIndex; }
            set { SetProperty(ref _SelectBalerIndex, value); }
        }

        private bool _bModify = false;
        public bool BModify
        {
            get { return _bModify; }
            set { SetProperty(ref _bModify, value); }
        }
        private double _MinLimit;
        public double MinLimit
        {
            get { return _MinLimit; }
            set { SetProperty(ref _MinLimit, value); }
        }
        private double _MaxLimit;
        public double MaxLimit
        {
            get { return _MaxLimit; }
            set { SetProperty(ref _MaxLimit, value); }
        }
        private string _NormLimit;
        public string NormLimit
        {
            get { return _NormLimit; }
            set { SetProperty(ref _NormLimit, value); }
        }

        private double _MinYAxis;
        public double MinYAxis
        {
            get { return _MinYAxis; }
            set { SetProperty(ref _MinYAxis, value); }
        }
        private double _MaxYAxis;
        public double MaxYAxis
        {
            get { return _MaxYAxis; }
            set { SetProperty(ref _MaxYAxis, value); }
        }
        


        private List<string> _ColorList;
        public List<string> ColorList
        {
            get { return _ColorList; }
            set { SetProperty(ref _ColorList, value); }
        }

        private List<Brush> BrushList;


        private Brush _GraphHiColor;
        public Brush GraphHiColor
        {
            get { return _GraphHiColor; }
            set { SetProperty(ref _GraphHiColor, value); }
        }

        private Brush _GraphNormColor;
        public Brush GraphNormColor
        {
            get { return _GraphNormColor; }
            set { SetProperty(ref _GraphNormColor, value); }
        }

        private Brush _GraphLowColor;
        public Brush GraphLowColor
        {
            get { return _GraphLowColor; }
            set { SetProperty(ref _GraphLowColor, value); }
        }

        private Brush _AlarmColor;
        public Brush AlarmColor
        {
            get { return _AlarmColor; }
            set { SetProperty(ref _AlarmColor, value); }
        }

        private string _AlarmMsg;
        public string AlarmMsg
        {
            get { return _AlarmMsg; }
            set { SetProperty(ref _AlarmMsg, value); }
        }

        private bool _bDataOnScreen = false;
        public bool BDataOnScreen
        {
            get { return _bDataOnScreen; }
            set { SetProperty(ref _bDataOnScreen, value); }
        }

        private bool _bWLDataReady = false;
        public bool BWLDataReady
        {
            get { return _bWLDataReady; }
            set { SetProperty(ref _bWLDataReady, value); }
        }


        private int _SampleBales = 20;
        public int RealTimeSamples
        {
            get { return _SampleBales; }
            set { SetProperty(ref _SampleBales, value); }
        }

        private List<string> _DayEndList;
        public List<string> DayEndList
        {
            get
            {
                _DayEndList = new List<string>();
                DateTime date = new DateTime();

                var result = Enumerable.Repeat(date, 24)
                                       .Select((x, i) => x.AddHours(i).ToString("HH:MM"));

                var hours2 = Enumerable.Range(00, 24).Select(i => (DateTime.MinValue.AddHours(i)).ToString("hh.mm"));

                var hours = from i in Enumerable.Range(0, 24)
                            let h = new DateTime(2019, 1, 1, i, 0, 0)
                            select h.ToString("t", CultureInfo.InvariantCulture);

                foreach (var item in hours)
                {
                    _DayEndList.Add(item.ToString());
                }
                return _DayEndList;
            }
        }


        private string _SelectDayEndTimeVal;
        public string SelectDayEndTimeVal
        {
            get { return Settings.Default.WLDayEndTime; }
            set
            {
                if (value != string.Empty)
                {
                    Settings.Default.WLDayEndTime = value;
                    Settings.Default.Save();
                }
                SetProperty(ref _SelectDayEndTimeVal, value);
            }
        }

        public string DayStartstr { get; set; }


        /// <summary>
        /// ////////////////////////////////////////////////////////
        /// </summary>
        private int _SelectHighIndex;
        public int SelectHighIndex
        {
            get { return _SelectHighIndex; }
            set { SetProperty(ref _SelectHighIndex, value); }
        }
        private int _SelectNormIndex;
        public int SelectNormIndex
        {
            get { return _SelectNormIndex; }
            set { SetProperty(ref _SelectNormIndex, value); }
        }
        private int _SelectLoIndex;
        public int SelectLoIndex
        {
            get { return _SelectLoIndex; }
            set { SetProperty(ref _SelectLoIndex, value); }
        }
        private int _SelectAlarmIndex;
        public int SelectAlarmIndex
        {
            get { return _SelectAlarmIndex; }
            set { SetProperty(ref _SelectAlarmIndex, value); }
        }
        private int _SelectOKIndex;
        public int SelectOKIndex
        {
            get { return _SelectOKIndex; }
            set { SetProperty(ref _SelectOKIndex, value); }
        }

        private bool _bTabOne = false;
        public bool BTabOne
        {
            get { return _bTabOne; }
            set { SetProperty(ref _bTabOne, value); }
        }
        private bool _bTabTwo = false;
        public bool BTabTwo
        {
            get { return _bTabTwo; }
            set { SetProperty(ref _bTabTwo, value); }
        }
        private bool _bTabThree = false;
        public bool BTabThree
        {
            get { return _bTabThree; }
            set { SetProperty(ref _bTabThree, value); }
        }


        private Nullable<DateTime> _startQueryDate = null;
        public Nullable<DateTime> StartQueryDate
        {
            get
            {
                if (_startQueryDate == null)
                    _startQueryDate = DateTime.Today;
                return _startQueryDate;
            }
            set { SetProperty(ref _startQueryDate, value); }
        }

        private Nullable<DateTime> _endQueryDate = null;
        public Nullable<DateTime> EndQueryDate
        {
            get
            {
                if (_endQueryDate == null)
                    _endQueryDate = DateTime.Today;

                if (_endQueryDate < _startQueryDate)
                    _endQueryDate = _startQueryDate;

                return _endQueryDate;
            }
            set { SetProperty(ref _endQueryDate, value); }
        }


        private string _DayEnd;
        public string DayEnd
        {
            get { return _DayEnd; }
            set { SetProperty(ref _DayEnd, value); }
        }
        ////////////////////////////////////////////////////////


        private bool _realtimechecked;
        public bool RealTimeChecked
        {
            get { return _realtimechecked; }
            set
            {
                if (value)
                {
                    MonthListEnable = false;
                    DayCheck = false;
                    Opac = 0.0;
                    RTOpac = 1;
                    BalerCheck = false;
                    BtnZidxRt = 99;
                }
                else
                {
                    PreBaleReadtime = DateTime.Now;
                    MonthListEnable = true;
                    Opac = 1;
                    RTOpac = 0.0;
                    BtnZidxRt = 0;
                }
                SetProperty(ref _realtimechecked, value);
            }
        }


        public bool _MonthChecked;
        public bool MonthChecked
        {
            get { return _MonthChecked; }
            set
            {
                SetProperty(ref _MonthChecked, value);
                if (value)
                {
                    PreBaleReadtime = DateTime.Now;
                    MonthListEnable = true;
                    Opac = 1;
                    RTOpac = 0.0;
                }
                else
                {
                    Opac = 0;
                }
            }
        }

        #region CSV 

        private string _strFileLocation;
        public string StrFileLocation
        {
            get { return _strFileLocation; }
            set
            {
                SetProperty(ref _strFileLocation, value);
                Settings.Default.CsvFileLocation = value;
                Settings.Default.Save();
            }
        }

        private string _StrFileName;
        public string StrFileName
        {
            get { return _StrFileName; }
            set { SetProperty(ref _StrFileName, value); }
        }

        private string _strPathFile;
        public string StrPathFile
        {
            get { return _strPathFile; }
            set { SetProperty(ref _strPathFile, value); }
        }

        private string _CSVTextMsg;
        public string CSVTextMsg
        {
            get { return _CSVTextMsg; }
            set { SetProperty(ref _CSVTextMsg, value); }
        }

        private string _CVScanInterval = "40";
        public string CVScanInterval
        {
            get { return _CVScanInterval; }
            set { SetProperty(ref _CVScanInterval, value); }
        }

        #endregion CSV


        private double _showme = .1;
        public double ShowMe
        {
            get { return _showme; }
            set { SetProperty(ref _showme, value); }
        }

      

        #region Graph Colors

        public Brush WLMinLimitColor { get; set; }
        public Brush WLMaxLimitColor { get; set; }
        public Brush WLNorLimitColor { get; set; }
        public Brush WLAlarmColor { get; set; }
        public Brush WLOKColor { get; set; }

        readonly int DefaultCount = 16;
        int iLayerCount = 0;

        bool bOutofLimitAlarmOn = false;
        private string StrCurrMonth;

        #endregion Graph Colors

        //GraphAutoScale

        private bool _graphAutoScale = Settings.Default.GraphAutoScale;
        public bool GraphAutoScale
        {
            get { return _graphAutoScale; }
            set 
            { 
                SetProperty(ref _graphAutoScale, value);
                Settings.Default.GraphAutoScale = value? true:false;
                Settings.Default.Save();
            }
        }

        public struct CALC_RESULTS
        {
            public long BaleID;
            public int iBalerID;
            public string strBaler; //*10
            public double dDeviation;
            public double dAverage;
            public double dMaxValue;
            public double dMinValue;
            public int iNumbOfSpots;
            public string strResult; //*10
            public int[] iVals;
            public int iSize;
            public double[] dCalcResults;
            public List<double> dLayers;
            public int iLayers;
            public double dMoisture;
            public bool bAlarm;
            public bool bTCStampsAssigned;
        };

        string strLayer;


        private List<string> _occrlist = new List<string> {"Latest", "Oldest", "All" };
        public List<string> Occrlist
        {
            get { return _occrlist; }
            set { SetProperty(ref _occrlist, value); }
        }

        private int _selectOccr = 0;
        public int SelectOccr
        {
            get => _selectOccr; 
            set 
            {
                if(value > -1)
                     SetProperty(ref _selectOccr, value); 
            }
        }

        private string _eventValue;
        public string EventValue
        {
            get { return _eventValue; }
            set
            {
                if (value == "All")
                    QuanEnable = false;
                else
                    QuanEnable = true;
                SetProperty(ref _eventValue, value);
            }
        }

        private bool _quanEnable;
        public bool QuanEnable
        {
            get { return _quanEnable; }
            set { SetProperty(ref _quanEnable, value); }
        }

        private int _recCount = 200;
        public int RecCount
        {
            get { return _recCount; }
            set { SetProperty(ref _recCount, value); }
        }

        private bool _rtIdle = true;
        public bool RTIdle
        {
            get { return _rtIdle; }
            set { SetProperty(ref _rtIdle, value); }
        }

        private bool _rtrunning = false;
        public bool RTRunning
        {
            get { return _rtrunning; }
            set
            {
                RTIdle = !value;
                SetProperty(ref _rtrunning, value);
            }
        }

        private bool _rptrunning = false;
        public bool RPTRunning
        {
            get { return _rptrunning; }
            set
            {
                RTIdle = !value;
                RPTIdle = !value;
                //_eventAggregator.GetEvent<UpdatedEvent>().Publish(value);
                //_eventAggregator.GetEvent<UpdatedWLEvent>().Publish(value);
                SetProperty(ref _rptrunning, value);
            }
        }

        private bool _rptIdle = true;
        public bool RPTIdle
        {
            get { return _rptIdle; }
            set { SetProperty(ref _rptIdle, value); }
        }

        List<Tuple<long, string, double>> MyCVListX { get; set; }

        private DataTable _wetlayerDataTable;
        public DataTable WetLayerDataTable
        {
            get { return _wetlayerDataTable; }
            set { SetProperty(ref _wetlayerDataTable, value); }
        }

      

        public WetLayerViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            MonthChecked = true;
            BModify = false;


            if (_sqlhandler == null)
            {
                _sqlhandler = ClassSqlHandler.Instance;

                WLMonthTableList = _sqlhandler.GettableList(ClassCommon.WET_LAYER);
                SelectTableIndex = 0;

                BalerList = _sqlhandler.GetUniqueStrItemlist("BalerID", WLMonthTableList[0]);
                if (BalerList.Count > 1)
                {
                    BalerList.Add("ALL");
                    SelectBalerValue = "ALL";
                }
                BalerCheck = false;

                SelectTableIndex = 0;
                SelectOCRIndex = 0;
                MonthChecked = true;
                BModify = false;

                SelectOccr = 1;
                EventValue = "Latest";

                GetColorandBrushLists();
                LoadGraphSettingtab();
                BDataOnScreen = false;
                //TxtStatus = " Page Loaded";

                MinYAxis = Settings.Default.WLMinYAxis;
                MaxYAxis = Settings.Default.WLMaxYAxis;

                StrFileLocation = Settings.Default.CsvFileLocation;


                //SetupAppTitle("Forté Wetlayer From  -> " + _sqlhandler.Host + @"\" + _sqlhandler.SqlInstance);

            }
        }

        private void SetupAppTitle(string strTitle)
        {
            ChartTitle = strTitle;
        }

        private int _selectWlData;
        public int SelectWlData
        {
            get => _selectWlData; 
            set
            {
                SetProperty(ref _selectWlData, value);
                if (value > -1)
                {  
                    DrawWetLayerChart(value);
                   
                }
            }
        }

        private DelegateCommand _QueryCommand;
        public DelegateCommand QueryCommand =>
            _QueryCommand ?? (_QueryCommand = new DelegateCommand(QueryExecute).ObservesCanExecute(() => MonthListEnable).ObservesCanExecute(() => BTabOne));
        private void QueryExecute()
        {
            GetArchiveWLData(SelectTableValue);
            SelectWlData = 0;
        }

        

        private DelegateCommand _WriteCSVCommand;
        public DelegateCommand WriteCSVCommand =>
            _WriteCSVCommand ?? (_WriteCSVCommand = new DelegateCommand(WriteCsvExecute, WriteCsvCanExecute)
            .ObservesProperty(() => BDataOnScreen).ObservesProperty(() => MonthChecked));

        private bool WriteCsvCanExecute()
        {
            return BDataOnScreen && MonthChecked;
        }

        private void WriteCsvExecute()
        {
            int iStart = 9999;
            int iEnd = WetLayerDataTable.Rows.Count;

            try
            {
                if(WetLayerDataTable.Rows.Count > 0)
                {
                    using (CSVReport csvDlg = new CSVReport(_eventAggregator))
                    {
                        csvDlg.InitCsv(WetLayerDataTable, SelectTableValue, iStart, iEnd);
                        csvDlg.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in WriteCsvExecute {ex.Message} ");
            }
        }


        private DelegateCommand _StartCommand;
        public DelegateCommand StartCommand =>
            _StartCommand ?? (_StartCommand = new DelegateCommand(StartExecute).ObservesCanExecute(() => RealTimeChecked).ObservesCanExecute(() => RTIdle));
        private void StartExecute()
        {
            RTRunning = true;

            _eventAggregator.GetEvent<UpdateAppRunEvents>().Publish(true);

            InitializeTimer();
            StartTimer();
        }

       

        private DelegateCommand _StopCommand;
        public DelegateCommand StopCommand =>
            _StopCommand ?? (_StopCommand = new DelegateCommand(StopExecute).ObservesCanExecute(() => RTRunning));
        private void StopExecute()
        {
            RTRunning = false;

            StopTimer();
            _eventAggregator.GetEvent<UpdateAppRunEvents>().Publish(false);
        }


        private DelegateCommand _ModifyCommand;
        public DelegateCommand ModifyCommand =>
            _ModifyCommand ?? (_ModifyCommand = new DelegateCommand(ModifyExecute));
        private void ModifyExecute()
        {
            BModify = true;
        }

        private DelegateCommand _CancelCommand;
        public DelegateCommand CancelCommand =>
            _CancelCommand ?? (_CancelCommand = new DelegateCommand(CancelCanExecute).ObservesCanExecute(() => BModify));
        private void CancelCanExecute()
        {
            BModify = false;
        }

        private DelegateCommand _ApplyCommand;
        public DelegateCommand ApplyCommand =>
            _ApplyCommand ?? (_ApplyCommand = new DelegateCommand(ApplyExecute).ObservesCanExecute(() => BModify));
        private void ApplyExecute()
        {

            Settings.Default.WLMinLimit = MinLimit;
            Settings.Default.WLMaxLimit = MaxLimit;

            Settings.Default.WLMinYAxis = MinYAxis;
            Settings.Default.WLMaxYAxis = MaxYAxis;

            Settings.Default.Save();

            SaveGraphSettings();

            BModify = false;
        }

        private DelegateCommand _ShowGraphCommand;
        public DelegateCommand ShowGraphCommand =>
            _ShowGraphCommand ?? (_ShowGraphCommand = new DelegateCommand(ShowGraphExecute).ObservesCanExecute(() => BDataOnScreen).ObservesCanExecute(() => MonthChecked));
        private void ShowGraphExecute()
        {
            double[] timeX =  new double[1];
            double[] dataY =  new double[1];
            string chartTitle = "Coefficient of Variation (CV) Chart";

            UCGraph2View ShowGrapgView = new UCGraph2View(WetLayerDataTable, chartTitle) //StrItem
            {
                Height = 800,
                Width = 1000
            };
            ShowGrapgView.ShowDialog();
        }

        private void ShowGraph()
        {
            throw new NotImplementedException();
        }

        private DelegateCommand _CSVTestCommand;
        public DelegateCommand CSVTestCommand =>
            _CSVTestCommand ?? (_CSVTestCommand = new DelegateCommand(CsvTestExecute).ObservesCanExecute(() => BWLDataReady).ObservesCanExecute(() => RPTRunning));
        private void CsvTestExecute()
        {
            
        }

        private DelegateCommand _BrowseCommand;
        public DelegateCommand BrowseCommand =>
            _BrowseCommand ?? (_BrowseCommand = new DelegateCommand(BrowseExecute).ObservesCanExecute(() => RPTRunning));
        private void BrowseExecute()
        {
           
        }


        private void GetArchiveWLData(string selectTableValue)
        {

            string strStartTime = "00:00";
            string strEndTime = "23:59";

            string strStartDate = _startQueryDate.Value.Date.ToString("MM/dd/yyyy") + " " + strStartTime; // Settings.Default.WLDayEndTime;
            string strEndDate = _endQueryDate.Value.Date.ToString("MM/dd/yyyy") + " " + strEndTime; // Settings.Default.WLDayEndTime;

            //string strBase = string.Empty;

            string strWlQuery;
            string strBaleID = string.Empty;
            string strone = string.Empty;
            string strQueryOccr;
            string Timeline = string.Empty;


            switch (EventValue)
            {
                case "All":
                    strone = "SELECT * From ";
                    Timeline = "DESC;";
                    break;
                case "Latest":
                    strone = "SELECT TOP " + RecCount + " * From ";
                    Timeline = "DESC;";
                    break;
                case "Oldest":
                    strone = "SELECT TOP " + RecCount + " * From ";
                    Timeline = "ASC;";
                    break;
            }

            if (BalerCheck)
            {
                strBaleID = "  WHERE BalerID = '" + SelectBalerValue + "'";
            }


            if (DayCheck)
            {
                strQueryOccr = " WHERE CAST(ReadTime AS DATETIME) BETWEEN '" + strStartDate + "' and '" + strEndDate + "'  ORDER BY ReadTime " + Timeline;
                if (BalerCheck)
                    strQueryOccr = " AND CAST(ReadTime AS DATETIME) BETWEEN '" + strStartDate + "' and '" + strEndDate + "'  ORDER BY ReadTime " + Timeline;
            }
            else
                strQueryOccr = " ORDER BY ReadTime " + Timeline;

            strWlQuery = strone + selectTableValue + " with (NOLOCK) " + strBaleID + " " + strQueryOccr;

            try
            {
                using (DataTable Mytable = (DataTable)_sqlhandler.GetWetLayerDataTable(selectTableValue, strWlQuery))
                {

                    if (Mytable.Rows.Count > 0)
                    {
                        CurBaleReadtime = Convert.ToDateTime(Mytable.Rows[0]["ReadTime"].ToString());
                        if ((CurBaleReadtime != PreBaleReadtime) || MonthChecked == true)
                        {
                            ProccessData(Mytable);
                          // TxtStatus = " Number of Bales =  " + Mytable.Rows.Count;
                        }
                    }
                   // else
                       // TxtStatus = "No Bale Data to show in the DataGrid";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in GetArchiveWLData {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GetArchiveWLData < {ex.Message}");
            }
        }

        private void ProccessData(DataTable mytable)
        {
            List<double> fLayerVal;

            try
            {
                if(mytable.Rows.Count > 0)
                {

                    MyCVListX = new List<Tuple<long, string, double>>();
                    BDataOnScreen = true;

                    if (string.IsNullOrEmpty(mytable.Rows[0]["Layers"].ToString()))
                        iLayerCount = DefaultCount;
                    else
                        iLayerCount = mytable.Rows[0].Field<int>("Layers");

                    WetLayerDataTable = new DataTable();

                    //Added for graph title
                    mytable.Columns.Add("Title", typeof(string));
                    mytable.AcceptChanges();

                   // SetupAppTitle("Forté Wetlayer From  -> " + _sqlhandler.Host + @"\" + _sqlhandler.SqlInstance);

                    foreach (DataRow dtRow in mytable.Rows)
                    {
                        CALC_RESULTS StructLast = new CALC_RESULTS();
                        fLayerVal = new List<double>();

                        for (int i = 1; i < iLayerCount + 1; i++)
                        {
                            strLayer = "Layer" + i.ToString();
                            if (!string.IsNullOrEmpty(dtRow[strLayer].ToString()) | (dtRow[strLayer].GetType().Name == "Double"))
                            {
                                if (ClassCommon.MoistureType == 0)
                                    fLayerVal.Add(dtRow.Field<Double>(strLayer));
                                if (ClassCommon.MoistureType == 1)
                                    fLayerVal.Add(ConvToMR(dtRow.Field<Double>(strLayer)));
                            }
                        }

                        if (fLayerVal.Count > 0)
                        {
                            CalCVMinMax(fLayerVal, iLayerCount, out StructLast);

                            if (!string.IsNullOrEmpty(dtRow["Moisture"].ToString()) | (dtRow["Moisture"].GetType().Name == "Double"))
                            {
                                //Settings.Default.MoistureUnit

                                if (ClassCommon.MoistureType == 0)
                                    dtRow["Moisture"] = dtRow.Field<Double>("Moisture").ToString("#0.00");
                                else if (ClassCommon.MoistureType == 1)
                                    dtRow["Moisture"] = ConvToMR(dtRow.Field<Double>("Moisture")).ToString("#0.00");
                            }
                            dtRow["Deviation"] = StructLast.dDeviation.ToString("#0.00");
                            dtRow["Param1"] = StructLast.dMaxValue.ToString("#0.00");
                            dtRow["Param2"] = StructLast.dMinValue.ToString("#0.00");

                            //For CV Graph data list of Tuple<int, string, double>
                            MyCVListX.Add(new Tuple<long, string, double>(dtRow.Field<long>("ID"),
                                dtRow.Field<DateTime>("ReadTime").ToString(), dtRow.Field<Double>("Deviation")));

                            for (int i = 1; i < iLayerCount + 1; i++)
                            {
                                strLayer = "Layer" + i.ToString();
                                dtRow[strLayer] = StructLast.dLayers[i - 1].ToString("#0.00");
                            }
                            //Graph Title
                            dtRow["Title"] = dtRow["ReadTime"] + " - Baler " + dtRow["BalerID"] + ", Number - " + dtRow["ID"];
                        }
                    }

                    //Removed Un wanted items.
                    if (mytable.Columns.Contains("Time1"))
                        mytable.Columns.Remove("Time1");
                    if (mytable.Columns.Contains("Time2"))
                        mytable.Columns.Remove("Time2");
                    if (mytable.Columns.Contains("Time3"))
                        mytable.Columns.Remove("Time3");
                    if (mytable.Columns.Contains("MaximumPrc"))
                        mytable.Columns.Remove("MaximumPrc");
                    if (mytable.Columns.Contains("Layer17"))
                        mytable.Columns.Remove("Layer17");
                    if (mytable.Columns.Contains("Layer18"))
                        mytable.Columns.Remove("Layer18");
                    if (mytable.Columns.Contains("Layer19"))
                        mytable.Columns.Remove("Layer19");
                    if (mytable.Columns.Contains("Layer20"))
                        mytable.Columns.Remove("Layer20");
                    if (mytable.Columns.Contains("Layer21"))
                        mytable.Columns.Remove("Layer21");
                    if (mytable.Columns.Contains("Layer22"))
                        mytable.Columns.Remove("Layer22");
                    if (mytable.Columns.Contains("Layer23"))
                        mytable.Columns.Remove("Layer23");
                    if (mytable.Columns.Contains("Layer24"))
                        mytable.Columns.Remove("Layer24");
                    if (mytable.Columns.Contains("Layer25"))
                        mytable.Columns.Remove("Layer25");
                    if (mytable.Columns.Contains("Layer26"))
                        mytable.Columns.Remove("Layer26");
                    if (mytable.Columns.Contains("Layer27"))
                        mytable.Columns.Remove("Layer27");
                    if (mytable.Columns.Contains("Layer28"))
                        mytable.Columns.Remove("Layer28");
                    if (mytable.Columns.Contains("Layer29"))
                        mytable.Columns.Remove("Layer29");
                    if (mytable.Columns.Contains("Layer30"))
                        mytable.Columns.Remove("Layer30");
                    if (mytable.Columns.Contains("Layers"))
                        mytable.Columns.Remove("Layers");

                    if (mytable.Columns.Contains("Title"))
                        mytable.Columns.Remove("Title");

                    mytable.AcceptChanges();

                    WetLayerDataTable = mytable;

                    DrawWetLayerChart(0);
                    SelectWlData = 0;

                    _eventAggregator.GetEvent<UpdateRealTimeEvents>().Publish(DateTime.Now);

                    if (RealTimeChecked)
                        PreBaleReadtime = CurBaleReadtime;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in ProccessData {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in ProcessData WL < {ex.Message}");
            }
        }

        /// <summary>
        /// Draw Bar Graph here!
        /// </summary>
        /// <param name="selectedrow"></param>
        private void DrawWetLayerChart(int selectedrow)
        {


            double[] timeX = new double[16] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16};
            double[] valueY = new double[16];


            if(WetLayerDataTable.Rows.Count > 0) 
            {
                if (ClassCommon.MoistureType == 0)
                {
                    YTitle = "Moisture Content %";
                }
                else if (ClassCommon.MoistureType == 1)
                {
                    YTitle = "Moisture Regain %";
                }


                for (int x = 1; x < iLayerCount + 1; x++)
                {

                    if ((WetLayerDataTable.Rows[selectedrow]["Layer" + x].ToString() != string.Empty) & (WetLayerDataTable != null))
                    {
                        double iDouble = WetLayerDataTable.Rows[selectedrow].Field<double>("Layer" + x);
                        valueY[x - 1] = iDouble;
                    }
                }

                SetupAppTitle($"Forté Wetlayer Bale ID = {WetLayerDataTable.Rows[selectedrow]["ID"]}");

                MinYAxis = Settings.Default.WLMinYAxis;
                MaxYAxis = Settings.Default.WLMaxYAxis;

                WetLayerView._wetLayerView?.PlotChart(timeX, valueY, ChartTitle, YTitle);
            }

        }

        private double ConvToMR(double fMoisture)
        {
            return fMoisture / (1 - fMoisture / 100);
        }


        private void CalCVMinMax(List<double> SampleList, int iLayers, out CALC_RESULTS tResults)
        {
            tResults = new CALC_RESULTS();
            double sumOfDerivation = 0;

            //Average
            tResults.dAverage = SampleList.Average();

            //Min Max
            tResults.dMinValue = SampleList.Min();
            tResults.dMaxValue = SampleList.Max();

            //layers
            tResults.dLayers = new List<Double>();
            tResults.dLayers = SampleList;

            //Deviation
            tResults.bAlarm = false;
            foreach (var value in SampleList)
            {
                sumOfDerivation += (value - tResults.dAverage) * (value - tResults.dAverage);
            }

            //STD
            double Variance = sumOfDerivation / (SampleList.Count - 1);
            double StandardDeviation = Math.Sqrt(Variance);

            //%CV (Coefficient of Variation = Standard Deviation / Mean)
            tResults.dDeviation = StandardDeviation / tResults.dAverage * 100;
            tResults.bAlarm = false;

        }


        private void LoadGraphSettingtab()
        {
            MinLimit = Settings.Default.WLMinLimit;
            MaxLimit = Settings.Default.WLMaxLimit;

            MinYAxis = Settings.Default.WLMinYAxis;
            MaxYAxis = Settings.Default.WLMaxYAxis;

         

            SelectHighIndex = Settings.Default.WLHiColorIndex;
            SelectNormIndex = Settings.Default.WLNormColorIndex;
            SelectLoIndex = Settings.Default.WLLoColorIndex;
            SelectAlarmIndex = Settings.Default.WLAlarmIndex;
            SelectOKIndex = Settings.Default.WLOkIndex;

            NormLimit = MinLimit.ToString() + " to " + MaxLimit.ToString();

        }

        private void SaveGraphSettings()
        {
            Settings.Default.WLMinLimit = MinLimit;
            Settings.Default.WLMaxLimit = MaxLimit;

            Settings.Default.WLMinYAxis = MinYAxis;
            Settings.Default.WLMaxYAxis = MaxYAxis;

           

            Settings.Default.WLHiColorIndex = SelectHighIndex;
            Settings.Default.WLNormColorIndex = SelectNormIndex;
            Settings.Default.WLLoColorIndex = SelectLoIndex;
            Settings.Default.WLAlarmIndex = SelectAlarmIndex;
            Settings.Default.WLOkIndex = SelectOKIndex;

            NormLimit = MinLimit.ToString() + " to " + MaxLimit.ToString();

            Settings.Default.Save();

            GraphHiColor = BrushList[SelectHighIndex];
            GraphNormColor = BrushList[SelectNormIndex];
            GraphLowColor = BrushList[SelectLoIndex];
        }


        private void GetColorandBrushLists()
        {

            ColorList = new List<string>
            {
                "BlanchedAlmond",
                "Red",
                "Yellow",
                "Blue",
                "Green",
                "Brown",
                "Gray",
                "Puple",
                "Pink",
                "Orange",
                "Olive",
                "White",
                "Beige",
                "SlateGray",
                "SpringGreen",
                "LightGreen",
                "LightSteelBlue",
                "Salmon",
                "Azure",
                "Aqua",
                "Aquamarine"
            };

            BrushList = new List<Brush>
            {
                Brushes.BlanchedAlmond,
                Brushes.Red,
                Brushes.Yellow,
                Brushes.Blue,
                Brushes.Green,
                Brushes.Brown,
                Brushes.Gray,
                Brushes.Purple,
                Brushes.Pink,
                Brushes.Orange,
                Brushes.Olive,
                Brushes.White,
                Brushes.Beige,
                Brushes.SlateGray,
                Brushes.SpringGreen,
                Brushes.LightGreen,
                Brushes.LightSteelBlue,
                Brushes.Salmon,
                Brushes.Azure,
                Brushes.Aqua,
                Brushes.Aquamarine
            };

        }

        private void Heartbeat()
        {
            if (ShowMe == 0.1) ShowMe = 1;
            else if (ShowMe == 1) ShowMe = 0.1;
        }


        public void LoadPage()
        {
            _pageloaded = true;


            try
            {

               





            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in LoadPage {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.INFO, $"---------- Load Page WetLayer ERROR {ex.Message}");
            }
            ClsSerilog.LogMessage(ClsSerilog.INFO, $"---------- Load Page WetLayer");
        }



        #region DispatcherTimer /////////////////////////////////////////////////////////////////////////////////////

        private System.Windows.Threading.DispatcherTimer dispatcherTimer;

        /// <summary>
        /// System.Windows.Threading.DispatcherTimer
        /// </summary>
        private void InitializeTimer()
        {
            if (dispatcherTimer != null) dispatcherTimer = null;
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(ClassCommon.ScanSec)
            };
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();

            Application.Current.Dispatcher.Invoke(new Action(() => { GetArchiveWLData(_sqlhandler.GetCurrentWLTable()); }));
            Application.Current.Dispatcher.Invoke(new Action(() => { Heartbeat(); }));

            Thread.Sleep(1000); //Rest for 1 Sec.
            dispatcherTimer.Start();
        }
        private void StartTimer()
        {
            dispatcherTimer.Start();
        }

        private void StopTimer()
        {
            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
                ShowMe = 0.1;
                //Opac = 1.0;
            }
        }

        #endregion DispatcherTimer ////////////////////////////////////////////////////////////////////////////////////



    }
}
