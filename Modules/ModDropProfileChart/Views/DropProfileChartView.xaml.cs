using AppServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModDropProfileChart.Views
{
    /// <summary>
    /// Interaction logic for DropProfileChartView.xaml
    /// </summary>
    public partial class DropProfileChartView : UserControl
    {

        public static DropProfileChartView _dropProfileChartView;
        private readonly Storyboard myStoryboard = new Storyboard();
        private double IScreenWidth { get; set; }
        private double wdCoef = 0.0;

        public DropProfileChartView()
        {
            InitializeComponent();
            _dropProfileChartView = this;

            b1c0.Visibility = Visibility.Hidden;
        }

        private void GridView_sidechanged(object sender, SizeChangedEventArgs e)
        {

            double xwidth = e.NewSize.Width * .055;

            txtbox1.FontSize = xwidth;
            txtbox2.FontSize = xwidth;
            txtbox3.FontSize = xwidth;
            txtbox4.FontSize = xwidth;
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


        public void MoveBaleOne(int iBaleNum)
        {

            SetBaleProp(iBaleNum);

            DoubleAnimation myDoubleAnimation = new DoubleAnimation
            {
                From = 0,
                To = 280,
                Duration = new Duration(TimeSpan.FromSeconds(2)),
                AutoReverse = false
            };

            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Canvas.LeftProperty));
            myStoryboard.Children.Add(myDoubleAnimation);

            Storyboard.SetTargetName(myDoubleAnimation, "b1c0");
            myStoryboard.Begin(b1c0);
        }

        private void SetBaleProp(int balpos)
        {
            b1c0.Visibility = Visibility.Visible;
            txtblc0.Text = balpos.ToString();

            switch (balpos)
            {
                case 0:
                    rtb10.Fill = Brushes.Green;
                    break;

                default:
                    rtb10.Fill = Brushes.Blue;
                    break;

            }
        }

        public void Clearbale()
        {
            b1c0.Visibility = Visibility.Hidden;
        }

        public void PlotChart(DataTable baleTable, string chartTitle, int graphHeightM, int graphHeightW, int graphLowM, int graphLowW)
        {
            ScottPlot.Plot myPlot = new();

            int MinLowM =0;// = graphLowM;
            int MinLowW = 0;// = graphLowW;

            if(graphLowM>0) MinLowM = graphLowM / 3;
            if (graphLowW > 0) MinLowW = graphLowW / 2;

            int GraphWidth = (ClassCommon.BaleInDrop * ClassCommon.DropInChart) + 6;// ClassCommon.BaleInDrop * Number of Drop + 1;

            WpfPlot1.Plot.Clear();
            WpfPlot1.Refresh();
            WpfPlot1.Plot.Axes.AutoScale();
            WpfPlot1.Interaction.Disable();
            WpfPlot1.Plot.Title(chartTitle);

            WpfPlot1.Plot.Axes.SetLimitsX(0, GraphWidth);

            if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
            {
                WpfPlot1.Plot.Axes.SetLimitsY(MinLowM, graphHeightM + (graphHeightM/2));
            }
            else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
            {
                WpfPlot1.Plot.Axes.SetLimitsY(MinLowW, graphHeightW + (graphHeightW/4));
            }

            if (ClassCommon.GraphDarkMode)
            {
                // change figure colors
                WpfPlot1.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#07263b");
                WpfPlot1.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#1f1f1f");

                // change axis and grid colors
                WpfPlot1.Plot.Axes.Color(ScottPlot.Color.FromHex("#d7d7d7"));
                WpfPlot1.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#7A7E7D");
            }


            WpfPlot1.Plot.ShowGrid();
            var DropNumList = baleTable.AsEnumerable().Select(x => x.Field<int>("DropNumber")).Distinct().ToList();

            
            //Drop 1
            double[] y1Value = new double[ClassCommon.BaleInDrop];
            int LastI = 0;
            for (int i = 0; i < ClassCommon.BaleInDrop; i++) 
            {
                if (baleTable.Rows[i].Field<int>("DropNumber") == DropNumList[0])
                {
                    if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                    {
                        y1Value[i] = baleTable.Rows[i].Field<float>("Moisture");
                    }
                    else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                    {
                        y1Value[i] = baleTable.Rows[i].Field<float>("Weight");
                    }
                }

                LastI = i + 1;
            }
            double[] xs1 = { 1, 2, 3, 4, 5, 6 };
            var bars1 = WpfPlot1.Plot.Add.Bars(xs1, y1Value);
            bars1.LegendText = "Drop 1";

          
            //Drop 2
            double[] y2Value = new double[ClassCommon.BaleInDrop];
            int LastII = LastI;
            for (int i = 0; i < ClassCommon.BaleInDrop; i++)
            {
                if (baleTable.Rows[LastI + i].Field<int>("DropNumber") == DropNumList[1])
                {
                    if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                    {
                        y2Value[i] = baleTable.Rows[LastI + i].Field<float>("Moisture");
                    }
                    else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                    {
                        y2Value[i] = baleTable.Rows[LastI + i].Field<float>("Weight");
                    }
                }
                LastII = LastI + i + 1;
            }
            double[] xs2 = { 8, 9, 10, 11, 12, 13 };
         
            var bars2 = WpfPlot1.Plot.Add.Bars(xs2, y2Value);
            bars2.LegendText = "Drop 2";

            //Drop 3
            double[] y3Value = new double[ClassCommon.BaleInDrop];
            int LastIII = LastII;
            for (int i = 0; i < ClassCommon.BaleInDrop; i++)
            {
                if (baleTable.Rows[LastII + i].Field<int>("DropNumber") == DropNumList[2])
                {
                    if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                    {
                        y3Value[i] = baleTable.Rows[LastII + i].Field<float>("Moisture");
                    }
                    else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                    {
                        y3Value[i] = baleTable.Rows[LastII + i].Field<float>("Weight");
                    }
                }
                LastIII = LastII + i + 1;
            }
            double[] xs3 = { 15, 16, 17, 18, 19, 20 };
            var bars3 = WpfPlot1.Plot.Add.Bars(xs3, y3Value);
            bars3.LegendText = "Drop 3";

            //Drop 4
            double[] y4Value = new double[ClassCommon.BaleInDrop];
            int LastIV = LastIII;
            for (int i = 0; i < ClassCommon.BaleInDrop; i++)
            {
                if (baleTable.Rows[LastIII + i].Field<int>("DropNumber") == DropNumList[3])
                {
                    if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                    {
                        y4Value[i] = baleTable.Rows[LastIII + i].Field<float>("Moisture");
                    }
                    else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                    {
                        y4Value[i] = baleTable.Rows[LastIII + i].Field<float>("Weight");
                    }
                }
                LastIV = LastIII + i + 1;
            }
            double[] xs4 = { 22, 23, 24, 25, 26, 27 };
            var bars4 = WpfPlot1.Plot.Add.Bars(xs4, y4Value);
            bars4.LegendText = "Drop 4";



            //Drop 5
            double[] y5Value = new double[ClassCommon.BaleInDrop];
            int LastV = LastIV;
            for (int i = 0; i < ClassCommon.BaleInDrop; i++)
            {

                if (baleTable.Rows[LastIV + i].Field<int>("DropNumber") == DropNumList[4])
                {
                    if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                    {
                        y5Value[i] = baleTable.Rows[LastIV + i].Field<float>("Moisture");
                    }
                    else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                    {
                        y5Value[i] = baleTable.Rows[LastIV + i].Field<float>("Weight");
                    }
                }
                LastV = LastIV + i + 1;
            }
            double[] xs5 = { 29, 30, 31, 32, 33, 34 };
            var bars5 = WpfPlot1.Plot.Add.Bars(xs5, y5Value);
            bars5.LegendText = "Drop 5";

            WpfPlot1.Refresh();
        }

        internal void SetUpChart(string chartTitle, int graphHeightM, int graphHeightW)
        {
            int GraphWidth = (ClassCommon.BaleInDrop * ClassCommon.DropInChart) + 6;// ClassCommon.BaleInDrop * Number of Drop + 1;

            WpfPlot1.Plot.Clear();
            WpfPlot1.Refresh();
            WpfPlot1.Plot.Axes.AutoScale();
            WpfPlot1.Interaction.Disable();
          

            WpfPlot1.Plot.Axes.SetLimitsX(0, GraphWidth);

            if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
            {
                WpfPlot1.Plot.Axes.SetLimitsY(0, graphHeightM + (graphHeightM * .5));
            }
            else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
            {
                WpfPlot1.Plot.Axes.SetLimitsY(0, graphHeightW + (graphHeightW * .5));
            }

            if (ClassCommon.GraphDarkMode)
            {
                // change figure colors
                WpfPlot1.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#07263b");
                WpfPlot1.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#1f1f1f");

                // change axis and grid colors
                WpfPlot1.Plot.Axes.Color(ScottPlot.Color.FromHex("#d7d7d7"));
                WpfPlot1.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#7A7E7D");
            }
            

        }
    }
}
