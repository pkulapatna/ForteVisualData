using AppServices;
using ModWetLayerTrend.Properties;
using ScottPlot;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModWetLayerTrend.Views
{
    /// <summary>
    /// Interaction logic for WetLayerTrendView.xaml
    /// </summary>
    public partial class WetLayerTrendView : UserControl
    {
        public static WetLayerTrendView _wetLayerTrendView;


        public WetLayerTrendView()
        {
            InitializeComponent();
            _wetLayerTrendView = this;
        }

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9.]+");
            return reg.IsMatch(str);
        }

        private void SampleBox_dclick(object sender, MouseButtonEventArgs e)
        {
            if (txtSample.IsReadOnly == false)
            {
                txtSample.Background = Brushes.AntiqueWhite;
                txtSample.IsReadOnly = true;
            }
            else
            {
                txtSample.Background = Brushes.White;
                txtSample.IsReadOnly = false;
            }
        }


        public void PlotChart(List<Double> listAvg, double[] avgX)
        {
            WpfPlot2.Plot.Clear();

            Tick[] labels = { new Tick(1,"Layer1"), new Tick(2, "Layer2"), new Tick(3, "Layer3"), new Tick(4, "Layer4"),
                new Tick(5, "Layer5"), new Tick(6, "Layer6") ,new Tick(7, "Layer7"), new Tick(8, "Layer8"),
                new Tick(9, "Layer9"),new Tick(10, "Layer10"),new Tick(11, "Layer11"), new Tick(12, "Layer12"),
                new Tick(13, "Layer13"),new Tick(14, "Layer14"),new Tick(15, "Layer15"), new Tick(16, "Layer16") };


            double[] dataY =  new double[listAvg.Count];
            double[] dataX = new double[] {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16 };

            double MinY = listAvg.Min() - 0.5;
            double MaxY = listAvg.Max() + 0.5;

            for (int i = 0; i < listAvg.Count; i++) 
            {
                dataY[i] = listAvg[i];
            }

            WpfPlot2.Plot.Axes.AutoScale();
            WpfPlot2.Plot.Axes.SetLimitsX(0, 17);
            WpfPlot2.Plot.Axes.SetLimitsY(MinY, MaxY);

            WpfPlot2.Interaction.Disable();
            

            var sp = WpfPlot2.Plot.Add.Scatter(dataX, dataY);

            sp.LegendText = "Layer MR%";
            
            sp.LineWidth = 3;
            sp.MarkerSize = 10;

            var cross = WpfPlot2.Plot.Add.Crosshair(0, listAvg.Max());
            cross.LineColor = ScottPlot.Colors.Orange;
            cross.LineWidth = 1;

            var cross1 = WpfPlot2.Plot.Add.Crosshair(0, listAvg.Average());
            cross1.LineColor = ScottPlot.Colors.Red;
            cross1.LineWidth = 2;

            var cross2 =  WpfPlot2.Plot.Add.Crosshair(0, listAvg.Min());
            cross2.LineColor = ScottPlot.Colors.Yellow;
            cross2.LineWidth = 1;


            WpfPlot2.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(labels);

            WpfPlot2.Refresh();


            if (ClassCommon.GraphDarkMode)
            {
                // change figure colors
                WpfPlot2.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#07263b");
                WpfPlot2.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#1f1f1f");

                // change axis and grid colors
                WpfPlot2.Plot.Axes.Color(ScottPlot.Color.FromHex("#d7d7d7"));
                WpfPlot2.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#7A7E7D");
            }
        }


        public void PlotChart2(double[] avgX)
        {
            WpfPlot2.Plot.Clear();

            Tick[] labels = { new Tick(1,"Layer1"), new Tick(2, "Layer2"), new Tick(3, "Layer3"), new Tick(4, "Layer4"),
                new Tick(5, "Layer5"), new Tick(6, "Layer6") ,new Tick(7, "Layer7"), new Tick(8, "Layer8"),
                new Tick(9, "Layer9"),new Tick(10, "Layer10"),new Tick(11, "Layer11"), new Tick(12, "Layer12"),
                new Tick(13, "Layer13"),new Tick(14, "Layer14"),new Tick(15, "Layer15"), new Tick(16, "Layer16") };


            double[] dataY = new double[avgX.Length];
            double[] dataX = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

            List<Double> AList = new List<double>();

           
            for (int i = 0; i < avgX.Length; i++)
            {
                AList.Add(avgX[i]);
            }

            double MinY = AList.Min() - 0.5;
            double MaxY = AList.Max() + 0.5;



            WpfPlot2.Plot.Axes.AutoScale();
            WpfPlot2.Plot.Axes.SetLimitsX(0, 17);
            WpfPlot2.Plot.Axes.SetLimitsY(MinY, MaxY);

            WpfPlot2.Interaction.Disable();

            var sp = WpfPlot2.Plot.Add.Scatter(dataX, avgX);

            //WpfPlot2.Plot.ScaleFactor = 2;

            if (ClassCommon.MoistureType == 0)
                sp.LegendText = "Layer Average - Layer Moisture Content%";
            else if (ClassCommon.MoistureType == 1)
                sp.LegendText = "Layer Average - Layer Moisture Regain%";

            sp.LineWidth = 3;
            sp.MarkerSize = 10;

            var cross = WpfPlot2.Plot.Add.Crosshair(0, AList.Max());
            cross.LineColor = ScottPlot.Colors.Orange;
            cross.LineWidth = 1;

            var cross1 = WpfPlot2.Plot.Add.Crosshair(0, AList.Average());
            cross1.LineColor = ScottPlot.Colors.Red;
            cross1.LineWidth = 2;

            var cross2 = WpfPlot2.Plot.Add.Crosshair(0, AList.Min());
            cross2.LineColor = ScottPlot.Colors.Yellow;
            cross2.LineWidth = 1;


            WpfPlot2.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(labels);

            WpfPlot2.Refresh();


            if (ClassCommon.GraphDarkMode)
            {
                // change figure colors
                WpfPlot2.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#07263b");
                WpfPlot2.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#1f1f1f");

                // change axis and grid colors
                WpfPlot2.Plot.Axes.Color(ScottPlot.Color.FromHex("#d7d7d7"));
                WpfPlot2.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#7A7E7D");
            }
        }
    }
}
