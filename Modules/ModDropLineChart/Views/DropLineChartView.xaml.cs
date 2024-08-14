using AppServices;
using ModDropLineChart.Properties;
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

namespace ModDropLineChart.Views
{
    /// <summary>
    /// Interaction logic for DropLineChartView.xaml
    /// </summary>
    public partial class DropLineChartView : UserControl
    {
        public static DropLineChartView _dropLineChartView;

        public DropLineChartView()
        {
            InitializeComponent();
            _dropLineChartView = this;
        }


        public void ShowStartBtn(bool bHide)
        {
            if (bHide)
                btnStart.Opacity = 1;
            else btnStart.Opacity = .3;
        }

        public void ShowStopBtn(bool bHide)
        {
            if (bHide)
                btnStop.Opacity = 1;
            else btnStop.Opacity = .3;
        }

        public void PlotChart(List<float> avgOne, List<float> avgTwo, 
            List<float> avgThree, List<float> avgFour, List<float> avgFive, List<float> avgSix, 
            List<float> avgSeven, List<float> avgEight, List<float> avgNine, List<float> avgTen, double maxVal, double minVal)
        {
            int LnOneLast = 0;
            int LnTwoLast = 0;
            int LnThreeLast = 0;
            int LnFourLast = 0;
            int LnFiveLast = 0;
            int LnSixLast = 0;
            int LnSevenLast = 0;
            int LnEightLast = 0;
            int LnNineLast = 0;

            double[] xs1 = new double[30];
            double[] xs2 = new double[30];
            double[] xs3 = new double[30];
            double[] xs4 = new double[30];
            double[] xs5 = new double[30];
            double[] xs6 = new double[30];
            double[] xs7 = new double[30];
            double[] xs8 = new double[30];
            double[] xs9 = new double[30];
            double[] xs10 = new double[30];

            double MaxWidth = Convert.ToDouble(Settings.Default.SelectedDropCt) * ClassCommon.BaleInDrop + ClassCommon.BaleInDrop;
            double MaxHeight = maxVal+(maxVal/4);
            double MinHeight = 0;
            string chartTitle = string.Empty;

            if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                chartTitle = $"Profile of - {ClassCommon.MoistureTypeLst[ClassCommon.MoistureType]} - " +
                             $" by bale position in {Convert.ToDouble(Settings.Default.SelectedDropCt)} Drop";
            else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                chartTitle = $"Profile of - {ClassCommon.WeightTypeLst[ClassCommon.WeightUnit]} - " +
                            $" by bale position in {Convert.ToDouble(Settings.Default.SelectedDropCt)} Drop";



            WpfPlot1.Plot.Clear();
            WpfPlot1.Refresh();
            WpfPlot1.Plot.Axes.AutoScale();
            WpfPlot1.Interaction.Disable();
            WpfPlot1.Plot.Title(chartTitle);
            WpfPlot1.Plot.ShowGrid();


            if (minVal > 0) MinHeight = minVal / 2;

            WpfPlot1.Plot.Axes.SetLimitsY(MinHeight, MaxHeight);
            WpfPlot1.Plot.Axes.SetLimitsX(0, MaxWidth);

           

            

            if (ClassCommon.GraphDarkMode)
            {
                // change figure colors
                WpfPlot1.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#07263b");
                WpfPlot1.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#1f1f1f");

                // change axis and grid colors
                WpfPlot1.Plot.Axes.Color(ScottPlot.Color.FromHex("#d7d7d7"));
                WpfPlot1.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#7A7E7D");
            }



            if(avgOne.Count > 0)   //Position 1
            {
                double[] y1Value = new double[30];
                for (int i = 1; i < avgOne.Count + 1; i++)
                {
                    xs1[i] = i;
                    y1Value[i] = avgOne[i - 1];
                    LnOneLast = i;
                }
                var bars1 = WpfPlot1.Plot.Add.Bars(xs1, y1Value);
                bars1.LegendText = "Position 1";
            }

            if(avgTwo.Count > 0)  //Position 2
            {
                double[] y2Value = new double[30];
                for (int i = 1; i < avgTwo.Count + 1; i++)
                {
                    xs2[i] = i + LnOneLast + 1;
                    y2Value[i] = avgTwo[i - 1];
                    LnTwoLast = LnOneLast + i;
                }
                var bars2 = WpfPlot1.Plot.Add.Bars(xs2, y2Value);
                bars2.LegendText = "Position 2";
            }

            if (avgThree.Count > 0) //Position 3
            {
                double[] y3Value = new double[30];
                for (int i = 1; i < avgThree.Count + 1; i++)
                {
                    xs3[i] = LnTwoLast + 2 + i;
                    y3Value[i] = avgThree[i - 1];
                    LnThreeLast = LnTwoLast + i;
                }
                var bars3 = WpfPlot1.Plot.Add.Bars(xs3, y3Value);
                bars3.LegendText = "Position 3";
            }
          
            if(avgFour.Count > 0)  //Position 4
            {
                double[] y4Value = new double[30];
                for (int i = 1; i < avgFour.Count + 1; i++)
                {
                    xs4[i] = LnThreeLast + 3 + i;
                    y4Value[i] = avgFour[i - 1];
                    LnFourLast = LnThreeLast + i;
                }
                var bars4 = WpfPlot1.Plot.Add.Bars(xs4, y4Value);
                bars4.LegendText = "Position 4";
            }

            if(avgFive.Count > 0) //Position 5
            {
                double[] y5Value = new double[30];

                for (int i = 1; i < avgFive.Count + 1; i++)
                {
                    xs5[i] = LnFourLast + 4 + i;
                    y5Value[i] = avgFive[i - 1];
                    LnFiveLast = LnFourLast + i;
                }
                var bars5 = WpfPlot1.Plot.Add.Bars(xs5, y5Value);
                bars5.LegendText = "Position 5";
            }

            if(avgSix.Count > 0) //Position 6
            {
                double[] y6Value = new double[30];

                for (int i = 1; i < avgSix.Count + 1; i++)
                {
                    xs6[i] = LnFiveLast + 5 + i;
                    y6Value[i] = avgSix[i - 1];
                    LnSixLast = LnFiveLast + i;
                }
                var bars6 = WpfPlot1.Plot.Add.Bars(xs6, y6Value);
                bars6.LegendText = "Position 6";
            }

            if (avgSeven.Count > 0) //Position 7
            {
                double[] y7Value = new double[30];

                for (int i = 1; i < avgSeven.Count + 1; i++)
                {
                    xs7[i] = LnSixLast + 6 + i;
                    y7Value[i] = avgSeven[i - 1];
                    LnSevenLast = LnSixLast + i;
                }
                var bars7 = WpfPlot1.Plot.Add.Bars(xs7, y7Value);
                bars7.LegendText = "Position 7";
            }

            if (avgEight.Count > 0) //Position 8
            {
                double[] y8Value = new double[30];

                for (int i = 1; i < avgEight.Count + 1; i++)
                {
                    xs8[i] = LnSevenLast + 7 + i;
                    y8Value[i] = avgEight[i - 1];
                    LnEightLast = LnSevenLast + i;
                }
                var bars8 = WpfPlot1.Plot.Add.Bars(xs8, y8Value);
                bars8.LegendText = "Position 8";
            }

            if (avgNine.Count > 0) //Position 9
            {
                double[] y9Value = new double[30];
                for (int i = 1; i < avgNine.Count + 1; i++)
                {
                    xs9[i] = LnEightLast + 8 + i;
                    y9Value[i] = avgNine[i - 1];
                    LnNineLast = LnSevenLast + i;
                }
                var bars9 = WpfPlot1.Plot.Add.Bars(xs9, y9Value);
                bars9.LegendText = "Position 9";
            }

            if (avgTen.Count > 0) //Position 10
            {
                double[] y10Value = new double[30];

                for (int i = 1; i < avgTen.Count + 1; i++)
                {
                    xs10[i] = LnNineLast + 9 + i;
                    y10Value[i] = avgTen[i - 1];
                }
                var bars10 = WpfPlot1.Plot.Add.Bars(xs10, y10Value);
                bars10.LegendText = "Position 10";
            }


            WpfPlot1.Refresh();
        }

        internal void Clearbale()
        {
           // b1c0.Visibility = Visibility.Hidden;
        }

        private void GridView_sidechanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void SampleBox_dclick(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
