using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GraphMenuBar.ViewModels;
using GraphMenuBar.Views;
using AppServices;
using System.Threading;
using System.Data;
using Prism.Commands;
using DataFieldsSelect.Views;
using static AppServices.ClassApplicationService;
using ModDropProfileChart.Views;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.Runtime.Intrinsics.Arm;
using System.Xml;
using ScottPlot.ArrowShapes;

namespace ModDropProfileChart.ViewModels
{
    public class DropProfileChartViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;

        private readonly ClassSqlHandler _sqlhandler;
        private Task _timerTask;
        private PeriodicTimer _timer;
        private CancellationTokenSource _cts;

        private readonly ClassXml MyXml = new ClassXml();

        private long preIndex = 0;
        private long newIndex = 0;

        public static string ModuleName = "Profile of Bale Data in Each Drop";

        private DataTable BaleDatatable;
        private DataTable DatatableLine2;
        private List<string> HeaderList;
        private List<string> HeaderFieldsList;

        private int CurIndexL1 = 0;
        private int PreIndexL1 = 0;

        private int CurIndexL2 = 0;
        private int PreIndexL2 = 0;

        private const int iLine1 = 0;
        private const int iLine2 = 1;

      

        public int GraphHeightM = 0;
        public int GraphHeightW = 0;
        public int GraphXWidth = 30;

        public int GraphLowM = 0;
        public int GraphLowW = 0;


        // private DataTable Hdrtable;

        private string _Pos1Text;
        public string Pos1Text
        {
            get { return _Pos1Text; }
            set { SetProperty(ref _Pos1Text, value); }
        }

        private string _Pos2Text;
        public string Pos2Text
        {
            get { return _Pos2Text; }
            set { SetProperty(ref _Pos2Text, value); }
        }

        private string _Pos3Text;
        public string Pos3Text
        {
            get { return _Pos3Text; }
            set { SetProperty(ref _Pos3Text, value); }
        }

        private string _Pos4Text;
        public string Pos4Text
        {
            get { return _Pos4Text; }
            set { SetProperty(ref _Pos4Text, value); }
        }

        private string _Pos5Text;
        public string Pos5Text
        {
            get { return _Pos5Text; }
            set { SetProperty(ref _Pos5Text, value); }
        }

        private string _Pos6Text;
        public string Pos6Text
        {
            get { return _Pos6Text; }
            set { SetProperty(ref _Pos6Text, value); }
        }

        private string _Pos7Text;
        public string Pos7Text
        {
            get { return _Pos7Text; }
            set { SetProperty(ref _Pos7Text, value); }
        }

        private string _Pos8Text;
        public string Pos8Text
        {
            get { return _Pos8Text; }
            set { SetProperty(ref _Pos8Text, value); }
        }


        private int _txtBoxFour;
        public int TxtBoxFour
        {
            get { return _txtBoxFour; }
            set { SetProperty(ref _txtBoxFour, value); }
        }

        private int _totalDropInGraph = ClassCommon.DropInChart;
        public int TotalDropInGraph
        {
            get { return _totalDropInGraph; }
            set { SetProperty(ref _totalDropInGraph, value); }
        }

        private int _balesInDrop = ClassCommon.BaleInDrop;
        public int BalesInDrop
        {
            get { return _balesInDrop; }
            set { SetProperty(ref _balesInDrop, value); }
        }

