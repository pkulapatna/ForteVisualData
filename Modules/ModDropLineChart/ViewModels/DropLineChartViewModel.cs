using AppServices;
using GraphMenuBar.ViewModels;
using GraphMenuBar.Views;
using ModDropLineChart.Properties;
using ModDropLineChart.Views;
using OpenTK.Mathematics;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using static AppServices.ClassApplicationService;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ModDropLineChart.ViewModels
{
    public class DropLineChartViewModel : BindableBase
    {
        private readonly ClassSqlHandler _sqlhandler;
        private Task _timerTask;
        private PeriodicTimer _timer;
        private CancellationTokenSource _cts;

        private bool debugy = false;

        private readonly ClassXml MyXml = new ClassXml();

        private long preIndex = 0;
        private long newIndex = 0;

        private int CurIndexL1 = 0;
        private int PreIndexL1 = 0;

        private int CurIndexL2 = 0;
        private int PreIndexL2 = 0;

        private const int iLine1 = 0;
        private const int iLine2 = 1;

        public static string ModuleName = "Profile of Bale Data in Each Position";
        private IEventAggregator _eventAggregator;

        private DataTable DropDatatable;
        private DataTable DatatableLine2;

        private List<string> HeaderList;
        private List<string> HeaderFieldsList;


        private List<Tuple<byte, Single>> ChartPtLst { get; set; }

        private int _showDropInListView = 2;
        public int ShowDropInListView
        {
            get { return _showDropInListView; }
            set { SetProperty(ref _showDropInListView, value); }
        }



        private List<string> _selectedItemList;
        public List<string> SelectedHdrList
        {
            get { return _selectedItemList; }
            set { SetProperty(ref _selectedItemList, value); }
        }

        private string _AverageHeader;
        public string AverageHeader
        {
            get { return _AverageHeader; }
            set { SetProperty(ref _AverageHeader, value); }
        }

        private int _dropSamples;
        public int DropSamples
        {
            get { return _dropSamples; }
            set { SetProperty(ref _dropSamples, value); }
        }


        public List<double> BalePos1Lst { get; set; }


        private int _balePositionL1;
        public int BalePositionL1
        {
            get { return _balePositionL1; }
            set { SetProperty(ref _balePositionL1, value); }
        }

        private int _balePositionL2;
        public int BalePositionL2
        {
            get { return _balePositionL2; }
            set { SetProperty(ref _balePositionL2, value); }
        }


        private string[] _balePosAvg;
        public string[] BalePosAvg
        {
            get { return _balePosAvg; }
            set { SetProperty(ref _balePosAvg, value); }
        }


        private Visibility[] _dropVisible;
        public Visibility[] DropVisible
        {
            get { return _dropVisible; }
            set { SetProperty(ref _dropVisible, value); }
        }



       
        private List<Double> BalesLis1;
        private List<Double> BalesLis2;
        private List<Double> BalesLis3;
        private List<Double> BalesLis4;
        private List<Double> BalesLis5;
        private List<Double> BalesLis6;
        private List<Double> BalesLis7;
        private List<Double> BalesLis8;
        private List<Double> BalesLis9;
        private List<Double> BalesLis10;




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

        //Number of Drops in chart 3 to 5 only.
        private List<int> _dropList = new List<int>() {2,3,4,5,6};
        public List<int> DropList
        {
            get => _dropList;
            set => SetProperty(ref _dropList, value);
        }
        private int _selectedDropsindex;
        public int SelectedDropsindex
        {
            get => _selectedDropsindex; 
            set => SetProperty(ref _selectedDropsindex, value);
        }
       

        private int _selectedDropCt = Settings.Default.SelectedDropCt;
        public int SelectedDropCt
        {
            get { return _selectedDropCt; }
            set
            {
                SetProperty(ref _selectedDropCt, value);
                if (value >0)
                {
                    Settings.Default.SelectedDropCt = value;
                    Settings.Default.Save();

                    DropSamples = Settings.Default.SelectedDropCt;
                    UpdateLabel();
                }
            }
        }


        private int _BalePositionL1;
        public int BalePosition
        {
            get { return _BalePositionL1; }
            set { SetProperty(ref _BalePositionL1, value); }
        }

        public UserControl TopMenuOneBar
        {
            get => new MenuBarView(_eventAggregator);
        }

        private DataTable _realtimeSumdatatable;
        public DataTable RealTimeSumDataTable
        {
            get => _realtimeSumdatatable;
            set => SetProperty(ref _realtimeSumdatatable, value);
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

        public DropLineChartViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;


            _eventAggregator.GetEvent<ChangeMenuEvents>().Subscribe(UpdateSettings);

            _timer = new PeriodicTimer(TimeSpan.FromSeconds(ClassCommon.ScanSec));

            _sqlhandler = ClassSqlHandler.Instance;
            List<string> LL = _sqlhandler.GetLineList();
            LineList = LL;

            InitGraph();

  
        }

        private void UpdateSettings(int obj)
        {
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                AverageHeader = $"Average  of each Bale position {ClassCommon.MoistureUnitLst[ClassCommon.MoistureType]} in {DropSamples} drops";
            else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                AverageHeader = $"Average  of each bale position {ClassCommon.WeightTypeLst[ClassCommon.WeightUnit]} in {DropSamples} drops";
        }

        private DelegateCommand _startCommand;
        public DelegateCommand StartCommand =>
        _startCommand ?? (_startCommand = new DelegateCommand(StartExecute).ObservesCanExecute(() => RTIdle));

        private void StartExecute()
        {
            RTRunning = true;
            preIndex = 0;

       
            StartRtTimer();

            _eventAggregator.GetEvent<UpdateAppRunEvents>().Publish(true);
              DropLineChartView._dropLineChartView?.ShowStartBtn(false);
              DropLineChartView._dropLineChartView?.ShowStopBtn(true);
        }

        private DelegateCommand _stopCommand;
        public DelegateCommand StopCommand =>
        _stopCommand ?? (_stopCommand = new DelegateCommand(StopExecute).ObservesCanExecute(() => RTRunning));

        private void StopExecute()
        {
            RTRunning = false;

            _ = StopAsync();

            _eventAggregator.GetEvent<UpdateAppRunEvents>().Publish(false);

              DropLineChartView._dropLineChartView?.ShowStartBtn(true);
              DropLineChartView._dropLineChartView?.ShowStopBtn(false);
        }


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


        private async Task UpdateDataAsync()
        {
            

            await Task.Run(() =>
            {
                int iFirstBale;

                string ChartTitle = $"Bar Chart of {DropSamples} Consecutive Drops, Ordered by Drop Positions (First to Last)"; //Need change!

                string strGetIndex = $"Select Top 1 [index] FROM [ForteData].[dbo].[{CurrentBaleTable}] WHERE LineId={SelectedLineVal} ORDER BY [TimeStart] DESC; ";

                string strGetSingleNewData = $"Select Top 1  Position, Weight, Forte, Moisture, UpCount, DownCount, DropNumber, NetWeight, BDWeight, " +
                $"BasisWeight, LotBaleNumber,SerialNumber,DropNumber,[Index] FROM dbo.[{CurrentBaleTable}] WHERE LineId=1 And Position > 0 ORDER BY [TimeStart] DESC;";

                DropLineChartView._dropLineChartView?.Clearbale();

                try
                {
                    DataTable SingleDataTable = _sqlhandler.GetBaleArchiveDataTable(strGetSingleNewData);
                    BalePosition = Convert.ToInt32(SingleDataTable.Rows[0]["Position"].ToString());

                    if (ClassCommon.DropHitoLow)
                        iFirstBale = 1;
                    else
                        iFirstBale = ClassCommon.BaleInDrop;

                    if (BalePosition == iFirstBale)
                    {
                        DropDatatable.Clear();
                        CurIndexL1 = _sqlhandler.GetIntNewItemData(strGetIndex);

                        if (CurIndexL1 != PreIndexL1)
                        {
                            string newquery = BuildQueryString(1);
                            DropDatatable = _sqlhandler.GetBaleArchiveDataTable(newquery);

                            if (DropDatatable.Columns.Contains("index"))
                                DropDatatable.Columns.Remove("index");

                            PreIndexL1 = CurIndexL1;

                            ClassCommon.UpdataTableUnits(DropDatatable);

                            ProcessData(DropDatatable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error in UpdateDataAsync (DropLineChartViewModel) {ex.Message}");
                    ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in UpdateDataAsync (DropLineChartViewModel) < {ex.Message}");
                }
            });
        }

        private void ProcessData(DataTable baledatatable)
        {

            List<Single> itemOnelst = new List<Single>();

            List<Single> itemTwolst = new List<Single>();

            ChartPtLst = new List<Tuple<byte, Single>>();
            
            itemOnelst.Clear();

            try
            {
                if(baledatatable.Rows.Count >0)
                {
                   // var DropNumList = baledatatable.AsEnumerable().Select(x => x.Field<int>("DropNumber")).Distinct().ToList();

                    for(int i = 1; i < ClassCommon.BaleInDrop+1; i++)
                    {
                        for(int y=0; y< baledatatable.Rows.Count; y++)
                        {
                            if (baledatatable.Rows[y].Field<byte>("Position") == i)
                            {
                                itemOnelst.Add((Single)baledatatable.Rows[y]["Moisture"]);

                                if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                                    ChartPtLst.Add(new Tuple<byte, Single>((byte)i, (Single)baledatatable.Rows[y]["Moisture"]));
                                else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                                    ChartPtLst.Add(new Tuple<byte, Single>((byte)i, (Single)baledatatable.Rows[y]["Weight"]));
                            }
                        }
                    }
                    
                    Application.Current.Dispatcher.Invoke(new Action(() => { updateAvgVals(ChartPtLst); }));
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in ProcessData (DropLineChartViewModel) {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in ProcessData (DropLineChartViewModel) < {ex.Message}");
            }
        }

        private void updateAvgVals(List<Tuple<byte, float>> chartPtLst)
        {

            List<float> avgOne = new List<float>();
            List<float> avgTwo = new List<float>();
            List<float> avgThree = new List<float>();
            List<float> avgFour = new List<float>();
            List<float> avgFive = new List<float>();
            List<float> avgSix = new List<float>();

            List<float> avgSeven = new List<float>();
            List<float> avgEight = new List<float>();
            List<float> avgNine = new List<float>();
            List<float> avgTen = new List<float>();

            string[] averageVal = new string[10];

            double maxval = 0;
            double minval = 0;

            try
            {
                
                    for (int x = 0; x < chartPtLst.Count; x++)
                    {
                        if (chartPtLst[x].Item1 == 1)
                        {
                            avgOne.Add(chartPtLst[x].Item2);
                            maxval = avgOne.Max();
                            minval = avgOne.Min();
                        }
                        if (chartPtLst[x].Item1 == 2)
                        {
                            avgTwo.Add(chartPtLst[x].Item2);
                            if (avgTwo.Max() > maxval)
                                maxval = avgTwo.Max();

                            if ((avgTwo.Min() > 0) & (avgTwo.Min() < minval))
                                minval = avgTwo.Min();
                        }
                        if (chartPtLst[x].Item1 == 3)
                        {
                            avgThree.Add(chartPtLst[x].Item2);
                            if (avgThree.Max() > maxval)
                                maxval = avgThree.Max();

                            if ((avgThree.Min() > 0) & (avgThree.Min() < minval))
                                minval = avgThree.Min();
                        }
                        if (chartPtLst[x].Item1 == 4)
                        {
                            avgFour.Add(chartPtLst[x].Item2);
                            if (avgFour.Max() > maxval)
                                maxval = avgFour.Max();

                            if ((avgFour.Min() > 0) & (avgFour.Min() < minval))
                                minval = avgFour.Min();
                        }
                        if (chartPtLst[x].Item1 == 5)
                        {
                            avgFive.Add(chartPtLst[x].Item2);
                            if (avgFive.Max() > maxval)
                                maxval = avgFive.Max();

                            if ((avgFive.Min() > 0) & (avgFive.Min() < minval))
                                minval = avgFive.Min();
                        }
                        if (chartPtLst[x].Item1 == 6)
                        {
                            avgSix.Add(chartPtLst[x].Item2);
                            if (avgSix.Max() > maxval)
                                maxval = avgSix.Max();

                            if ((avgSix.Min() > 0) & (avgSix.Min() < minval))
                                minval = avgSix.Min();
                        }
                        if (chartPtLst[x].Item1 == 7)
                        {
                            avgSeven.Add(chartPtLst[x].Item2);
                            if (avgSeven.Max() > maxval)
                                maxval = avgSeven.Max();

                            if ((avgSeven.Min() > 0) & (avgSeven.Min() < minval))
                                minval = avgSeven.Min();
                        }
                        if (chartPtLst[x].Item1 == 8)
                        {
                            avgEight.Add(chartPtLst[x].Item2);
                            if (avgEight.Max() > maxval)
                                maxval = avgEight.Max();

                            if ((avgEight.Min() > 0) & (avgEight.Min() < minval))
                                minval = avgEight.Min();
                        }
                        if (chartPtLst[x].Item1 == 9)
                        {
                            avgNine.Add(chartPtLst[x].Item2);
                            if (avgNine.Max() > maxval)
                                maxval = avgNine.Max();

                            if ((avgNine.Min() > 0) & (avgNine.Min() < minval))
                                minval = avgNine.Min();
                        }
                        if (chartPtLst[x].Item1 == 10)
                        {
                            avgTen.Add(chartPtLst[x].Item2);
                            if (avgTen.Max() > maxval)
                                maxval = avgTen.Max();

                            if ((avgTen.Min() > 0) & (avgTen.Min() < minval))
                                minval = avgTen.Min();
                        }
                    }


                DropLineChartView._dropLineChartView.PlotChart(avgOne, avgTwo, avgThree, avgFour, avgFive, avgSix, avgSeven, avgEight, avgNine, avgTen, maxval, minval);


                if (avgOne.Count > 0)
                    averageVal[0] = avgOne.Average().ToString("#0.00");
                if (avgTwo.Count > 0)
                    averageVal[1] = avgTwo.Average().ToString("#0.00");
                if (avgThree.Count > 0)
                    averageVal[2] = avgThree.Average().ToString("#0.00");
                if (avgFour.Count > 0)
                    averageVal[3] = avgFour.Average().ToString("#0.00");
                if (avgFive.Count > 0)
                    averageVal[4] = avgFive.Average().ToString("#0.00");
                if (avgSix.Count > 0)
                    averageVal[5] = avgSix.Average().ToString("#0.00");
                if(avgSeven.Count > 0) 
                    averageVal[6] = avgSeven.Average().ToString("#0.00");
                if(avgEight.Count > 0)    
                        averageVal[7] = avgEight.Average().ToString("#0.00");
                if (avgNine.Count > 0)
                    averageVal[8] = avgNine.Average().ToString("#0.00");
                if (avgTen.Count > 0)
                    averageVal[9] = avgTen.Average().ToString("#0.00");




                BalePosAvg = new string[] 
                    {
                        averageVal[0],
                        averageVal[1],
                        averageVal[2],
                        averageVal[3],
                        averageVal[4],
                        averageVal[5],
                        averageVal[6],
                        averageVal[7],
                        averageVal[8],
                        averageVal[9]
                    };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in updateAvgVals (DropLineChartViewModel) {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in updateAvgVals (DropLineChartViewModel) < {ex.Message}");
            }
        }

        private string BuildQueryString(int LineID)
        {
            string strtemp = string.Empty;
            string strList = string.Empty;

            char charsToTrim = ',';

            int IntColSamples = DropSamples * ClassCommon.BaleInDrop;

            try
            {
                if (LineID > 0)
                {
                    string strLineSeleted = "WHERE LineId = " + LineID.ToString();

                    GetHeaderList();

                    strList += "[index],";
                    foreach (var item in HeaderFieldsList)
                    {
                        strList += item + ",";
                    }
                    //Get all fields 
                    strtemp = "SELECT TOP  "
                        + IntColSamples + " "
                        + strList.Trim(charsToTrim)
                        + " FROM " + "dbo.["
                        + CurrentBaleTable
                        + "] "
                        + strLineSeleted
                        + " ORDER BY [TimeStart] DESC";
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in BuildQueryString (DropLineChartViewModel) {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in BuildQueryString (DropLineChartViewModel) < {ex.Message}");
            }
            return strtemp;
        }

        private void GetHeaderList()
        {
            DataTable Hdrtable = new DataTable();

            HeaderFieldsList.Clear();
            HeaderList.Clear();

            try
            {

                Hdrtable = _sqlhandler.GetSqlScema();
                if (Hdrtable.Rows.Count > 0)
                {
                    int i = 0;
                    foreach (var column in Hdrtable.Rows)
                    {
                        HeaderFieldsList.Add(Hdrtable.Rows[i]["COLUMN_NAME"].ToString());
                        i += 1;
                    }
                }

                SelectedHdrList = MyXml.ReadXmlGridView(MyXml.XMLHdrFilePath);

                if (SelectedHdrList.Count > 0)
                {
                    foreach (var item in SelectedHdrList)
                        HeaderList.Add(item);
                }
                else
                {
                    HeaderList.Add("LotBaleNumber");
                    HeaderList.Add("LotNumber");
                    HeaderList.Add("Weight");
                    HeaderList.Add("Moisture");
                    HeaderList.Add("Forte");
                    HeaderList.Add("TimeComplete");
                    HeaderList.Add("SerialNumber");
                    HeaderList.Add("StockName");
                    // HeaderList.Add("SourceID");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in GetHeaderList (DropLineChartViewModel) {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GetHeaderList (DropLineChartViewModel) < {ex.Message}");
            }
        }

        public string CurrentBaleTable
        {
            get
            {
                return _sqlhandler.GetCurrentBaleTableName();
            }
        }


        private void InitGraph()
        {
           
            SetUpGraphsColors();
            DropSamples = Settings.Default.SelectedDropCt;

            DropDatatable = new DataTable();
            HeaderFieldsList = new List<string>();
            HeaderList = new List<string>();

        }

        private void SetUpGraphsColors()
        {
           
            DropVisible = new Visibility[] { Visibility.Hidden, Visibility.Hidden, Visibility.Hidden, Visibility.Hidden, 
                Visibility.Hidden, Visibility.Hidden,Visibility.Hidden,Visibility.Hidden,Visibility.Hidden,Visibility.Hidden };

            for (int i = 0; i < ClassCommon.BaleInDrop; i++)
            {
                DropVisible[i] = Visibility.Visible;
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
        #endregion Timer

    }
}
