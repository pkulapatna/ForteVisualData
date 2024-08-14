using AppServices;
using DataFieldsSelect.Views;
using GraphMenuBar.ViewModels;
using GraphMenuBar.Views;
using ModCombine.Properties;
using ModCombine.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static AppServices.ClassApplicationService;

namespace ModCombine.ViewModels
{
    public class CombineViewModel : BindableBase
    {

        double WCoef = 1.0;

        double[] timeX;
        double[] valueY;

        private long preIndex = 0;
        private long newIndex = 0;

        public static string ModuleName = "Realtime Graphic and Data";
        protected readonly IEventAggregator _eventAggregator = new EventAggregator();

        private readonly ClassSqlHandler _sqlhandler;

        private Task _timerTask;
        private PeriodicTimer _timer;
        private CancellationTokenSource _cts;

     
        public UserControl TopMenuOneBar
        {
           get => new MenuBarView(_eventAggregator); 
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _chartTitle;
        public string ChartTitle
        {
            get => _chartTitle;
            set => SetProperty(ref _chartTitle, value);
        }

        private DataTable _realtimeSumdatatable;
        public DataTable RealTimeSumDataTable
        {
            get => _realtimeSumdatatable;
            set => SetProperty(ref _realtimeSumdatatable, value); 
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


        //Lines
        private List<string> _LineList;
        public List<string> LineList
        {
            get => _LineList;
            set => SetProperty(ref _LineList, value);
        }

        private int _SelectLineIndex;
        public int SelectLineIndex
        {
            get { return _SelectLineIndex; }
            set
            {
                SetProperty(ref _SelectLineIndex, value);
            }
        }
        private string _selectedLineVal = Settings.Default.SelectedLineVal;
        public string SelectedLineVal
        {
            get { return _selectedLineVal; }
            set
            {
                SetProperty(ref _selectedLineVal, value);
                if (value != null)
                {
                    Settings.Default.SelectedLineVal = value;
                    Settings.Default.Save();
                }
            }
        }

        //Sources
        private List<string> _sourceList;
        public List<string> SourceList
        {
            get => _sourceList;
            set => SetProperty(ref _sourceList, value);
        }
        private int _selectSourceIndex;
        public int SelectSourceIndex
        {
            get { return _selectSourceIndex; }
            set
            {
                SetProperty(ref _selectSourceIndex, value);
            }
        }
        private string _selectedSourceVal = Settings.Default.SelectedSourceVal;
        public string SelectedSourceVal
        {
            get { return _selectedSourceVal; }
            set
            {
                SetProperty(ref _selectedSourceVal, value);
                if (value != null)
                {
                    Settings.Default.SelectedSourceVal = value;
                    Settings.Default.Save();
                }
            }
        }

        

        #region Graph HiLow limits

        private double _graphLimitHi;
        public double GraphLimitHi
        {
            get => _graphLimitHi;
            set
            {
                SetProperty(ref _graphLimitHi, value);

                if(value > -1) 
                {
                    if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                    {
                        Settings.Default.GraphOneLimitHi = value;
                        Settings.Default.Save();
                    }
                    else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                    {
                        Settings.Default.GraphTwoLimitHi = value;
                        Settings.Default.Save();
                    }
                }
            }
        }

        private double _graphLimitLo;
        public double GraphLimitLo
        {
            get => _graphLimitLo;
            set
            {
                SetProperty(ref _graphLimitLo, value);

                if (value > -1) 
                {
                    if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                    {
                        Settings.Default.GraphOneLimitLo = value;
                        Settings.Default.Save();
                    }
                    else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                    {
                        Settings.Default.GraphTwoLimitLo = value;
                        Settings.Default.Save();
                    }
                }
            }
        }



        #endregion Graph HiLow limits

     


        

        private string _curMoistureHdr = ClassCommon.MoistureTypeLst[ClassCommon.MoistureType];  //"Current Moisture";
        public string CurMoistureHdr
        {
            get { return _curMoistureHdr; }
            set { SetProperty(ref _curMoistureHdr, value); }
        }


        private string _curMoisture = string.Empty;
        public string CurMoisture
        {
            get { return _curMoisture; }
            set { SetProperty(ref _curMoisture, value); }
        }

        private string _curWeightHdr = ClassCommon.WeightTypeLst[ClassCommon.WeightUnit]; //"Current Bale Weight";
        public string CurWeightHdr
        {
            get { return _curWeightHdr; }
            set { SetProperty(ref _curWeightHdr, value); }
        }
        private string _curWeight = string.Empty;
        public string CurWeight
        {
            get { return _curWeight; }
            set { SetProperty(ref _curWeight, value); }
        }

        private string _spareBoxHdr = "Option";
        public string SpareBoxHdr
        {
            get { return _spareBoxHdr; }
            set { SetProperty(ref _spareBoxHdr, value); }
        }

        private string _spareBox = string.Empty;
        public string SpareBox
        {
            get { return _spareBox; }
            set { SetProperty(ref _spareBox, value); }
        }

        private int _summaryIdx;
        public int SummaryIdx
        {
            get { return _summaryIdx; }
            set 
            {
                
                SetProperty(ref _summaryIdx, value); 
            
                if(value > -1)
                {
                    CurMoisture = RealTimeSumDataTable.Rows[SummaryIdx].Field<Single>("Moisture").ToString("#0.00");
                    double CurrWt = RealTimeSumDataTable.Rows[SummaryIdx].Field<Single>("Weight");
                    CurWeight = (CurrWt * WCoef).ToString("#0.00");
                }
            }
        }

        private bool _CustFieldCheck;
        public bool CustFieldCheck
        {
            get => _CustFieldCheck;
            set
            {
                SetProperty(ref _CustFieldCheck, value);
                if (value)
                {
                    Settings.Default.UseDefaultFields = false;
                }
                else
                {
                    Settings.Default.UseDefaultFields = true;
                }
                Settings.Default.Save();
            }
        }

        private bool _AllFieldCheck;
        public bool AllFieldCheck
        {
            get => _AllFieldCheck;
            set
            {
                SetProperty(ref _AllFieldCheck, value);
                if (value)
                {
                    Settings.Default.UseDefaultFields = true;
                }
                else
                {
                    Settings.Default.UseDefaultFields = false;
                }
                Settings.Default.Save();

            }
        }

        public List<string> _cmbDropDownList = new List<string>();
        public List<string> CmbDropDownList
        {
            get => _cmbDropDownList;
            set => SetProperty(ref _cmbDropDownList, value);
        }


        private int _selectedBox3Combo;
        public int SelectedBox3Combo
        {
            get => _selectedBox3Combo;
            set
            {
                SetProperty(ref _selectedBox3Combo, value);
                if (value > -1)
                {
                    Settings.Default.iRTBox3 = value;
                    Settings.Default.Save();
                }
            }
        }

        private string _graphHigh;
        public string GraphHigh
        {
            get => _graphHigh;
            set => SetProperty(ref _graphHigh, value);
        }

        private string _graphLow;
        public string GraphLow
        {
            get => _graphLow;
            set => SetProperty(ref _graphLow, value);
        }


        /// <summary>
        /// Leave Page Clean Up
        /// </summary>
        public void UnloadPage()
        {
            //_chartControlViewModel.UnLoadPage();

            _cts?.Cancel();
            _cts?.Dispose();
        }

        public void LoadPage()
        {
           CombineView._combineView?.Clearbale();
           CombineView._combineView?.ShowStopBtn(false);
           ClsSerilog.LogMessage(ClsSerilog.INFO, $"---------- Load Page Combine");
          
        }


        private void ReloadPage()
        {
            Setup_DropDownLists();

            SelectedLineVal = Settings.Default.SelectedLineVal;
            SelectedSourceVal = Settings.Default.SelectedSourceVal;
          
            CurMoistureHdr = ClassCommon.MoistureTypeLst[ClassCommon.MoistureType];
            CurWeightHdr = ClassCommon.WeightTypeLst[ClassCommon.WeightUnit]; //"Current Bale Weight

            if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
            {
                GraphLimitHi = Settings.Default.GraphOneLimitHi;
                GraphLimitLo = Settings.Default.GraphOneLimitLo;

            }
            else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
            {
                GraphLimitHi = Settings.Default.GraphTwoLimitHi;
                GraphLimitLo = Settings.Default.GraphTwoLimitLo;
            }
        }


        public CombineViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
       
            RealTimeSumDataTable = new DataTable();

            if (Settings.Default.UseDefaultFields == true) AllFieldCheck = true;
            else CustFieldCheck = true;

            if (_sqlhandler == null) 
                _sqlhandler = ClassSqlHandler.Instance;
            
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(ClassCommon.ScanSec));

            Setup_DropDownLists();

            _eventAggregator.GetEvent<ChangeMenuEvents>().Subscribe(UpdateSettings);

            List<string> LL = _sqlhandler.GetLineList();
            if (LL.Count > 0) { LL.Add("ALL"); }
            LineList = LL;

            List<string> SL = _sqlhandler.GetSourceList();
            if (SL.Count > 0) { SL.Add("ALL"); }
            SourceList = SL;

            SelectedBox3Combo = Settings.Default.iRTBox3;

        }

