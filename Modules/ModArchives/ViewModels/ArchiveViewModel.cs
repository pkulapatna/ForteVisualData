using AppServices;
using AppServices.Control;
using CSVReports.Views;
using DataFieldsSelect.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Intrinsics.Arm;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UcGraph.Views;
using static AppServices.ClassApplicationService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ModArchives.ViewModels
{
    public class ArchiveViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;
        private bool _pageloaded = false;

        public static string ModuleName = "Archives Data from SQL Server";

        private readonly ClassSqlHandler _sqlhandler;
       // private readonly ClassXml MyXml = new ClassXml();
        private string AllFieldsName = string.Empty;
        private string CustomFieldSelect = string.Empty;

        private int _totalcount;
        public int Totalcount
        {
            get => _totalcount; 
            set => SetProperty(ref _totalcount, value); 
        }

        private List<string> _monthtableList ; //
        public List<string> MonthTableList
        {
            get { return _monthtableList; }
            set { SetProperty(ref _monthtableList, value); }
        }

        private int _selecttableindex;
        public int SelectTableIndex
        {
            get { return _selecttableindex; }
            set { SetProperty(ref _selecttableindex, value); }
        }

        private string _selectTableValue;
        public string SelectTableValue
        {
            get => _selectTableValue;
            set 
            { 
                SetProperty(ref _selectTableValue, value);

                string strMonth = SelectTableValue.Substring(11, 3);
                string strYear = SelectTableValue.Substring(14, 2);
                SelectedMonth = strMonth + strYear;
                string startDate = "01" + "/" + strMonth + "/" + strYear;
                DateTime.TryParse(startDate, out DateTime dateValue);
            }
        }
        public string SelectedMonth { get; set; }

     
        //Stock //---------------------------------------------------------------------------- 

        private List<string> _stockList = new List<string>();
        public List<string> StockList
        {
            get { return _stockList; }
            set { SetProperty(ref _stockList, value); }
        }

        private bool _stockChecked;
        public bool StockChecked
        {
            get { return _stockChecked; }
            set
            {
                SetProperty(ref _stockChecked, value);
                if (value)
                {
                    if (iTabSelected == 1) 
                    {
                        StockList = _sqlhandler.GetUniquIntitemlist("StockName", SelectTableValue);
                        StockIndex = 0;
                    }
                     
                    //if (iTabSelected == 2) { }
                    //   StockList = _sqlhandler.GetSqlStockList(SelectLotTableValue);
                }
                else
                {
                   StockList = null;
                }
            }
        }
        private int _stockIndex = 0;
        public int StockIndex
        {
            get { return _stockIndex; }
            set { SetProperty(ref _stockIndex, value); }
        }
        public string StockSelected { get; set; }


        //Grade  //--------------------------------------------------------------------------

        private List<string> _gradeList = new List<string>();
        public List<string> GradeList
        {
            get { return _gradeList; }
            set { SetProperty(ref _gradeList, value); }
        }

        private bool _gradeChecked;
        public bool GradeChecked
        {
            get { return _gradeChecked; }
            set
            {
                SetProperty(ref _gradeChecked, value);

                if (value)
                {
                    if (iTabSelected == 1)
                    {
                        GradeList = _sqlhandler.GetUniquIntitemlist("GradeName", SelectTableValue);
                        GradeIndex = 0;
                    }
                }
                else
                {
                    GradeList = null;
                }
            }

        }
        private int _gradeIndex = 0;
        public int GradeIndex
        {
            get { return _gradeIndex; }
            set { SetProperty(ref _gradeIndex, value); }
        }
        public string GradeSelected { get; set; }



        //Line
        private List<string> _lineList = new List<string>();
        public List<string> LineList
        {
            get { return _lineList; }
            set { SetProperty(ref _lineList, value); }
        }
        private bool _lineChecked;
        public bool LineChecked
        {
            get { return _lineChecked; }
            set
            {
                SetProperty(ref _lineChecked, value);

                if(value)
                {
                    if (iTabSelected == 1)
                    {
                        LineList = _sqlhandler.GetUniquIntitemlist("LineName", SelectTableValue);
                        LineIndex = 0;
                    }
                }
                else
                {
                    LineList = null;
                }
            }
        }
        private int _lineIndex = 0;
        public int LineIndex
        {
            get { return _lineIndex; }
            set { SetProperty(ref _lineIndex, value); }
        }
        public string LineSelected { get; set; }
        //Source
        private List<string> _sourceList = new List<string>();
        public List<string> SourceList
        {
            get { return _sourceList; }
            set { SetProperty(ref _sourceList, value); }
        }

        private bool _sourceChecked;
        public bool SourceChecked
        {
            get { return _sourceChecked; }
            set
            {
                SetProperty(ref _sourceChecked, value);

                if (value)
                {
                    if (iTabSelected == 1)
                    {
                        SourceList = _sqlhandler.GetUniquIntitemlist("SourceName", SelectTableValue);
                        SourceIndex = 0;
                    }
                }
                else
                    SourceList = null;
            }
        }

        private int _sourceIndex = 0;
        public int SourceIndex
        {
            get { return _sourceIndex; }
            set { SetProperty(ref _sourceIndex, value); }
        }
        public string SourceSelected { get; set; }


        private int itemchecked = 0;

        private List<string> _occrlist;
        public List<string> Occrlist
        {
            get { return _occrlist; }
            set { SetProperty(ref _occrlist, value); }
        }

        private List<string> _sortOrder = new List<string>() { "Newest", "Oldest" };
        public List<string> SortOrder
        {
            get { return _sortOrder; }
            set { SetProperty(ref _sortOrder, value); }
        }

        private int _selectSortOrder = 0;
        public int SelectSortOrder
        {
            get { return _selectSortOrder; }
            set { SetProperty(ref _selectSortOrder, value); }
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
                return _endQueryDate;
            }
            set { SetProperty(ref _endQueryDate, value); }
        }


        private Visibility _cusDataBoxVis;
        public Visibility CusDataBoxVis
        {
            get { return _cusDataBoxVis; }
            set { SetProperty(ref _cusDataBoxVis, value); }
        }

        private Visibility _customFieldVis;
        public Visibility CustomFieldVis
        {
            get { return _customFieldVis; }
            set { SetProperty(ref _customFieldVis, value); }
        }

     

        private bool _allDataCheck;
        public bool AllDataCheck
        {
            get { return _allDataCheck; }
            set
            {
                SetProperty(ref _allDataCheck, value);
                if (value) CusDataBoxVis = Visibility.Hidden;
            }
        }
        private bool _customDataCheck;
        public bool CustomDataCheck
        {
            get { return _customDataCheck; }
            set
            {
                SetProperty(ref _customDataCheck, value);
                if (value) CusDataBoxVis = Visibility.Visible;
            }
        }

        private int _recCount = 200;
        public int RecCount
        {
            get => _recCount; 
            set => SetProperty(ref _recCount, value); 
        }

        private bool _dataSelEnable;
        public bool DataSelEnable
        {
            get { return _dataSelEnable; }
            set
            {
                SetProperty(ref _dataSelEnable, value);
            }
        }
        private bool _sortOrdEnable;
        public bool SortOrdEnable
        {
            get { return _sortOrdEnable; }
            set { SetProperty(ref _sortOrdEnable, value); }
        }


        private bool _MonthChecked;
        public bool MonthChecked
        {
            get { return _MonthChecked; }
            set
            {
                SetProperty(ref _MonthChecked, value);
                DataSelEnable = true;
                SortOrdEnable = true;
            }
        }

        private bool _cusFieldChecked;
        public bool CusFieldChecked
        {
            get { return _cusFieldChecked; }
            set
            {
                SetProperty(ref _cusFieldChecked, value);
                CustomFieldVis = (value == true) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private bool _allFieldsChecked;
        public bool AllFieldsChecked
        {
            get { return _allFieldsChecked; }
            set
            {
                SetProperty(ref _allFieldsChecked, value);
                if (value) CustomFieldVis = Visibility.Hidden;
            }
        }


        private bool _daychecked;
        public bool DayChecked
        {
            get { return _daychecked; }
            set
            {
                SetProperty(ref _daychecked, value);
                if (value)
                {
                    AllDataCheck = true;
                    SelectSortOrder = 0;
                    MonthChecked = false;
                    DataSelEnable = false;
                    SortOrdEnable = false;
                }
            }
        }


        private bool _queryOn;
        public bool QueryOn
        {
            get { return _queryOn; }
            set { SetProperty(ref _queryOn, value); }
        }

        private double _wrtopc;
        public double WriteOpc
        {
            get { return _wrtopc; }
            set { SetProperty(ref _wrtopc, value); }
        }

        private int iTabSelected = 0;

        private bool _SelectBaleTab;
        public bool SelectBaleTab
        {
            get => _SelectBaleTab;
            set 
            { 
                SetProperty(ref _SelectBaleTab, value); 
                if(value)
                {
                    iTabSelected = 1;
                    QueryOn = false;
                    WriteOpc = 0.2;
                    MonthTableList = _sqlhandler.GettableList(ClassSqlHandler.BALE_ARCHIVE);
                    BLotDataexcist = false;
                    BBaleDataexcist = false;

                }
            }
        }

        private bool _SelectLotTab;
        public bool SelectLotTab
        {
            get => _SelectLotTab;
            set
            {
                SetProperty(ref _SelectLotTab, value);
                if (value)
                {
                    iTabSelected = 2;
                    QueryOn = false;
                    WriteOpc = 0.2;
                    LotMonthTableList = _sqlhandler.GettableList(ClassSqlHandler.LOT_ARCHIVE);
                    BLotDataexcist = false;
                    BBaleDataexcist = false;
                }
            }
        }

        private int _selectedIdx;
        public int SelectedIdx
        {
            get => _selectedIdx; 
            set 
            { 
                SetProperty(ref _selectedIdx, value); 
                if (value > -1)
                {
                  //  MessageBox.Show($"Selected tab {value}");
                }
            }
        }


        //SelectLotTab/////////////////////////////////////////////////////////////////////////////////////
        //

        private string AllLotFieldsName = string.Empty;

        private List<string> RemoveLotColumnList;
        private List<string> RemoveBaleColumnList;

        private bool _LotTabEnable = true;
        public bool LotTabEnable
        {
            get { return _LotTabEnable; }
            set { SetProperty(ref _LotTabEnable, value); }
        }


        private List<string> _lotMonthtableList; //
        public List<string> LotMonthTableList
        {
            get { return _lotMonthtableList; }
            set { SetProperty(ref _lotMonthtableList, value); }
        }

        private bool _lotMonthChecked;
        public bool LotMonthChecked
        {
            get { return _lotMonthChecked; }
            set
            {
                SetProperty(ref _lotMonthChecked, value);
                //DataSelEnable = true;
                //SortOrdEnable = true;
            }
        }

        private int _selectLotTableIndex;
        public int SelectLotTableIndex
        {
            get { return _selectLotTableIndex; }
            set { SetProperty(ref _selectLotTableIndex, value); }
        }

        private bool _lotDayChecked;
        public bool LotDayChecked
        {
            get { return _lotDayChecked; }
            set
            {
                SetProperty(ref _lotDayChecked, value);
                if (value)
                {
                        AllLotDataCheck = true;
                    //  SelectSortOrder = 0;
                        LotMonthChecked = false;
                    //  DataSelEnable = false;
                    //  SortOrdEnable = false;
                }
            }
        }


        private string _selectLotTableValue;
        public string SelectLotTableValue
        {
            get => _selectLotTableValue;
            set
            {
                SetProperty(ref _selectLotTableValue, value);

                if(value != null)
                {
                    string strMonth = value.Substring(10, 3);
                    string strYear = value.Substring(13, 2);
                    SelectedLotMonth = value.Substring(10, 5);// strMonth + strYear;
                    string startDate = "01" + "/" + strMonth + "/" + strYear;
                    DateTime.TryParse(startDate, out DateTime dateValue);
                }
            }
        }

        public string SelectedLotMonth { get; set; }

        private Nullable<DateTime> _startLotQueryDate = null;
        public Nullable<DateTime> StartLotQueryDate
        {
            get
            {
                if (_startLotQueryDate == null)
                    _startLotQueryDate = DateTime.Today;
                return _startLotQueryDate;
            }
            set { SetProperty(ref _startLotQueryDate, value); }
        }

        private Nullable<DateTime> _endLotQueryDate = null;
        public Nullable<DateTime> EndLotQueryDate
        {
            get
            {
                if (_endLotQueryDate == null)
                    _endLotQueryDate = DateTime.Today;
                return _endLotQueryDate;
            }
            set { SetProperty(ref _endLotQueryDate, value); }
        }


        private bool _allLotDataCheck;
        public bool AllLotDataCheck
        {
            get { return _allLotDataCheck; }
            set
            {
                SetProperty(ref _allLotDataCheck, value);
                if (value) CusLotDataBoxVis = Visibility.Hidden;
            }
        }
        private bool _customLotDataCheck;
        public bool CustomLotDataCheck
        {
            get { return _customLotDataCheck; }
            set
            {
                SetProperty(ref _customLotDataCheck, value);
                if (value) CusLotDataBoxVis = Visibility.Visible;
            }
        }


        private Visibility _cusLotDataBoxVis;
        public Visibility CusLotDataBoxVis
        {
            get { return _cusLotDataBoxVis; }
            set { SetProperty(ref _cusLotDataBoxVis, value); }
        }


        private int _lotRecCount = 200;
        public int LotRecCount
        {
            get => _lotRecCount;
            set => SetProperty(ref _lotRecCount, value);
        }

        private List<string> _sortLotOrder = new List<string>() { "Newest", "Oldest" };
        public List<string> SortLotOrder
        {
            get { return _sortLotOrder; }
            set { SetProperty(ref _sortLotOrder, value); }
        }

        private int _selectLotSortOrder = 0;
        public int SelectLotSortOrder
        {
            get { return _selectLotSortOrder; }
            set { SetProperty(ref _selectLotSortOrder, value); }
        }


        private bool _allLotFieldsChecked = true;
        public bool AllLotFieldsChecked
        {
            get { return _allLotFieldsChecked; }
            set
            {
                SetProperty(ref _allLotFieldsChecked, value);
            }
        }

        private int _SelectedBaleIndex;
        public int SelectedBaleIndex
        {
            get => _SelectedBaleIndex; 
            set
            {
                if (value > -1) BLotDataexcist = true;
                SetProperty(ref _SelectedBaleIndex, value);
            }
        }

        private string _selectedItemName;
        public string SelectedItemName
        {
            get => _selectedItemName;
            set{SetProperty(ref _selectedItemName, value);}
        }


        private bool _bLotDataexcist = false;
        public bool BLotDataexcist
        {
            get => _bLotDataexcist;
            set => SetProperty(ref _bLotDataexcist, value);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        private DataTable _archiveDataTable;
        public DataTable ArchiveDataTable
        {
            get { return _archiveDataTable; }
            set { SetProperty(ref _archiveDataTable, value);}
        }

        public ArchiveViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            
            if(_sqlhandler == null) 
                _sqlhandler = ClassSqlHandler.Instance;

            MonthChecked = true;
            AllDataCheck = true;
            AllFieldsChecked = true;

            LotMonthChecked = true;
            AllLotDataCheck = true;

            BLotDataexcist = false;
            BBaleDataexcist = false;
        }

        private bool _bBaleDataexcist = false;
        public bool BBaleDataexcist
        {
            get => _bBaleDataexcist;
            set => SetProperty(ref _bBaleDataexcist, value);
        }




        private DelegateCommand _ApplyCommand;
        public DelegateCommand ApplyCommand => 
            _ApplyCommand ?? (_ApplyCommand = new DelegateCommand(ApplyExecute));
        private void ApplyExecute()
        {
            string orderby = "UID"; //UID Index

            _eventAggregator.GetEvent<SendMessageEvents>().Publish($"Clear");

            if (iTabSelected == 1)
            {
                string strItems = GetItemsFilters();
                string strDataSelect = GetDataItemFilters();
                string strOrder = (SelectSortOrder == 0) ? $" ORDER BY [{orderby}] DESC;" : $"ORDER BY [{orderby}] ASC;";
                string strTimeFrame = GetTimeFilters();
                string strquery = $"SELECT {strDataSelect} {strTimeFrame} {strItems} {strOrder}";

                _ = UpdateBaleArchiveData(strquery);
            }
            else if (iTabSelected == 2)
            {
                RemoveLotColumnList = new List<string>();
                CreateLotFieldListtoRemove();

                string strDataSelect = GetLotDataItemFilters();
                string strOrder = (SelectLotSortOrder == 0) ? $" ORDER BY [{orderby}] DESC;" : $"ORDER BY [{orderby}] ASC;";
                string strTimeFrame = GetLotTimeFilters();
                string strquery = $"SELECT {strDataSelect} {strTimeFrame} {strOrder}";

                _ = UpdateBaleArchiveData(strquery);
            }
        }


        private async Task UpdateBaleArchiveData(string strquery)
        {
            LoadingWindow tempWindow = new LoadingWindow();
            tempWindow.Show();

            try
            {
                if (ArchiveDataTable != null) ArchiveDataTable = null;
                ArchiveDataTable?.Clear();

                DataTable TempTable = await _sqlhandler.GetBaleArchiveDataTableAsyn(strquery);
                Totalcount = TempTable.Rows.Count;
                if (Totalcount > 0)
                {
                    QueryOn = true;
                    WriteOpc = 1;

                    if (iTabSelected == 1)
                    {
                        if (AllFieldsChecked)
                        {
                            if (TempTable.Rows.Count > 0)
                            {
                                _sqlhandler.SetMoistureType(TempTable);
                            }
                        }
                    }
                    else if (iTabSelected == 2)
                    {
                        foreach (var item in RemoveLotColumnList)
                        {   
                            if (RemoveLotColumnList.Contains(item))
                            {

                                if(TempTable.Columns.Contains(item))
                                    TempTable.Columns.Remove(item);
                            }
                               
                        }
                        if (TempTable.Rows.Count > 0)
                        {
                            _sqlhandler.SetWeightType(TempTable);
                        }
                    }
                    TempTable.AcceptChanges();
                    ArchiveDataTable = TempTable;
                }
                else
                {
                    _eventAggregator.GetEvent<SendMessageEvents>().Publish($"No Record Found!");
                    BLotDataexcist = false;
                }
            }
            catch (Exception ex)
            {
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in UpdateBaleArchiveData < {ex.Message}");
                MessageBox.Show($"ERROR in UpdateBaleArchiveData < {ex.Message}");
            }

            finally 
            {
                tempWindow.Close();
                if (tempWindow != null) tempWindow = null;
            }
        }

        private DelegateCommand _selectFieldsCommand;
        public DelegateCommand SelectFieldsCommand =>
        _selectFieldsCommand ?? (_selectFieldsCommand = new DelegateCommand(SelserFieldExecute));
        private void SelserFieldExecute()
        {
            FieldSelectView MyDataItems = new FieldSelectView(_eventAggregator)
            {
                Height = 460,
                Width = 960
            };
            MyDataItems.ShowDialog();
        }

        private DelegateCommand _showGraphCommand;
        public DelegateCommand ShowGraphCommand =>
            _showGraphCommand ?? (_showGraphCommand =
           new DelegateCommand(ShowGraphExecute).ObservesCanExecute(() => BLotDataexcist));
        private void ShowGraphExecute()
        {
            ShowGraph();
        }

        private void ShowGraph()
        {
            string LotIdString = string.Empty;
            string StrItem = string.Empty;
            DateTime Opendate = DateTime.Today;
            DateTime Closedate = DateTime.Today;
            int iCloseHour = 0;
            char[] separators = { ':' };

            if (ArchiveDataTable.Rows.Count > 0)
            {
                if (SelectedBaleIndex == -1)
                    MessageBox.Show("Please Select lot Number to display graph !");
                else
                {
                    if (ArchiveDataTable.Rows[SelectedBaleIndex]["LotNum"] != null)
                        StrItem = ArchiveDataTable.Rows[SelectedBaleIndex]["LotNum"].ToString();

                    if (ArchiveDataTable.Rows[SelectedBaleIndex]["OpenTD"] != null)
                        Opendate = ArchiveDataTable.Rows[SelectedBaleIndex].Field<DateTime>("OpenTD");

                    if (ArchiveDataTable.Rows[SelectedBaleIndex]["CloseTD"] != null)
                        Closedate = ArchiveDataTable.Rows[SelectedBaleIndex].Field<DateTime>("CloseTD");

                    if (ArchiveDataTable.Rows[SelectedBaleIndex]["FC_IdentString"] != null)
                        LotIdString = ArchiveDataTable.Rows[SelectedBaleIndex].Field<String>("FC_IdentString");

                    if (Opendate.Hour < iCloseHour)
                        Opendate = Opendate.AddDays(1);

                    if (Closedate.Hour < iCloseHour)
                        Closedate = Closedate.AddDays(1);

                    try
                    {
                        using (DataTable LotCsvTable = _sqlhandler.GetTableByLotNum(StrItem, SetupStrCSVitems(), Opendate.AddSeconds(-5), Closedate.AddSeconds(5), LotIdString, SelectedLotMonth))
                        {
                            _sqlhandler.SetWeightType(LotCsvTable);

                            if (LotCsvTable.Rows.Count > 0)
                            {
                                UCGraphView ShowGrapgView = new UCGraphView(LotCsvTable, StrItem, SelectedLotMonth) //StrItem
                                {
                                    Height = 800,
                                    Width = 1000
                                };
                                ShowGrapgView.ShowDialog();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in ShowGraph < {ex.Message}");
                    }
                }
            }
            else
                MessageBox.Show("No data for the Selected month, Please select another month and do new query!");
        }


        // Write CSV file ------------------------------------------- // BBaleDataexcist = false;

        private DelegateCommand _writeCSVAllCommand;
        public DelegateCommand WriteCSVAllCommand =>
             _writeCSVAllCommand ?? (_writeCSVAllCommand =
            new DelegateCommand(WriteCSVAllExecute).ObservesCanExecute(() => QueryOn).ObservesCanExecute(() => BLotDataexcist));
        private void WriteCSVAllExecute()
        {
            int iStart = 9999;
            int iEnd = ArchiveDataTable.Rows.Count;

            if (iEnd > 0) 
            {
                try
                {
                    switch (iTabSelected)
                    {
                        case 1:
                            if (ArchiveDataTable.Rows.Count > 0)
                            {
                                using (CSVReport csvDlg = new CSVReport(_eventAggregator))
                                {
                                    csvDlg.InitCsv(ArchiveDataTable, SelectTableValue, iStart, iEnd);
                                    csvDlg.ShowDialog();
                                }
                            }
                            break;
                        
                        case 2:
                            if (SelectedBaleIndex == -1)
                                MessageBox.Show("No data for the Selected month, Please select another month and do new query!");
                            else
                            {
                                WriteLotCsv(iStart, iEnd);
                            }
                            break;
                    }

                    QueryOn = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR in MenuWrite_Click " + ex);
                    ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in MenuWrite_Click < {ex.Message}");
                }
                finally { WriteOpc = .2; }
            }
        }

        private void WriteLotCsv(int iStart, int iEnd)
        {
            string LotIdString = string.Empty;
            string StrItem = string.Empty;
            DateTime Opendate = DateTime.Today;
            DateTime Closedate = DateTime.Today;

            if (ArchiveDataTable.Rows[SelectedBaleIndex]["LotNum"] != null)
                StrItem = ArchiveDataTable.Rows[SelectedBaleIndex]["LotNum"].ToString();

            if (ArchiveDataTable.Rows[SelectedBaleIndex]["OpenTD"] != null)
                Opendate = ArchiveDataTable.Rows[SelectedBaleIndex].Field<DateTime>("OpenTD");

            if (ArchiveDataTable.Rows[SelectedBaleIndex]["CloseTD"] != null)
                Closedate = ArchiveDataTable.Rows[SelectedBaleIndex].Field<DateTime>("CloseTD");

            if (ArchiveDataTable.Rows[SelectedBaleIndex]["FC_IdentString"] != null)
                LotIdString = ArchiveDataTable.Rows[SelectedBaleIndex]["FC_IdentString"].ToString();

            try
            {
                using (DataTable LotCsvTable = _sqlhandler.GetTableByLotNum(StrItem, SetupStrCSVitems(), Opendate, Closedate, LotIdString, SelectedMonth))
                {
                    if (LotCsvTable.Rows.Count > 0)
                    {
                        using (CSVReport csvDlg = new CSVReport(_eventAggregator))
                        {
                            csvDlg.InitCsv(LotCsvTable, $"LOT_{StrItem} Date_{Closedate.Day}_{Closedate.Month}_{Closedate.Year}" + LotIdString, iStart, iEnd);
                            csvDlg.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in WriteLotCsv < {ex.Message}");
            }
        }



        private string SetupStrCSVitems()
        {

            string strQuery = string.Empty;
            char[] charsToTrim = { ',' };

            List<string> CSVItems = new List<string>
            {
                "LotBaleNumber",
                "Weight",
                "Moisture",
                "StockName",
                //"UpCount",
                //"DownCount",
                "TimeComplete",
                "UnitNumber",
                "Forte"
            };

            foreach (var item in CSVItems)
            {
                strQuery += item + ",";
            }
            return strQuery.TrimEnd(charsToTrim);
        }

        private string GetTimeFilters()
        {
            string Timeframe = string.Empty;
            if(MonthChecked) 
            {
                Timeframe = $" FROM  [ForteData].[dbo].[{SelectTableValue}] with (NOLOCK) ";
            }
            else if(DayChecked) 
            {
                string strStartTime = "00:00";
                string strEndTime = "23:59";
                if (EndQueryDate > DateTime.Now)
                    EndQueryDate = DateTime.Now;

                string strStartDate = StartQueryDate.Value.Date.ToString("MM/dd/yyyy") + " " + strStartTime;
                string strEndDate = EndQueryDate.Value.Date.ToString("MM/dd/yyyy") + " " + strEndTime;

                string startmonth = StartQueryDate.Value.Date.ToString("MMM");
                string endtmonth = EndQueryDate.Value.Date.ToString("MMM");

                string yearstart = StartQueryDate.Value.Date.ToString("yy");

                if (startmonth == endtmonth)
                {
                    string tablename = $"BaleArchive{startmonth}{yearstart}";
                    Timeframe = $"FROM [ForteData].[dbo].[{tablename}] with (NOLOCK) WHERE CAST(TimeStart AS DATETIME) BETWEEN '{strStartDate}' and '{strEndDate}'";
                }
               // Timeframe = $"FROM [ForteData].[dbo].[{SelectTableValue}] with (NOLOCK) WHERE CAST(TimeStart AS DATETIME) BETWEEN '{strStartDate}' and '{strEndDate}'";
            }
            return Timeframe;
        }

        private string GetDataItemFilters()
        {
            AllFieldsName = _sqlhandler.GetAllFieldName(ClassSqlHandler.BALE_ARCHIVE);
            CustomFieldSelect = GetCustomFields();

            string datdItems = string.Empty;
            string QueryFields =   string.Empty;

           
            if (AllDataCheck)
            {
                if(AllFieldsChecked)
                {
                    datdItems = AllFieldsName; //"*"; //
                }
                else
                    datdItems = CustomFieldSelect;

            }
            else if(CustomDataCheck) 
            {
                if (AllFieldsChecked)
                {
                    datdItems = $"TOP {RecCount.ToString()} {AllFieldsName}";
                }
                else
                {
                    datdItems = $"TOP {RecCount.ToString()} {CustomFieldSelect}";
                }
                    
            }
            return datdItems;
        }

        private string GetCustomFields()
        {
            string Cfield = string.Empty;

            _sqlhandler.GetCustomFieldsList();

            List<string> CustomFieldLst = _sqlhandler.CustomFieldsList;


            if (CustomFieldLst.Count > 0)
            {
                string AllList = string.Empty;

                foreach (var item in CustomFieldLst)
                {
                    AllList = AllList + item + ",";
                }
                AllList = AllList.Remove(AllList.Length - 1);

                Cfield = AllList + " ";
            }
            return Cfield;
        }



        private string GetItemsFilters()
        {
            string strItems = string.Empty;
            string strStock = string.Empty;
            string strGrade = string.Empty;
            string strLine = string.Empty;
            string strSource = string.Empty;
            itemchecked = 0;

            if (StockChecked)
            {
                if (DayChecked)
                {
                    strStock = $"And StockName = '{StockSelected}'";
                } else
                    strStock = $"Where StockName = '{StockSelected}'";
                
                itemchecked += 1;
            }
            if (GradeChecked)
            {
                if ((itemchecked > 0) | (DayChecked))
                {
                    strGrade = $"And GradeName = '{GradeSelected}'";
                }
                else
                    strGrade = $"Where GradeName = '{GradeSelected}'";

                itemchecked += 1;
            }
            if (LineChecked)
            {
                if ((itemchecked > 0) | (DayChecked))
                {
                    strLine = $"And LineName = '{LineSelected}'";
                }
                else
                    strLine = $"Where LineName = '{LineSelected}'";

                itemchecked += 1;

            }
            if (SourceChecked)
            {
                if ((itemchecked > 0) | (DayChecked))
                {
                    strSource = $"And SourceName = '{SourceSelected}'";
                }
                else
                    strSource = $"Where SourceName = '{SourceSelected}'";

                itemchecked += 1;
            }
            strItems = $"{strStock} {strGrade} {strLine} {strSource}";
            

            return strItems;
        }

        private string GetLotDataItemFilters()
        {
            AllLotFieldsName = _sqlhandler.GetAllFieldName(ClassSqlHandler.LOT_ARCHIVE);


            string datdItems = string.Empty;
            string QueryFields = string.Empty;


            if (AllLotDataCheck)
            {
                if (AllLotFieldsChecked)
                {
                    datdItems = "*";
                }
                else
                    datdItems = CustomFieldSelect;

            }
            else if (CustomLotDataCheck)
            {
                if (AllLotFieldsChecked)
                {
                    datdItems = $"TOP {RecCount.ToString()} {AllLotFieldsName}";
                }

            }
            return datdItems;
        }

        private string GetLotTimeFilters()
        {
            string Timeframe = string.Empty;
            if (MonthChecked)
            {
                Timeframe = $" FROM  [ForteData].[dbo].[{SelectLotTableValue}] with (NOLOCK) ";
            }
            else if (DayChecked)
            {
                string strStartTime = "00:00";
                string strEndTime = "23:59";
                if (EndQueryDate > DateTime.Now)
                    EndQueryDate = DateTime.Now;

                string strStartDate = StartLotQueryDate.Value.Date.ToString("MM/dd/yyyy") + " " + strStartTime;
                string strEndDate = EndLotQueryDate.Value.Date.ToString("MM/dd/yyyy") + " " + strEndTime;

                string startmonth = StartLotQueryDate.Value.Date.ToString("MMM");
                string endtmonth = EndLotQueryDate.Value.Date.ToString("MMM");

                string yearstart = StartLotQueryDate.Value.Date.ToString("yy");

                if (startmonth == endtmonth)
                {
                    string tablename = $"BaleArchive{startmonth}{yearstart}";
                    Timeframe = $"FROM [ForteData].[dbo].[{tablename}] with (NOLOCK) WHERE CAST(TimeStart AS DATETIME) BETWEEN '{strStartDate}' and '{strEndDate}'";
                }
                // Timeframe = $"FROM [ForteData].[dbo].[{SelectTableValue}] with (NOLOCK) WHERE CAST(TimeStart AS DATETIME) BETWEEN '{strStartDate}' and '{strEndDate}'";
            }
            return Timeframe;
        }


        public void LoadPage()
        {
            _pageloaded = true;

            ClsSerilog.LogMessage(ClsSerilog.INFO, $"---------- Load Page Archive");

        }

        public void UnLoadPage()
        {
            _pageloaded = false;
        }


        private void CreateLotFieldListtoRemove()
        {
            if (RemoveLotColumnList.Count > 0) RemoveLotColumnList.Clear();
            else
            {
                RemoveLotColumnList.Add("Empty");
                RemoveLotColumnList.Add("PriGrp");
                RemoveLotColumnList.Add("SecGrp");
                RemoveLotColumnList.Add("OnHold");
                RemoveLotColumnList.Add("JobNum");
                RemoveLotColumnList.Add("NW2N");
                RemoveLotColumnList.Add("MeanNW");
                RemoveLotColumnList.Add("StdDevNW");
                RemoveLotColumnList.Add("MC2M");
                RemoveLotColumnList.Add("AsciiFld1");
                RemoveLotColumnList.Add("AsciiFld2");
                RemoveLotColumnList.Add("AsciiFld3");
                RemoveLotColumnList.Add("AsciiFld4");
                RemoveLotColumnList.Add("SpareSngFld1");
                RemoveLotColumnList.Add("SpareSngFld2");
                RemoveLotColumnList.Add("SpareSngFld3");
                RemoveLotColumnList.Add("CloseBySize");
                RemoveLotColumnList.Add("CloseByTime");
                RemoveLotColumnList.Add("NextBaleNumber");
            }
        }
        private void CreateBaleFieldListtoRemove()
        {
            if (RemoveBaleColumnList.Count > 0) RemoveBaleColumnList.Clear();
            else
            {
                RemoveBaleColumnList.Add("Empty");
                RemoveBaleColumnList.Add("Index");
                RemoveBaleColumnList.Add("ForteAveraging");
                RemoveBaleColumnList.Add("Forte1");
                RemoveBaleColumnList.Add("Forte1Status");
                RemoveBaleColumnList.Add("Forte2");
                RemoveBaleColumnList.Add("Forte2Status");
                RemoveBaleColumnList.Add("DownCount2");
                RemoveBaleColumnList.Add("DownCount2Status");
                RemoveBaleColumnList.Add("BrightnessStatus");
                RemoveBaleColumnList.Add("BaleHeightStatus");
                RemoveBaleColumnList.Add("TimeStartStatus");
                RemoveBaleColumnList.Add("TimeCompleteStatus");
                RemoveBaleColumnList.Add("SheetCountStatus");
                RemoveBaleColumnList.Add("AllowanceStatus");
            }
        }
    }
}