        private int _showDropInListView = 2;
        public int ShowDropInListView
        {
            get { return _showDropInListView; }
            set { SetProperty(ref _showDropInListView, value); }
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

        private bool _PickTabOne = true;
        public bool PickTabOne
        {
            get { return _PickTabOne; }
            set { SetProperty(ref _PickTabOne, value); }
        }

        private bool _PickTabTwo;
        public bool PickTabTwo
        {
            get { return _PickTabTwo; }
            set { SetProperty(ref _PickTabTwo, value); }
        }

        public UserControl TopMenuOneBar
        {
            get => new MenuBarView(_eventAggregator);
        }

        private int _BalePositionL1;
        public int BalePosition
        {
            get { return _BalePositionL1; }
            set { SetProperty(ref _BalePositionL1, value); }
        }


        private string _DropNumber;
        public string DropNumber
        {
            get { return _DropNumber; }
            set { SetProperty(ref _DropNumber, value); }
        }

        private string _AverageHeader;
        public string AverageHeader
        {
            get { return _AverageHeader; }
            set { SetProperty(ref _AverageHeader, value); }
        }



        private string _curMoistureHdr = ClassCommon.MoistureTypeLst[ClassCommon.MoistureType];  //"Current Moisture";
        public string CurMoistureHdr
        {
            get { return _curMoistureHdr; }
            set { SetProperty(ref _curMoistureHdr, value); }
        }

        private string _curWeightHdr = ClassCommon.WeightTypeLst[ClassCommon.WeightUnit]; //"Current Bale Weight";
        public string CurWeightHdr
        {
            get { return _curWeightHdr; }
            set { SetProperty(ref _curWeightHdr, value); }
        }



        private List<string> _LineList;
        public List<string> LineList
        {
            get { return _LineList; }
            set { SetProperty(ref _LineList, value); }
        }

        private List<string> _SourceList;
        public List<string> SourceList
        {
            get { return _SourceList; }
            set { SetProperty(ref _SourceList, value); }
        }

        /// <summary>
        /// For Big Numbers
        /// </summary>
        private ObservableCollection<string> _avgMoisture;
        public ObservableCollection<string> AvgMoisture
        {
            get { return _avgMoisture; }
            set { SetProperty(ref _avgMoisture, value); }
        }

        private ObservableCollection<string> _AvgWeight;
        public ObservableCollection<string> AvgWeight
        {
            get { return _AvgWeight; }
            set { SetProperty(ref _AvgWeight, value); }
        }

        private List<RemoteProfile> _itemListL1;
        public List<RemoteProfile> ItemListL1
        {
            get { return _itemListL1; }
            set { SetProperty(ref _itemListL1, value); }
        }

        private List<RemoteProfile> _itemListL2;
        public List<RemoteProfile> ItemListL2
        {
            get { return _itemListL2; }
            set { SetProperty(ref _itemListL2, value); }
        }


        private List<string> _selectedItemList;
        public List<string> SelectedHdrList
        {
            get { return _selectedItemList; }
            set { SetProperty(ref _selectedItemList, value); }
        }

        private ObservableCollection<CheckedListItem> _AvailableHdrList;
        public ObservableCollection<CheckedListItem> AvailableHdrList
        {
            get { return _AvailableHdrList; }
            set { SetProperty(ref _AvailableHdrList, value); }
        }


        //Number of Drops in chart 3 to 5 only.
        private List<int> _dropList = new List<int>() { 3, 4, 5 };
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


        private int _selectedDropCt = ClassCommon.DropInChart;
        public int SelectedDropCt
        {
            get { return _selectedDropCt; }
            set
            {
                SetProperty(ref _selectedDropCt, value);
                if (value > 0)
                {
                    ClassCommon.DropInChart = value;
                }
            }
        }


        public DropProfileChartViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
        
            string ChartTitle = $"Bar Chart of {ClassCommon.DropInChart} Consecutive Drops, Newest Drop on the left";

            _timer = new PeriodicTimer(TimeSpan.FromSeconds(ClassCommon.ScanSec));

            AvgMoisture = new ObservableCollection<string>();
            AvgWeight = new ObservableCollection<string>();

            BaleDatatable = new DataTable();
            DatatableLine2 = new DataTable();
            _sqlhandler = ClassSqlHandler.Instance;

            SelectedHdrList = new List<string>();
            AvailableHdrList = new ObservableCollection<CheckedListItem>();
            HeaderList = new List<string>();
            HeaderFieldsList = new List<string>();

            LineList = new List<string>();
            SourceList = new List<string>();

            string tablename = _sqlhandler.GetCurrentBaleTableName();

            LineList = _sqlhandler.GetUniquIntitemlist("LineName", tablename);
            SourceList = _sqlhandler.GetUniquIntitemlist("SourceName", tablename);


           // DropProfileChartView._dropProfileChartView.SetUpChart(ChartTitle, GraphHeightM, GraphHeightW);

            if (LineList.Count > 0)
                for (int i = 0; i < LineList.Count; i++)
                {
                    AvgMoisture.Add("0.00");
                    AvgWeight.Add("0.00");
                }

            GetHeaderList();
            Set_ListviewGrid();


            if (ClassCommon.DropHitoLow)
            {
                Pos1Text = "1";
                Pos2Text = "2";
                Pos3Text = "3";
                Pos4Text = "4";
                Pos5Text = "5";
                Pos6Text = "6";
                Pos7Text = "7";
                Pos8Text = "8";


            }
            else
            {
                switch (ClassCommon.BaleInDrop)
                {
                    case 3:
                        Pos1Text = "3";
                        Pos2Text = "2";
                        Pos3Text = "1";
                        Pos4Text = "";
                        Pos5Text = "";
                        Pos6Text = "";
                        Pos7Text = "";
                        Pos8Text = "";
                        break;
                    case 4:
                        Pos1Text = "4";
                        Pos2Text = "3";
                        Pos3Text = "2";
                        Pos4Text = "1";
                        Pos5Text = "";
                        Pos6Text = "";
                        Pos7Text = "";
                        Pos8Text = "";
                        break;
                    case 5:
                        Pos1Text = "5";
                        Pos2Text = "4";
                        Pos3Text = "3";
                        Pos4Text = "2";
                        Pos5Text = "1";
                        Pos6Text = "";
                        Pos7Text = "";
                        Pos8Text = "";
                        break;
                    case 6:
                        Pos1Text = "6";
                        Pos2Text = "5";
                        Pos3Text = "4";
                        Pos4Text = "3";
                        Pos5Text = "2";
                        Pos6Text = "1";
                        Pos7Text = "";
                        Pos8Text = "";
                        break;
                    case 7:
                        Pos1Text = "7";
                        Pos2Text = "6";
                        Pos3Text = "5";
                        Pos4Text = "4";
                        Pos5Text = "3";
                        Pos6Text = "2";
                        Pos7Text = "1";
                        Pos8Text = "";
                        break;
                    case 8:
                        Pos1Text = "8";
                        Pos2Text = "7";
                        Pos3Text = "6";
                        Pos4Text = "5";
                        Pos5Text = "4";
                        Pos6Text = "3";
                        Pos7Text = "2";
                        Pos8Text = "1";
                        break;
                }


            }
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
            DropProfileChartView._dropProfileChartView?.ShowStartBtn(false);
            DropProfileChartView._dropProfileChartView?.ShowStopBtn(true);
        }

