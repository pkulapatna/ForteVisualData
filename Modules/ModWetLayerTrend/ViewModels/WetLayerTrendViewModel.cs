using AppServices;
using ModWetLayerTrend.Properties;
using ModWetLayerTrend.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModWetLayerTrend.ViewModels
{
    public class WetLayerTrendViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;
        public static string ModuleName = "Wet Layer Trend Graph And layers offset";
        private readonly ClassSqlHandler _sqlhandler;

        private int iLayerCount;
        public int DefaultLayersCount = 16;


        private bool _graphAvgChecked;
        public bool GraphAvgChecked
        {
            get => _graphAvgChecked;
            set { SetProperty(ref _graphAvgChecked, value); }
        }

        private bool _graphOffChecked;
        public bool GraphOffChecked
        {
            get => _graphOffChecked;
            set { SetProperty(ref _graphOffChecked, value); }
        }


        private bool _enablePrint = false;
        public bool EnablePrint
        {
            get { return _enablePrint; }
            set { SetProperty(ref _enablePrint, value); }
        }

        private List<string> _wlmonthTableList;
        public List<string> WLMonthTableList
        {
            get { return _wlmonthTableList; }
            set { SetProperty(ref _wlmonthTableList, value); }
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
            get { return _selectTableValue; }
            set { SetProperty(ref _selectTableValue, value); }
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

        private bool _BalerCheck = false;
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

        private int _selectOCRindex;
        public int SelectOCRIndex
        {
            get { return _selectOCRindex; }
            set { SetProperty(ref _selectOCRindex, value); }
        }


        private int _bsample = Settings.Default.BSamples;
        public int BSamples
        {
            get => _bsample;
            set
            {
                if ((value > 9) & (value < 1001))
                    SetProperty(ref _bsample, value);
                else
                    SetProperty(ref _bsample, 100);

                Settings.Default.BSamples = _bsample;
                Settings.Default.Save();
            }
        }

        private string _totalcount = string.Empty;
        public string Totalcount
        {
            get { return _totalcount; }
            set { SetProperty(ref _totalcount, value); }
        }


        private string _LayerAvg;
        public string LayerAvg
        {
            get { return _LayerAvg; }
            set { SetProperty(ref _LayerAvg, value); }
        }

        private string _LayerMax;
        public string LayerMax
        {
            get { return _LayerMax; }
            set { SetProperty(ref _LayerMax, value); }
        }

        private string _LayerMin;
        public string LayerMin
        {
            get { return _LayerMin; }
            set { SetProperty(ref _LayerMin, value); }
        }

        private double _MaximumHeight;
        public double MaximumHeight
        {
            get { return _MaximumHeight; }
            set { SetProperty(ref _MaximumHeight, value); }
        }

        private double _MinimumHeight;
        public double MinimumHeight
        {
            get { return _MinimumHeight; }
            set { SetProperty(ref _MinimumHeight, value); }
        }

        private DataTable _WetLayerDeltaTable;
        public DataTable WetLayerDeltaTable
        {
            get { return _WetLayerDeltaTable; }
            set { SetProperty(ref _WetLayerDeltaTable, value); }
        }

        public WetLayerTrendViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;


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
                GraphOffChecked = true;

                SelectTableIndex = 0;
                SelectOCRIndex = 0;

            }
        }

        private int _XColumns;
        public int XColumns
        {
            get { return _XColumns; }
            set { SetProperty(ref _XColumns, value); }
        }

        private DelegateCommand _QueryCommand;
        public DelegateCommand QueryCommand =>
            _QueryCommand ?? (_QueryCommand = new DelegateCommand(QueryExecute));
        private void QueryExecute()
        {
            GetArchiveWLData();
           
        }

        private void GetArchiveWLData()
        {
            if (BSamples > 0)
            {
                GetWLDataGridview(SelectTableValue, BSamples);
            }
        }


        private void GetWLDataGridview(string selectTableValue, int bSamples)
        {
            string strOccr = string.Empty;
            string strAllColumns = "*";

            if (BalerCheck)
            {
                if (SelectOCRIndex == 0) strOccr = " WHERE BalerID = " + SelectBalerValue + " ORDER BY ReadTime DESC ;";
                else if (SelectOCRIndex == 1) strOccr = " WHERE BalerID= " + SelectBalerValue + " ORDER BY ReadTime ASC ;";
            }
            else
            {
                if (SelectOCRIndex == 0) strOccr = " ORDER BY ReadTime DESC ;";
                else if (SelectOCRIndex == 1) strOccr = " ORDER BY ReadTime ASC ;";
            }

            string strQuery = "SELECT TOP " + BSamples + " " + strAllColumns + " From " + SelectTableValue + " with (NOLOCK) " + strOccr;

            //strQuery = SELECT TOP 100 * From FValueReadingsJul24 with (NOLOCK)  ORDER BY ReadTime DESC ;
            
            try
            {
                DataTable WLDataTable = new DataTable();
                WLDataTable = (DataTable)_sqlhandler.GetWetLayerDataTable(SelectTableValue, strQuery);
                if (WLDataTable.Rows.Count > 0)
                {
                    EnablePrint = true; 
                    ProccessData(WLDataTable);
                }
                else
                {
                    MessageBox.Show("No Record found in  = " + SelectTableValue);
                    EnablePrint = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in GetWLDataGridview = " + ex.Message);
            }
        }


        private void ProccessData(DataTable wLDataTable)
        {
            List<Double> ListAvg = new List<double>();

            double[] AvgX = new double[16]; 

            if (wLDataTable.Rows[0]["Layers"] == null)
                iLayerCount = DefaultLayersCount;
            else
                iLayerCount = Convert.ToInt32(wLDataTable.Rows[0]["Layers"].ToString());

            Totalcount = wLDataTable.Rows.Count.ToString();

            XColumns = iLayerCount + 1;
            DataTable DtaTable = SetUpDatTable(XColumns);

            //Array of double for each layers.
            List<double>[] fLayer = new List<double>[iLayerCount];


            try
            {
                for (int y = 0; y < iLayerCount; y++)
                {
                    fLayer[y] = new List<double>();
                    for (int i = 0; i < wLDataTable.Rows.Count; i++)
                    {
                        if (wLDataTable.Rows[i]["Layer" + (y + 1).ToString()].ToString() != String.Empty)
                        {
                            if (ClassCommon.MoistureType == 0)
                                fLayer[y].Add(wLDataTable.Rows[i].Field<Double>("Layer" + (y + 1)));
                            if (ClassCommon.MoistureType == 1)
                                fLayer[y].Add(ConvToMR(wLDataTable.Rows[i].Field<Double>("Layer" + (y + 1))));
                        }
                    }
                }

                if (ClassCommon.MoistureType == 0)
                    DtaTable.Rows.Add("MC.%");
                else if (ClassCommon.MoistureType == 1)
                    DtaTable.Rows.Add("MR.%");

                for (int i = 1; i < iLayerCount + 1; i++)
                {
                    if (fLayer[i - 1].Count > 1)
                    {
                        ListAvg.Add(fLayer[i - 1].Average());
                        DtaTable.Rows[0][i] = fLayer[i - 1].Average().ToString("#0.00");
                    }
                }

                MaximumHeight = ListAvg.Max() + 0.5;
                MinimumHeight = ListAvg.Min() - 0.5;

                if (ClassCommon.MoistureType == 0)
                    DtaTable.Rows.Add("Avg.- MC.%");
                else if (ClassCommon.MoistureType == 1)
                    DtaTable.Rows.Add("Avg.- MR.%");

                for (int i = 1; i < iLayerCount + 1; i++)
                {
                    if (fLayer[i - 1].Count > 1)
                    {
                        DtaTable.Rows[1][i] = (fLayer[i - 1].Average() - ListAvg.Average()).ToString("#0.00");

                        AvgX[i-1] = (double)DtaTable.Rows[1][i];
                    }
                }

                WetLayerDeltaTable = DtaTable;


                if(GraphAvgChecked)
                {
                    LayerAvg = ListAvg.Average().ToString("#0.00");
                    LayerMax = ListAvg.Max().ToString("#0.00");
                    LayerMin = ListAvg.Min().ToString("#0.00");
                    WetLayerTrendView._wetLayerTrendView?.PlotChart(ListAvg, AvgX);
                }
                else
                {
                    LayerAvg = AvgX.Average().ToString("#0.00");
                    LayerMax = AvgX.Max().ToString("#0.00");
                    LayerMin = AvgX.Min().ToString("#0.00");
                    WetLayerTrendView._wetLayerTrendView?.PlotChart2(AvgX);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in ProccessData = " + ex.Message);
            }

        }

        private double ConvToMR(double fMoisture)
        {
            return fMoisture / (1 - fMoisture / 100);
        }

        private DataTable SetUpDatTable(int LayersColumn)
        {
            DataTable NewTable = new DataTable();
            DataColumn[] DatColumn = new DataColumn[LayersColumn];

            DatColumn[0] = new DataColumn("Type", typeof(string));
            NewTable.Columns.Add(DatColumn[0]);

            for (int i = 1; i < LayersColumn; i++)
            {
                DatColumn[i] = new DataColumn("Layer" + i.ToString(), typeof(double));
                NewTable.Columns.Add(DatColumn[i]);
                //Console.WriteLine("DatColumn[i] = " + DatColumn[i] + " i= " + i);
            }
            return NewTable;
        }

    }
}
