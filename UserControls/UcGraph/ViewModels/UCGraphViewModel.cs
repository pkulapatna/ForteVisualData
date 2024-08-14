using AppServices;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using UcGraph.Views;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace UcGraph.ViewModels
{
    internal class UCGraphViewModel : BindableBase
    {
        private readonly string selectedLot;
        private readonly DateTime datestart;
        private readonly DateTime dateEnd;
        private readonly string lotid;


        private string _lowvalue;
        public string LowValue
        {
            get { return _lowvalue; }
            set { SetProperty(ref _lowvalue, value); }
        }
        private string _hiValue;
        public string HiValue
        {
            get { return _hiValue; }
            set { SetProperty(ref _hiValue, value); }
        }
        private string _AvgValue;
        public string AvgValue
        {
            get { return _AvgValue; }
            set { SetProperty(ref _AvgValue, value); }
        }
        private string _stdValue;
        public string STDValue
        {
            get { return _stdValue; }
            set { SetProperty(ref _stdValue, value); }
        }

        private string _ItemUnit;
        public string ItemUnit
        {
            get { return _ItemUnit; }
            set { SetProperty(ref _ItemUnit, value); }
        }

        private string _ItemLegend;
        public string ItemLegend
        {
            get { return _ItemLegend; }
            set { SetProperty(ref _ItemLegend, value); }
        }

        private string _TxtStatus;
        public string TxtStatus
        {
            get { return _TxtStatus; }
            set { SetProperty(ref _TxtStatus, value); }
        }

        private DataTable _ArchiveTable;
        public DataTable ArchiveTable
        {
            get { return _ArchiveTable; }
            set { SetProperty(ref _ArchiveTable, value); }
        }

        private string _tableName;
        public string TableName
        {
            get { return _tableName; }
            set { SetProperty(ref _tableName, value); }
        }

        private int _BaleInLot;
        public int BaleInLot
        {
            get { return _BaleInLot; }
            set { SetProperty(ref _BaleInLot, value); }
        }


        private string _lotcharttitle;
        public string LotChartTitle
        {
            get { return _lotcharttitle; }
            set { SetProperty(ref _lotcharttitle, value); }
        }

        private double[] timeX;
        private double[] MvalueY;
        private double[] WvalueY;
        private double[] FvalueY;

        private string _chartTitle;
        public string ChartTitle
        {
            get => _chartTitle;
            set => SetProperty(ref _chartTitle, value);
        }


        private bool _menuOneChecked = true;
        public bool MenuOneChecked
        {
            get { return _menuOneChecked; }
            set
            {
                if (value)
                {
                    double sumOfDerivation = 0;

                    ChartTitle = "Graph of Bale Moisture from Lot # " + selectedLot;
                    List<double> xData = new();


                    UCGraphView._ucGraphicView.PlotChart(timeX, MvalueY, ChartTitle);


                    for (int i = 0; i < timeX.Length; i++)
                    {
                        xData.Add(MvalueY[i]);
                    }

                    HiValue = xData.Max().ToString("#0.00"); 
                    LowValue = xData.Min().ToString("#0.00");
                    AvgValue = xData.Average().ToString("#0.00");


                    //Calculate STD
                    foreach (var Value in xData)
                    {
                        sumOfDerivation += (Value - Convert.ToDouble(AvgValue)) * (Value - Convert.ToDouble(AvgValue));
                    }
                    double Variance = sumOfDerivation / (xData.Count - 1);
                    STDValue = Math.Sqrt(Variance).ToString("#0.00");
                    BaleInLot = timeX.Length;

                }
                SetProperty(ref _menuOneChecked, value);
            }
        }

        private bool _menuTwoChecked;
        public bool MenuTwoChecked
        {
            get { return _menuTwoChecked; }
            set
            {
                if (value)
                {
                    double sumOfDerivation = 0;

                    ChartTitle = "Graph of Bales Weight from Lot # " + selectedLot;

                    List<double> xData = new();

                    UCGraphView._ucGraphicView.PlotChart(timeX, WvalueY, ChartTitle);

                    for (int i = 0; i < timeX.Length; i++)
                    {
                        xData.Add(WvalueY[i]);
                    }

                    HiValue = xData.Max().ToString("#0.00");
                    LowValue = xData.Min().ToString("#0.00");
                    AvgValue = xData.Average().ToString("#0.00");

                    //Calculate STD
                    foreach (var Value in xData)
                    {
                        sumOfDerivation += (Value - Convert.ToDouble(AvgValue)) * (Value - Convert.ToDouble(AvgValue));
                    }
                    double Variance = sumOfDerivation / (xData.Count - 1);
                    STDValue = Math.Sqrt(Variance).ToString("#0.00");
                    BaleInLot = timeX.Length;
                }
                SetProperty(ref _menuTwoChecked, value);
            }
        }

        private bool _menuThreeChecked;
        public bool MenuThreeChecked
        {
            get { return _menuThreeChecked; }
            set
            {
                if (value)
                {
                    ChartTitle = "Graph of Forte Number from Lot # " + selectedLot;

                    double sumOfDerivation = 0;
                    List<double> xData = new();

                    UCGraphView._ucGraphicView.PlotChart(timeX, FvalueY, ChartTitle);

                    for (int i = 0; i < timeX.Length; i++)
                    {
                        xData.Add(FvalueY[i]);
                    }

                    HiValue = xData.Max().ToString("#0.00");
                    LowValue = xData.Min().ToString("#0.00");
                    AvgValue = xData.Average().ToString("#0.00");

                    //Calculate STD
                    foreach (var Value in xData)
                    {
                        sumOfDerivation += (Value - Convert.ToDouble(AvgValue)) * (Value - Convert.ToDouble(AvgValue));
                    }
                    double Variance = sumOfDerivation / (xData.Count - 1);
                    STDValue = Math.Sqrt(Variance).ToString("#0.00");
                    BaleInLot = timeX.Length;
                }
                SetProperty(ref _menuThreeChecked, value);
            }
        }

        public UCGraphViewModel(System.Data.DataTable lotdatatable, string strItem, string selectedMonth)
        {
            ArchiveTable = lotdatatable;
            selectedLot = strItem;

            TableName = $"BaleArchive{selectedMonth}";

            if (ArchiveTable.Rows.Count > 0)
            {
                timeX = new double[ArchiveTable.Rows.Count];
                MvalueY = new double[ArchiveTable.Rows.Count];
                WvalueY = new double[ArchiveTable.Rows.Count];
                FvalueY = new double[ArchiveTable.Rows.Count];

                for (int i = 0; i < ArchiveTable.Rows.Count; i++)
                {
                    timeX[i] = i + 1;
                    MvalueY[i] = ClassCommon.ConvertMoisture(ArchiveTable.Rows[i].Field<float>("Moisture"), ClassCommon.MoistureType);
                    WvalueY[i] = ArchiveTable.Rows[i].Field<float>("Weight");
                    FvalueY[i] = Convert.ToDouble(ArchiveTable.Rows[i].Field<Int32>("Forte"));
                }

            }

            MenuOneChecked = true;
        }

    }
}