        private DelegateCommand _stopCommand;
        public DelegateCommand StopCommand =>
        _stopCommand ?? (_stopCommand = new DelegateCommand(StopExecute).ObservesCanExecute(() => RTRunning));

        private void StopExecute()
        {
            RTRunning = false;

            _ = StopAsync();

            _eventAggregator.GetEvent<UpdateAppRunEvents>().Publish(false);

            DropProfileChartView._dropProfileChartView?.ShowStartBtn(true);
            DropProfileChartView._dropProfileChartView?.ShowStopBtn(false);
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

        private async Task UpdateDataAsync()
        {

            int iFirstBale;

            string curTable = CurrentBaleTable; 
            string strGetIndex = string.Empty;
            string strGetSingleNewData = string.Empty;

            string ChartTitle = $"Bar Chart of {ClassCommon.DropInChart} Consecutive Drops, Newest Drop on the left";

            DropProfileChartView._dropProfileChartView.Clearbale();

            string LineID = string.Empty;
            if (PickTabOne)
            {
                LineID = "LineId=1";
            }
            else if (PickTabTwo)
            {
                LineID = "LineId=2";
            }

            try
            {
                strGetIndex = $"Select Top 1 [index] FROM [ForteData].[dbo].[{curTable}] WHERE {LineID} ORDER BY [TimeStart] DESC; ";

                strGetSingleNewData = "Select Top 1 " +
                                       " Position, Weight, Forte, Moisture, UpCount, DownCount, DropNumber, NetWeight, BDWeight, BasisWeight, LotBaleNumber,SerialNumber,DropNumber,[Index] FROM dbo.[" +
                                        curTable +
                                        "] WHERE " + LineID + " And Position > 0 ORDER BY [TimeStart] DESC;";


                DataTable SingleDataTable = await _sqlhandler.GetBaleArchiveDataTableAsyn(strGetSingleNewData);

                newIndex = Convert.ToInt32(SingleDataTable.Rows[0]["Index"].ToString());

                BalePosition = Convert.ToInt32(SingleDataTable.Rows[0]["Position"].ToString());

                if (preIndex != newIndex)
                {
                    DropProfileChartView._dropProfileChartView.MoveBaleOne(BalePosition);
                    preIndex = newIndex;

                    if (ClassCommon.DropHitoLow)
                        iFirstBale = 1;
                    else
                        iFirstBale = ClassCommon.BaleInDrop;

                    if (BalePosition == iFirstBale)
                    {
                        {
                            BaleDatatable.Clear();
                            CurIndexL1 = _sqlhandler.GetIntNewItemData(strGetIndex);

                            if (CurIndexL1 != PreIndexL1) // not for the same bale!
                            {
                                string newquery = BuildQueryString(1);
                                BaleDatatable = await _sqlhandler.GetBaleArchiveDataTableAsyn(newquery);

                                if (BaleDatatable.Rows.Count > 0)
                                {
                                    if (BaleDatatable.Columns.Contains("index"))
                                        BaleDatatable.Columns.Remove("index");

                                    PreIndexL1 = CurIndexL1;

                                    MoveL1ListViewDown(ItemListL1, HeaderList.Count);

                                    ClassCommon.UpdataTableUnits(BaleDatatable);

                                    _ = UpdateListView(BaleDatatable);

                                    UpdateBigNumbers(BaleDatatable, iLine1);

                                    DropProfileChartView._dropProfileChartView.PlotChart(BaleDatatable,ChartTitle,GraphHeightM,GraphHeightW, GraphLowM, GraphLowW);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in UpdateDataAsync (DropProfile) {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in UpdateDataAsync (DropProfile) < {ex.Message}");
            }
        }


        private void UpdateBigNumbers(DataTable baledatatable, int iLine)
        {
            double MoistureAvg;
            double WeightAvg;

            List<double> MoistureAvgLst =  new();
            List<double> WeightAvgLst= new();

            MoistureAvgLst.Clear();
            WeightAvgLst.Clear();

            try
            {
                if (baledatatable.Rows.Count > 0)
                {
                    for (int i = 0; i < ClassCommon.BaleInDrop; i++)
                    {
                        MoistureAvgLst.Add(baledatatable.Rows[i].Field<float>("Moisture"));
                        WeightAvgLst.Add(Convert.ToDouble(baledatatable.Rows[i]["Weight"].ToString()));

                        DropNumber = baledatatable.Rows[i]["DropNumber"].ToString();
                    }

                    MoistureAvg = MoistureAvgLst.Average();
                    WeightAvg = WeightAvgLst.Average();
                    AvgMoisture[iLine] = MoistureAvg.ToString("#0.00");
                    AvgWeight[iLine] = WeightAvg.ToString("#0.00");

                    GraphLowM = (int)MoistureAvgLst.Min();
                    GraphLowW = (int)WeightAvgLst.Min();

                    GraphHeightM = (int)Math.Round(MoistureAvg);
                    GraphHeightW = (int)Math.Round(WeightAvg);

                    

                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in UpdateBigNumbers (DropProfile) {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in UpdateBigNumbers (DropProfile) < {ex.Message}");
            }
        }

        private async Task UpdateListView(DataTable newData)
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (ClassCommon.DropHitoLow)
                    {
                        //from left to right
                        for (int i = 1; i > ClassCommon.BaleInDrop; i++)
                        {
                            await Task.Delay(500);
                            UpdateProfileTable(i, newData);
                        }
                    }
                    else
                    {
                        //from right to left
                        for (int i = ClassCommon.BaleInDrop; i > 0; i--)
                        {
                            await Task.Delay(500);
                            UpdateProfileTable(i, newData);
                        }
                    }
                }
                catch (Exception ex )
                {
                    System.Windows.MessageBox.Show($"Error in UpdateListView (DropProfile) {ex.Message}");
                    ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in UpdateListView (DropProfile) < {ex.Message}");
                }
            });
        }

        private void UpdateProfileTable(int idx, DataTable newData)
        {
            string strFormat = "HH:mm:ss";
            try
            {
                if (newData != null)
                {
                    switch (idx)
                    {
                        case 1:

                            for (int i = 0; i < HeaderList.Count; i++)
                            {
                                if ((newData.Rows[0][HeaderList[i]].GetType().Name == "Single") || (newData.Rows[0][HeaderList[i]].GetType().Name == "Double"))
                                {
                                    double dTemp = Convert.ToDouble(newData.Rows[0][HeaderList[i]].ToString());
                                    ItemListL1[i].GvCol1 = dTemp.ToString("#0.0#");
                                }
                                else if (newData.Rows[0][HeaderList[i]].GetType().FullName == "System.DateTime")
                                    ItemListL1[i].GvCol1 = Convert.ToDateTime(newData.Rows[0][HeaderList[i]]).ToString(strFormat);
                                else
                                    ItemListL1[i].GvCol1 = newData.Rows[0][HeaderList[i]].ToString();
                            }
                            break;

                        case 2:
                            for (int i = 0; i < HeaderList.Count; i++)
                            {
                                if ((newData.Rows[1][HeaderList[i]].GetType().Name == "Single") || (newData.Rows[1][HeaderList[i]].GetType().Name == "Double"))
                                {
                                    double dTemp = Convert.ToDouble(newData.Rows[1][HeaderList[i]].ToString());
                                    ItemListL1[i].GvCol2 = dTemp.ToString("#0.0#");
                                }
                                else if (newData.Rows[1][HeaderList[i]].GetType().FullName == "System.DateTime")
                                    ItemListL1[i].GvCol2 = Convert.ToDateTime(newData.Rows[1][HeaderList[i]]).ToString(strFormat);
                                else
                                    ItemListL1[i].GvCol2 = newData.Rows[1][HeaderList[i]].ToString();

                            }
                            break;

                        case 3:
                            for (int i = 0; i < HeaderList.Count; i++)
                            {
                                if ((newData.Rows[2][HeaderList[i]].GetType().Name == "Single") || (newData.Rows[2][HeaderList[i]].GetType().Name == "Double"))
                                {
                                    double dTemp = Convert.ToDouble(newData.Rows[2][HeaderList[i]].ToString());
                                    ItemListL1[i].GvCol3 = dTemp.ToString("#0.0#");
                                }
                                else if (newData.Rows[2][HeaderList[i]].GetType().FullName == "System.DateTime")
                                    ItemListL1[i].GvCol3 = Convert.ToDateTime(newData.Rows[2][HeaderList[i]]).ToString(strFormat);
                                else
                                    ItemListL1[i].GvCol3 = newData.Rows[2][HeaderList[i]].ToString();
                            }
                            break;

                        case 4:
                            for (int i = 0; i < HeaderList.Count; i++)
                            {
                                if ((newData.Rows[3][HeaderList[i]].GetType().Name == "Single") || (newData.Rows[3][HeaderList[i]].GetType().Name == "Double"))
                                {
                                    double dTemp = Convert.ToDouble(newData.Rows[3][HeaderList[i]].ToString());
                                    ItemListL1[i].GvCol4 = dTemp.ToString("#0.0#");
                                }
                                else if (newData.Rows[3][HeaderList[i]].GetType().FullName == "System.DateTime")
                                    ItemListL1[i].GvCol4 = Convert.ToDateTime(newData.Rows[3][HeaderList[i]]).ToString(strFormat);
                                else
                                    ItemListL1[i].GvCol4 = newData.Rows[3][HeaderList[i]].ToString();
                            }
                            break;

                        case 5:
                            for (int i = 0; i < HeaderList.Count; i++)
                            {
                                if ((newData.Rows[4][HeaderList[i]].GetType().Name == "Single") || (newData.Rows[4][HeaderList[i]].GetType().Name == "Double"))
                                {
                                    double dTemp = Convert.ToDouble(newData.Rows[4][HeaderList[i]].ToString());
                                    ItemListL1[i].GvCol5 = dTemp.ToString("#0.0#");
                                }
                                else if (newData.Rows[4][HeaderList[i]].GetType().FullName == "System.DateTime")
                                    ItemListL1[i].GvCol5 = Convert.ToDateTime(newData.Rows[4][HeaderList[i]]).ToString(strFormat);
                                else
                                    ItemListL1[i].GvCol5 = newData.Rows[4][HeaderList[i]].ToString();
                            }
                            break;

                        case 6:
                            for (int i = 0; i < HeaderList.Count; i++)
                            {
                                if ((newData.Rows[5][HeaderList[i]].GetType().Name == "Single") || (newData.Rows[5][HeaderList[i]].GetType().Name == "Double"))
                                {
                                    double dTemp = Convert.ToDouble(newData.Rows[5][HeaderList[i]].ToString());
                                    ItemListL1[i].GvCol6 = dTemp.ToString("#0.0#");
                                }
                                else if (newData.Rows[5][HeaderList[i]].GetType().FullName == "System.DateTime")
                                    ItemListL1[i].GvCol6 = Convert.ToDateTime(newData.Rows[5][HeaderList[i]]).ToString(strFormat);
                                else
                                    ItemListL1[i].GvCol6 = newData.Rows[5][HeaderList[i]].ToString();
                            }
                            break;

                        case 7:
                            for (int i = 0; i < HeaderList.Count; i++)
                            {
                                if ((newData.Rows[6][HeaderList[i]].GetType().Name == "Single") || (newData.Rows[6][HeaderList[i]].GetType().Name == "Double"))
                                {
                                    double dTemp = Convert.ToDouble(newData.Rows[6][HeaderList[i]].ToString());
                                    ItemListL1[i].GvCol7 = dTemp.ToString("#0.0#");
                                }
                                else if (newData.Rows[6][HeaderList[i]].GetType().FullName == "System.DateTime")
                                    ItemListL1[i].GvCol7 = Convert.ToDateTime(newData.Rows[6][HeaderList[i]]).ToString(strFormat);
                                else
                                    ItemListL1[i].GvCol7 = newData.Rows[6][HeaderList[i]].ToString();
                            }
                            break;

                        case 8:
                            for (int i = 0; i < HeaderList.Count; i++)
                            {
                                if ((newData.Rows[7][HeaderList[i]].GetType().Name == "Single") || (newData.Rows[7][HeaderList[i]].GetType().Name == "Double"))
                                {
                                    double dTemp = Convert.ToDouble(newData.Rows[7][HeaderList[i]].ToString());
                                    ItemListL1[i].GvCol8 = dTemp.ToString("#0.0#");
                                }
                                else if (newData.Rows[7][HeaderList[i]].GetType().FullName == "System.DateTime")
                                    ItemListL1[i].GvCol8 = Convert.ToDateTime(newData.Rows[7][HeaderList[i]]).ToString(strFormat);
                                else
                                    ItemListL1[i].GvCol8 = newData.Rows[7][HeaderList[i]].ToString();
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in UpdateProfileTable (DropProfile) {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in UpdateProfileTable (DropProfile) < {ex.Message}");

            }
        }

        private void Set_ListviewGrid()
        {
            ItemListL1 = new List<RemoteProfile>();

            for (int i = 0; i < HeaderList.Count * 2 + 2; i++)
            {
                ItemListL1.Add(new RemoteProfile() { RowsName = "", GvCol1 = "", GvCol2 = "", GvCol3 = "", GvCol4 = "", GvCol5 = "", GvCol6 = "", GvCol7 = "", GvCol8 = "" });
            }

            for (int i = 0; i < HeaderList.Count; i++)
            {
                ItemListL1[i].RowsName = HeaderList[i];
            }
            ItemListL1[HeaderList.Count].RowsName = "";

            for (int i = 0; i < HeaderList.Count; i++)
            {
                ItemListL1[HeaderList.Count + 1 + i].RowsName = HeaderList[i];
            }

            //For Line 2
            ItemListL2 = new List<RemoteProfile>();

            for (int i = 0; i < HeaderList.Count * 2 + 2; i++)
            {
                ItemListL2.Add(new RemoteProfile() { RowsName = "", GvCol1 = "", GvCol2 = "", GvCol3 = "", GvCol4 = "", GvCol5 = "", GvCol6 = "", GvCol7 = "", GvCol8 = "" });
            }

            for (int i = 0; i < HeaderList.Count; i++)
            {
                ItemListL2[i].RowsName = HeaderList[i];
            }
            ItemListL2[HeaderList.Count].RowsName = "";

            for (int i = 0; i < HeaderList.Count; i++)
            {
                ItemListL2[HeaderList.Count + 1 + i].RowsName = HeaderList[i];
            }

        }

        public string CurrentBaleTable
        {
            get
            {
                return _sqlhandler.GetCurrentBaleTableName();
            }
        }

        private string BuildQueryString(int LineID)
        {
            string strtemp = string.Empty;
            string strList = string.Empty;
            char charsToTrim = ',';
            string strLineSeleted = "WHERE LineId = " + LineID.ToString(); //_dropModel.m_Line;

            int IntColSamples = ClassCommon.DropInChart * ClassCommon.BaleInDrop; //30

            try
            {

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

                return strtemp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in ViewModel BuildQueryString " + ex.Message);
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
                MessageBox.Show("ERROR In GetHeaderList " + ex);
            }
        }


        private void MoveL1ListViewDown(List<RemoteProfile> ItemList, int HdrCount)
        {
            int y = HdrCount + 1;

            try
            {
                for (int i = 0; i < HdrCount; i++)
                {
                    ItemList[y + i].GvCol1 = ItemList[i].GvCol1;
                    ItemList[y + i].GvCol2 = ItemList[i].GvCol2;
                    ItemList[y + i].GvCol3 = ItemList[i].GvCol3;
                    ItemList[y + i].GvCol4 = ItemList[i].GvCol4;
                    ItemList[y + i].GvCol5 = ItemList[i].GvCol5;
                    ItemList[y + i].GvCol6 = ItemList[i].GvCol6;
                    ItemList[y + i].GvCol7 = ItemList[i].GvCol7;
                    ItemList[y + i].GvCol8 = ItemList[i].GvCol8;
                    // Clear Top Rows
                    ItemList[i].GvCol1 = "";
                    ItemList[i].GvCol2 = "";
                    ItemList[i].GvCol3 = "";
                    ItemList[i].GvCol4 = "";
                    ItemList[i].GvCol5 = "";
                    ItemList[i].GvCol6 = "";
                    ItemList[i].GvCol7 = "";
                    ItemList[i].GvCol8 = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in MoveL1ListViewDown " + ex.Message);
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

    public class RemoteProfile : BindableBase
    {
        private string _Headers;
        public string RowsName
        {
            get { return _Headers; }
            set { SetProperty(ref _Headers, value); }
        }

        private string _pos1;
        public string GvCol1
        {
            get { return _pos1; }
            set { SetProperty(ref _pos1, value); }
        }

        private string _pos2;
        public string GvCol2
        {
            get { return _pos2; }
            set { SetProperty(ref _pos2, value); }
        }

        private string _pos3;
        public string GvCol3
        {
            get { return _pos3; }
            set { SetProperty(ref _pos3, value); }
        }

        private string _pos4;
        public string GvCol4
        {
            get { return _pos4; }
            set { SetProperty(ref _pos4, value); }
        }

        private string _pos5;
        public string GvCol5
        {
            get { return _pos5; }
            set { SetProperty(ref _pos5, value); }
        }

        private string _pos6;
        public string GvCol6
        {
            get { return _pos6; }
            set { SetProperty(ref _pos6, value); }
        }

        private string _pos7;
        public string GvCol7
        {
            get { return _pos7; }
            set { SetProperty(ref _pos7, value); }
        }

        private string _pos8;
        public string GvCol8
        {
            get { return _pos8; }
            set { SetProperty(ref _pos8, value); }
        }

        private string _pos9;
        public string GvCol9
        {
            get { return _pos9; }
            set { SetProperty(ref _pos9, value); }
        }

        private string _pos10;
        public string GvCol10
        {
            get { return _pos10; }
            set { SetProperty(ref _pos10, value); }
        }

        private string _pos11;
        public string GvCol11
        {
            get { return _pos11; }
            set { SetProperty(ref _pos11, value); }
        }

        private string _pos12;
        public string GvCol12
        {
            get { return _pos12; }
            set { SetProperty(ref _pos12, value); }
        }
    }
}