        private void UpdateSettings(int obj)
        {
            ReloadPage();
        }

        private void Setup_DropDownLists()
        {
            CmbDropDownList = _sqlhandler.GetBigBoxHdrList();
            if (CmbDropDownList.Count < 1) CmbDropDownList.Add("Forte");

         //   CmbDropDownList.Sort();

            SelectedBox3Combo = Settings.Default.iRTBox3;
        }


        private void UpdateBigNumber(DataTable myTable)
        {

            try
            {
                if (myTable != null)
                {
                    if (myTable.Rows.Count > 0)
                    {
                        for (int i = 0; i < CmbDropDownList.Count; i++)
                        {
                            if (CmbDropDownList[i].Contains("Viscosity"))
                                CmbDropDownList[i] = "Finish";

                            if (CmbDropDownList[i].Contains("CusLotNumber"))
                                CmbDropDownList[i] = "FC_LotIdentString";

                            if (CmbDropDownList[i].Contains("%CV"))
                                CmbDropDownList[i] = "SpareSngFld3";

                            if (CmbDropDownList[i].Contains(ClassCommon.MoistureTypeLst[ClassCommon.MoistureType]))
                                CmbDropDownList[i] = "Moisture";
                        }
                        SpareBox = SetBigBoxData(myTable, SelectedBox3Combo);
                       

                    //  BigComboBox = new string[] { strBox1, strBox2, strBox3, strBox4, strBox5 };

                        for (int i = 0; i < CmbDropDownList.Count; i++)
                        {
                            if (CmbDropDownList[i].Contains("SpareSngFld3"))
                                CmbDropDownList[i] = "%CV";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in UpdateBigNumber {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in Update Big Number < {ex.Message}");
            }
        }

        private string SetBigBoxData(DataTable myTable, int selectedIndex)
        {
            string strItem = string.Empty;
            string strFormat = "HH:mm:ss";

            try
            {
                var Newdat = myTable.Rows[0][CmbDropDownList[selectedIndex]];

                if ((Newdat.GetType().FullName == "System.Single") || (Newdat.GetType().FullName == "System.Double"))
                {
                    strItem = Convert.ToDouble(Newdat.ToString()).ToString("0.##");
                }
                else if (Newdat.GetType().FullName == "System.DateTime")
                    strItem = Convert.ToDateTime(Newdat).ToString(strFormat); //System.DateTime
                else
                    strItem = Newdat.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in SetBigBoxData {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in SetBigBoxData < {ex.Message}");
            }
            return strItem;
        }


        private DelegateCommand _startCommand;
        public DelegateCommand StartCommand =>
        _startCommand ?? (_startCommand = new DelegateCommand(StartExecute).ObservesCanExecute(() => RTIdle));

        private  void StartExecute()
        {
            _sqlhandler.GetCustomFieldsList();
            preIndex = 0;
            RTRunning = true;

            SetChartTitle(ClassCommon.MenuChecked);
            ReloadPage();
            StartRtTimer();
           

            _eventAggregator.GetEvent<UpdateAppRunEvents>().Publish(true);

            CombineView._combineView?.ShowStartBtn(false);
            CombineView._combineView?.ShowStopBtn(true);

        }

        private DelegateCommand _stopCommand;
        public DelegateCommand StopCommand =>
        _stopCommand ?? (_stopCommand = new DelegateCommand(StopExecute).ObservesCanExecute(() => RTRunning));
        private void StopExecute()
        {
            _ = StopAsync();

            RTRunning = false;

            _eventAggregator.GetEvent<UpdateAppRunEvents>().Publish(false);

            CombineView._combineView?.ShowStartBtn(true);
            CombineView._combineView?.ShowStopBtn(false);
        }

        private DelegateCommand _setFieldCommand;
        public DelegateCommand SetFieldCommand =>
        _setFieldCommand ?? (_setFieldCommand = new DelegateCommand(SstFieldExecute).ObservesCanExecute(() => RTIdle));
        private void SstFieldExecute()
        {
            FieldSelectView MyDataItems = new FieldSelectView(_eventAggregator)
            {
                Height = 460,
                Width = 960
            };
            MyDataItems.ShowDialog();
        }


        private void SetChartTitle(int nIndex)
        {

            switch (nIndex)
            {
                case 0:
                    ChartTitle = $"Graph of {ClassCommon.MoistureTypeLst[ClassCommon.MoistureType]} from the newest {ClassCommon.ComBineSample} items  ";
                    break;
                case 1:
                    ChartTitle = $"Graph of Weight (kg) from the newest {ClassCommon.ComBineSample} items ";
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }


        #region Timer

        public void StartRtTimer()
        {
            _cts = new CancellationTokenSource();
            _timerTask = DoWorkAsync();
        }

        public async Task StopAsync()
        {
            if (_timer is null)
            {
                return;
            }
            _cts?.Cancel();
            await _timerTask;
            _cts?.Dispose();
        }

        private async Task DoWorkAsync()
        {
            if (RealTimeSumDataTable != null) RealTimeSumDataTable = null;
            RealTimeSumDataTable = new DataTable();
            try
            {
                while (await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    _ = UpdateDataAsync();
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        private async Task UpdateDataAsync()
        {
            CombineView._combineView?.Clearbale();
            DataTable rDt = new DataTable();

            List<double> MData = new List<double>();
            List<double> WData = new List<double>();

            string YTitle = string.Empty;

            try
            {
                rDt = await GetCurrentDataTableAsync();

                if (rDt.Rows.Count > 0)
                {
                    newIndex = rDt.Rows[0].Field<int>("Index");

                    timeX = new double[rDt.Rows.Count];
                    valueY = new double[rDt.Rows.Count];
 
                    if (preIndex != newIndex)
                    {
                        _sqlhandler.SetMoistureType(rDt);
                        _sqlhandler.SetWeightType(rDt); 

                        RealTimeSumDataTable = _sqlhandler.SetDataFields(rDt, CustFieldCheck);

                        UpdateBigNumber(rDt);


                        for (int i = 0; i < rDt.Rows.Count; i++)
                        {
                            timeX[i] = i + 1;

                            if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                            {
                                valueY[i] = rDt.Rows[i].Field<float>("Moisture");
                                YTitle = ClassCommon.MoistureTypeLst[ClassCommon.MoistureType];
                            }
                            else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                            {
                                valueY[i] = rDt.Rows[i].Field<float>("Weight");
                                YTitle = ClassCommon.WeightTypeLst[ClassCommon.WeightUnit];
                            }
                                
                        }

                        CombineView._combineView?.PlotChart(timeX, valueY, ChartTitle, YTitle);

                        if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                        {
                            for (int i = 0; i < ClassCommon.ComBineSample; i++)
                            {
                                MData.Add(valueY[i]);
                            }
                            double mMax = MData.Max();
                            double mMin = MData.Min();

                            GraphHigh = mMax.ToString("#0.00");
                            GraphLow = mMin.ToString("#0.00");
                        }
                        if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                        {
                            for (int i = 0; i < ClassCommon.ComBineSample; i++)
                            {
                                WData.Add(valueY[i]);
                            }
                            double wMax = WData.Max();
                            double wMin = WData.Min();

                            GraphHigh = wMax.ToString("#0.00");
                            GraphLow = wMin.ToString("#0.00");
                        }

                        CurMoisture = ClassCommon.ConvertMoisture(RealTimeSumDataTable.Rows[0].Field<Single>("Moisture"), ClassCommon.MoistureType).ToString("#0.00");
                        CurWeight = ClassCommon.ConvertWeight(RealTimeSumDataTable.Rows[0].Field<Single>("Weight"), ClassCommon.WeightUnit).ToString("#0.00");

                        int iPosition = rDt.Rows[0].Field<byte>("Position");

                        SummaryIdx = 0;

                        preIndex = newIndex;

                        CombineView._combineView?.MoveBaleOne(iPosition);

                        _eventAggregator.GetEvent<UpdateRealTimeEvents>().Publish(DateTime.Now);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in UpdateDataAsync {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in UpdateDataAsync < {ex.Message}");
            }
        }

        internal async Task<DataTable> GetCurrentDataTableAsync()
        {
            string criteria = GetstrCriteria();
            return await _sqlhandler.GetCurrentDataTableAsyn("*", ClassCommon.ComBineSample, criteria);
        }


        /// <summary>
        /// Check for Line and Source
        /// </summary>
        /// <returns></returns>
        private string GetstrCriteria()
        {
            string strCondition = string.Empty;

            string Linelogic = string.Empty;

            string line = Settings.Default.SelectedLineVal;
            string source = Settings.Default.SelectedSourceVal;

            if ((line == "ALL") & (source == "ALL"))
            {
                strCondition = string.Empty;
            }
            else if ((line == "ALL") & (source != "ALL"))
            {
                strCondition = $" Where SourceName = '{source}' ";
            }
            else if ((line != "ALL") & (source == "ALL"))
            {
                strCondition = $" Where LineName = '{line}' ";
            }
            else if ((line != "ALL") & (source != "ALL"))
            {
                strCondition = $" Where SourceName = '{source}' AND LineName = '{line}' ";
            }

            return strCondition;
        }


        #endregion Timer


    }
}
