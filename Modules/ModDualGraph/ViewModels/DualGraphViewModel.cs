using AppServices;
using ModDualGraph.Properties;
using ModDualGraph.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static AppServices.ClassApplicationService;

namespace ModDualGraph.ViewModels
{
    public  class DualGraphViewModel : BindableBase
    {
        public static string ModuleName = "RealTime Moisture and Weight Graphs";
        protected readonly IEventAggregator _eventAggregator;

        private readonly ClassSqlHandler _sqlhandler;

        private Task _timerTask = null;
        private PeriodicTimer _timer; // = new PeriodicTimer(TimeSpan.FromSeconds(5));
        private CancellationTokenSource _cts; // = new CancellationTokenSource();

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

        private long preIndex = 0;
        private long newIndex = 0;

        double[] timeX;
        double[] valueYM;
        double[] valueYW;



       
        //Moisture

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
        private string _moisturelow;
        public string MoistureLow
        {
            get { return _moisturelow; }
            set { SetProperty(ref _moisturelow, value); }
        }
        private string _moistureavg;
        public string MoistureAVG
        {
            get { return _moistureavg; }
            set { SetProperty(ref _moistureavg, value); }
        }
        private string _moisturehi;
        public string MoistureHi
        {
            get { return _moisturehi; }
            set { SetProperty(ref _moisturehi, value); }
        }


        //Weight

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
        private string _weightlow;
        public string WeightLow
        {
            get { return _weightlow; }
            set { SetProperty(ref _weightlow, value); }
        }
        private string _weighthi;
        public string WeightHi
        {
            get { return _weighthi; }
            set { SetProperty(ref _weighthi, value); }
        }
        private string _weightavg;
        public string WeightAVG
        {
            get { return _weightavg; }
            set { SetProperty(ref _weightavg, value); }
        }

        //Lines
        private List<string> _LineList;
        public List<string> LineList
        {
            get => _LineList;
            set => SetProperty(ref _LineList, value);
        }

        private int _selectedLineindex;
        public int SelectedLineindex
        {
            get { return _selectedLineindex; }
            set
            {
                SetProperty(ref _selectedLineindex, value);
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
        private int _selectedSourceindex;
        public int SelectedSourceindex
        {
            get { return _selectedSourceindex; }
            set
            {
                SetProperty(ref _selectedSourceindex, value);
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

       




        public DualGraphViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            _sqlhandler = ClassSqlHandler.Instance;
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(ClassCommon.ScanSec));

            List<string> LL = _sqlhandler.GetLineList();
            if (LL.Count > 0) { LL.Add("ALL"); }
            LineList = LL;

            List<string> SL = _sqlhandler.GetSourceList();
            if (SL.Count > 0) { SL.Add("ALL"); }
            SourceList = SL;


        }

        private DelegateCommand _startCommand;
        public DelegateCommand StartCommand =>
        _startCommand ?? (_startCommand = new DelegateCommand(StartExecute).ObservesCanExecute(() => RTIdle));

        private void StartExecute()
        {
            preIndex = 0;
            RTRunning = true;

            StartGrTimer();

            DualGraphView._dualGraphView.ShowStartBtn(false);
            DualGraphView._dualGraphView.ShowStopBtn(true);

            _eventAggregator.GetEvent<UpdateAppRunEvents>().Publish(true);

        }


        private DelegateCommand _stopCommand;
        public DelegateCommand StopCommand =>
        _stopCommand ?? (_stopCommand = new DelegateCommand(StopExecute).ObservesCanExecute(() => RTRunning));
        private void StopExecute()
        {
            _ = StopAsync();
            RTRunning = false;

            DualGraphView._dualGraphView.ShowStartBtn(true);
            DualGraphView._dualGraphView.ShowStopBtn(false);

            _eventAggregator.GetEvent<UpdateAppRunEvents>().Publish(false);

        }




        #region Timer

        public void StartGrTimer()
        {
            _cts = new CancellationTokenSource();
            _timerTask = DoWorkAsync();
        }

        private async Task DoWorkAsync()
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    _ = UpdateGraphicDataAsync();
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        private async Task UpdateGraphicDataAsync()
        {
            List<double> MData = new List<double>();
            List<double> WData = new List<double>();

            DualGraphView._dualGraphView.Clearbale();

            string YMTitle = ClassCommon.MoistureTypeLst[ClassCommon.MoistureType];
            string YWTitle = ClassCommon.WeightUnitLst[ClassCommon.WeightUnit];

           string  ChartTitleM = $"Graph of {ClassCommon.MoistureTypeLst[ClassCommon.MoistureType]} from the newest {ClassCommon.ComBineSample} items  ";
           
           string ChartTitleW = $"Graph of {ClassCommon.WeightTypeLst[ClassCommon.WeightUnit]} from the newest {ClassCommon.ComBineSample} items  ";

            try
            {
                DataTable rDt = new DataTable();

                rDt = await GetCurrentDataTableAsync();

                if (rDt.Rows.Count > 0)
                {
                    newIndex = rDt.Rows[0].Field<int>("UID");

                    timeX = new double[rDt.Rows.Count];
                    valueYM = new double[rDt.Rows.Count];
                    valueYW = new double[rDt.Rows.Count];


                    for (int i = 0; i < rDt.Rows.Count; i++)
                    {
                        timeX[i] = i + 1;

                        valueYM[i] = ClassCommon.ConvertMoisture(rDt.Rows[i].Field<float>("Moisture"), ClassCommon.MoistureType);
                        valueYW[i] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("Weight"), ClassCommon.WeightUnit);
                    }

                    if (preIndex != newIndex)
                    {
                        DualGraphView._dualGraphView.PlotChartOne(timeX, valueYM, ChartTitleM, YMTitle);
                        DualGraphView._dualGraphView.PlotChartTwo(timeX, valueYW, ChartTitleW, YWTitle);

                        preIndex = newIndex;
                        _eventAggregator.GetEvent<UpdateRealTimeEvents>().Publish(DateTime.Now);

                        int iPosition = rDt.Rows[0].Field<byte>("Position");
                        DualGraphView._dualGraphView.MoveBaleOne(iPosition);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in UpdateGraphicDataAsync {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in UpdateGraphicDataAsync < {ex.Message}");
            }
        }


        internal async Task<DataTable> GetCurrentDataTableAsync()
        {
            string criteria = GetstrCriteria();

            return await _sqlhandler.GetCurrentDataTableAsyn("*", ClassCommon.ComBineSample, criteria);
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

        public void UnLoadPage()
        {

            //if (_chartControl != null) _chartControl = null;

            _cts?.Cancel();
            _cts?.Dispose();
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
