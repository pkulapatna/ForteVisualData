using AppServices;
using GraphMenuBar.ViewModels;
using GraphMenuBar.Views;
using ModGraphic.Properties;
using ModGraphic.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Linq;
using static AppServices.ClassApplicationService;

namespace ModGraphic.ViewModels
{
    public class GraphicViewModel : BindableBase
    {
        public static string ModuleName = "Realtime Graphic Data";
        protected readonly IEventAggregator _eventAggregator;
        
        private readonly ClassSqlHandler _sqlhandler;

        private Task _timerTask = null;
        private PeriodicTimer _timer; // = new PeriodicTimer(TimeSpan.FromSeconds(5));
        private CancellationTokenSource _cts; // = new CancellationTokenSource();

      
        public UserControl TopMenuOneBar
        {
            get => new MenuBarView(_eventAggregator);
        }

        double[] timeX;
        double[] valueY;

        private long preIndex = 0;
        private long newIndex = 0;

        public int BaleSample = ClassCommon.ComBineSample;


        private DataTable _realTimeDataTable;
        public DataTable RealTimeDataTable
        {
            get { return _realTimeDataTable; }
            set { SetProperty(ref _realTimeDataTable, value); }
        }

        private string _chartTitle;
        public string ChartTitle
        {
            get => _chartTitle; 
            set => SetProperty(ref _chartTitle, value); 
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

                if (value > -1)
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



       
      
        private string _graphHigh;
        public string GraphHigh
        {
            get => _graphHigh; 
            set =>  SetProperty(ref _graphHigh, value); 
        }

        private string _graphLow;
        public string GraphLow
        {
            get => _graphLow;
            set => SetProperty(ref _graphLow, value);
        }

        public void LoadPage()
        {
            
            //  MenuOneChecked = true;
            ClsSerilog.LogMessage(ClsSerilog.INFO, $"---------- Load Page Graphic");
        }

        private void ReloadPage()
        {
            ModuleName = "Realtime Graphic Data";
            SelectedLineVal = Settings.Default.SelectedLineVal;
            BaleSample = ClassCommon.ComBineSample;




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

        public void UnloadPage()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public GraphicViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            _sqlhandler = ClassSqlHandler.Instance;
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(ClassCommon.ScanSec));


            _eventAggregator.GetEvent<ChangeMenuEvents>().Subscribe(UpdateSettings);

            List<string> LL = _sqlhandler.GetLineList();
            if (LL.Count > 0) { LL.Add("ALL"); }
            LineList = LL;

            List<string> SL = _sqlhandler.GetSourceList();
            if (SL.Count > 0) { SL.Add("ALL"); }
            SourceList = SL;

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

        private void UpdateSettings(int obj)
        {
            SetChartTitle(obj);

            ReloadPage();
        }

        private DelegateCommand _startCommand;
        public DelegateCommand StartCommand =>
        _startCommand ?? (_startCommand = new DelegateCommand(StartExecute).ObservesCanExecute(() => RTIdle));

        private void StartExecute()
        {
            preIndex = 0;
            RTRunning = true;
           
            SetChartTitle(ClassCommon.MenuChecked);
            StartGrTimer();

            GraphicView._graphicView.ShowStartBtn(false);
            GraphicView._graphicView.ShowStopBtn(true);

            _eventAggregator.GetEvent<UpdateAppRunEvents>().Publish(true);

        }

        private DelegateCommand _stopCommand; 
        public DelegateCommand StopCommand =>
        _stopCommand ?? (_stopCommand = new DelegateCommand(StopExecute).ObservesCanExecute(() => RTRunning));
        private void StopExecute()
        {
            _ = StopAsync();
            RTRunning = false;

            GraphicView._graphicView.ShowStartBtn(true);
            GraphicView._graphicView.ShowStopBtn(false);

            _eventAggregator.GetEvent<UpdateAppRunEvents>().Publish(false);

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

            GraphicView._graphicView.Clearbale();

            string YTitle = string.Empty; // = ClassCommon.MoistureTypeLst[ClassCommon.MoistureType];

            try
            {
                DataTable rDt = new DataTable();

                rDt = await GetCurrentDataTableAsync();

                if (rDt.Rows.Count > 0)
                {
                    newIndex = rDt.Rows[0].Field<int>("UID");

                    timeX = new double[rDt.Rows.Count];
                    valueY = new double[rDt.Rows.Count];
                    for (int i = 0; i < rDt.Rows.Count; i++)
                    {
                        timeX[i] = i + 1;

                        if(ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                        {
                            //valueY[i] = rDt.Rows[i].Field<float>("Moisture");
                            valueY[i] = ClassCommon.ConvertMoisture(rDt.Rows[i].Field<float>("Moisture"), ClassCommon.MoistureType);
                            YTitle = ClassCommon.MoistureTypeLst[ClassCommon.MoistureType];
                        }
                        else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                        {
                            valueY[i] = ClassCommon.ConvertWeight(rDt.Rows[i].Field<float>("Weight"), ClassCommon.WeightUnit);
                            YTitle = ClassCommon.WeightTypeLst[ClassCommon.WeightUnit];
                        }
                            
                    }

                    if (preIndex != newIndex)
                    {
                        GraphicView._graphicView.PlotChart(timeX, valueY, ChartTitle, YTitle);


                        preIndex = newIndex;
                        _eventAggregator.GetEvent<UpdateRealTimeEvents>().Publish(DateTime.Now);

                        int iPosition = rDt.Rows[0].Field<byte>("Position");

                        GraphicView._graphicView.MoveBaleOne(iPosition);

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
