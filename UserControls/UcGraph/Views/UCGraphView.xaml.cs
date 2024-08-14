using AppServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UcGraph.ViewModels;

namespace UcGraph.Views
{
    /// <summary>
    /// Interaction logic for UCGraphView.xaml
    /// </summary>
    public partial class UCGraphView : Window
    {
        public static UCGraphView _ucGraphicView;

        private UCGraphViewModel _uCGraphViewModel;

        private Point startpoint;


        public UCGraphView(System.Data.DataTable lotCsvTable, string strItem, string selectedMonth)
        {
            InitializeComponent();
            _ucGraphicView = this;
            _uCGraphViewModel =  new UCGraphViewModel(lotCsvTable, strItem, selectedMonth);
            this.DataContext = _uCGraphViewModel;
        }

        private void BtnShowData_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = true;
            //Double DMvalue;
            //Double DWvalue;
            double Coef;

            if (ClassCommon.WeightUnit == 0)
                Coef = 1; //Kg
            else
                Coef = 2.20462; //Lb.

            if (RealTimeGridView2 != null)
            {
               
                RealTimeGridView2.DataContext = _uCGraphViewModel.ArchiveTable;
            }
        }

        private Double GetMoisture(double dVal)
        {
            switch (ClassCommon.MoistureUnit)
            {
                case 0: // %MC == moisture from Sql database

                    break;

                case 1: // %MR  = Moisture / ( 1- Moisture / 100)
                    dVal /= (1 - dVal / 100);
                    break;

                case 2: // %AD = (100 - moisture) / 0.9
                    dVal = (100 - dVal) / 0.9;
                    break;

                case 3: // %BD  = 100 - moisture
                    dVal = 100 - dVal;
                    break;
            }
            return dVal;
        }


        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.StartsWith("index")) e.Column.Visibility = Visibility.Hidden;

            if (e.PropertyName.StartsWith("Moisture"))
            {
                switch (ClassCommon.MoistureUnit)
                {
                    case 0: // %MC
                        e.Column.Header = "MC %";
                        break;

                    case 1: // %MR
                        e.Column.Header = "MR %";
                        break;

                    case 2: // %AD
                        e.Column.Header = "AD %";
                        break;

                    case 3: // %BD
                        e.Column.Header = "BD %";
                        break;
                }
            }

            if (e.PropertyName.StartsWith("Weight"))
            {
                if (ClassCommon.WeightUnit == 0)
                    e.Column.Header = "Weight KG.";
                else
                    e.Column.Header = "Weight LB.";
            }

            if ((e.PropertyType == typeof(System.Single)) || (e.PropertyType == typeof(System.Double)))
            {
                e.Column.ClipboardContentBinding.StringFormat = "{0:0.##}";
                e.Column.Width = 80;
            }

        }

        private void CSV_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = false;
        }

        private void CSVAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PopUp_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point relative = e.GetPosition(null);
                Point AbsolutePos = new(relative.X, relative.Y);
                MyPopup.HorizontalOffset += AbsolutePos.X - startpoint.X;
                MyPopup.VerticalOffset += AbsolutePos.Y - startpoint.Y;
            }
        }

        private void PopUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startpoint = e.GetPosition(null);
        }

        internal void PlotChart(double[] timeX, double[] dataY, string chartTitle)
        {
            List<double> xData = new();

            WpfPlot3.Plot.Clear();

            WpfPlot3.Plot.Axes.Bottom.Label.Text = $"{timeX.Length} Bales in this Lot";
            WpfPlot3.Plot.Axes.Left.Label.Text = "Vertical Axis";
            WpfPlot3.Plot.Title(chartTitle);

            WpfPlot3.Plot.Axes.AutoScale();
            WpfPlot3.Plot.Axes.SetLimitsX(1, timeX.Length);
            WpfPlot3.Interaction.Disable();

            try
            {
               

                for (int i = 0; i < timeX.Length; i++)
                {
                    xData.Add(dataY[i]);
                }

                double mMax = xData.Max();
                double mMin = xData.Min();

                WpfPlot3.Plot.Axes.SetLimitsY(mMin - 2, mMax + 2);
                WpfPlot3.Plot.Add.Scatter(timeX, dataY);
                WpfPlot3.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in PlotChart {ex.Message}");
            }




        }

       
    }
}
