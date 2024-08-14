using AppServices;
using DataFieldsSelect.Views;
using ModRealTime.Properties;
using ModRealTime.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;
using static AppServices.ClassApplicationService;

namespace ModRealTime.ViewModels
{
    public class RealTimeViewModel : BindableBase
    {

        public static string ModuleName = "Realtime Data";
        private readonly ClassSqlHandler _sqlhandler;
        protected readonly IEventAggregator _eventAggregator;

        private Task _timerTask;
        private PeriodicTimer _timer;
        private CancellationTokenSource _cts;
      
        private long preIndex = 0;
        private long newIndex = 0;

        private bool _rtIdle = true;
        public bool RTIdle
        {
            get => _rtIdle;
            set { SetProperty(ref _rtIdle, value); }
        }

        private bool _rtrunning = false;
        public bool RTRunning
        {
            get => _rtrunning;
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



        #region BigBox

        private string[] _combobox;
        public string[] BigComboBox
        {
            get => _combobox;
            set { SetProperty(ref _combobox, value); }
        }

        private int _selectedBox1Combo;
        public int SelectedBox1Combo
        {
            get => _selectedBox1Combo; 
            set 
            { 
                SetProperty(ref _selectedBox1Combo, value);
                if(value > -1)
                {
                    Settings.Default.iRTBox1 = value;
                    Settings.Default.Save();
                }
            }
        }

        private int _selectedBox2Combo;
        public int SelectedBox2Combo
        {
            get => _selectedBox2Combo; 
            set 
            { 
                SetProperty(ref _selectedBox2Combo, value);
                if (value > -1)
                {
                    Settings.Default.iRTBox2 = value;
                    Settings.Default.Save();
                }
            }
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

        private int _selectedBox4Combo;
        public int SelectedBox4Combo
        {
            get => _selectedBox4Combo;
            set 
            { 
                SetProperty(ref _selectedBox4Combo, value);
                if (value > -1)
                {
                    Settings.Default.iRTBox4 = value;
                    Settings.Default.Save();
                }
            }
        }

        private int _selectedBox5Combo;
        public int SelectedBox5Combo
        {
            get => _selectedBox5Combo;
            set 
            { 
                SetProperty(ref _selectedBox5Combo, value);
                if (value > -1)
                {
                    Settings.Default.iRTBox5 = value;
                    Settings.Default.Save();
                }
            }
        }


        private int _summaryIdx;
        public int SummaryIdx
        {
            get { return _summaryIdx; }
            set
            {
                SetProperty(ref _summaryIdx, value);
                if (value > -1)
                {
            
                }
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

        public List<string> _cmbDropDownList = new List<string>();
        public List<string> CmbDropDownList
        {
            get => _cmbDropDownList;
            set => SetProperty(ref _cmbDropDownList, value);
        }

        #endregion BigBox

        private DataTable _realtimeSumdatatable;
        public DataTable RealTimeSumDataTable
        {
            get => _realtimeSumdatatable;
            set { SetProperty(ref _realtimeSumdatatable, value); }
        }

        private DelegateCommand _startCommand;
        public DelegateCommand StartCommand =>
        _startCommand ?? (_startCommand = new DelegateCommand(StartExecute).ObservesCanExecute(() => RTIdle));
        private void StartExecute()
        {
            _sqlhandler.GetCustomFieldsList();

            StartRtTimer();
            RTRunning = true;
            preIndex = 0;
            _eventAggregator.GetEvent<UpdateAppRunEvents>().Publish(true);

            RealTimeView.RTWindows.ShowStartBtn(false);
            RealTimeView.RTWindows.ShowStopBtn(true);

        }

        private DelegateCommand _stopCommand;
        public DelegateCommand StopCommand =>
        _stopCommand ?? (_stopCommand = new DelegateCommand(StopExecute).ObservesCanExecute(() => RTRunning));
        private void StopExecute()
        {
            _ = StopAsync();
            RTRunning = false;
            _eventAggregator.GetEvent<UpdateAppRunEvents>().Publish(false);

            RealTimeView.RTWindows.ShowStartBtn(true);
            RealTimeView.RTWindows.ShowStopBtn(false);

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

        /// <summary>
        /// Leave Page Clean Up
        /// </summary>
        public  void UnloadPage()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public void LoadPage()
        {
            ClsSerilog.LogMessage(ClsSerilog.INFO, $"---------- Load Page RealTime");
        }

        public RealTimeViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            if(_sqlhandler == null)
                _sqlhandler = ClassSqlHandler.Instance;
            
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(ClassCommon.ScanSec));

            Setup_DropDownLists();

            _eventAggregator.GetEvent<SettingsChangedEvents>().Subscribe(UpdateSettings);

            List<string> LL = _sqlhandler.GetLineList();
            if(LL.Count > 0) { LL.Add("ALL"); }
            LineList = LL;

            List<string> SL = _sqlhandler.GetSourceList();
            if (SL.Count > 0) { SL.Add("ALL"); }
            SourceList = SL;

            if (Settings.Default.UseDefaultFields == true) AllFieldCheck = true;
            else CustFieldCheck = true;

           
        }

        private void UpdateSettings(bool obj)
        {
            Setup_DropDownLists();
        }

        private void Setup_DropDownLists()
        {
           CmbDropDownList = _sqlhandler.GetBigBoxHdrList();
            if (CmbDropDownList.Count < 1) CmbDropDownList.Add("Forte");

            SelectedBox1Combo = Settings.Default.iRTBox1;
            SelectedBox2Combo = Settings.Default.iRTBox2;
            SelectedBox3Combo = Settings.Default.iRTBox3;
            SelectedBox4Combo = Settings.Default.iRTBox4;
            SelectedBox5Combo = Settings.Default.iRTBox5;

            CmbDropDownList.Sort();
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

                        _sqlhandler.SetMoistureType(myTable);

                        string strBox1 = SetBigBoxData(myTable, _selectedBox1Combo);
                        string strBox2 = SetBigBoxData(myTable, _selectedBox2Combo);
                        string strBox3 = SetBigBoxData(myTable, _selectedBox3Combo);
                        string strBox4 = SetBigBoxData(myTable, _selectedBox4Combo);
                        string strBox5 = SetBigBoxData(myTable, _selectedBox5Combo);

                        BigComboBox = new string[] { strBox1, strBox2, strBox3, strBox4, strBox5 };

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
            RealTimeView.RTWindows.Clearbale();

            string criteria = GetstrCriteria();

            DataTable rDt = await _sqlhandler.GetCurrentDataTableAsyn("*", ClassCommon.ComBineSample, criteria);

            if (rDt.Rows.Count > 0) 
            {      
                newIndex = rDt.Rows[0].Field<int>("Index");

                if (preIndex != newIndex)
                {
                    _sqlhandler.SetWeightType(rDt);

                    UpdateBigNumber(rDt);
                    RealTimeSumDataTable = _sqlhandler.SetDataFields(rDt, CustFieldCheck);   
                    preIndex = newIndex;

                    SummaryIdx = 0;

                    int iPosition = rDt.Rows[0].Field<byte>("Position");

                    RealTimeView.RTWindows.MoveBaleOne(iPosition);

                  

                    _eventAggregator.GetEvent<UpdateRealTimeEvents>().Publish(DateTime.Now);
                }   
            }   
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
