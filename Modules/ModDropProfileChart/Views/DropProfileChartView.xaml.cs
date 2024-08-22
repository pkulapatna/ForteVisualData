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

         

            try
            {
                if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                {
                    WpfPlot1.Plot.Axes.SetLimitsY(MinLowM, graphHeightM + (graphHeightM / 2));
                }
                else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                {
                    WpfPlot1.Plot.Axes.SetLimitsY(MinLowW, graphHeightW + (graphHeightW / 4));
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


                double[] yValue = new double[ClassCommon.BaleInDrop];

                double[] xValue = new double[ClassCommon.BaleInDrop];

                //Drop 1
                int LastI = 0;
                for (int i = 0; i < ClassCommon.BaleInDrop; i++)
                {
                    if (baleTable.Rows[i].Field<int>("DropNumber") == DropNumList[0])
                    {
                        if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                        {
                            yValue[i] = baleTable.Rows[i].Field<float>("Moisture");
                        }
                        else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                        {
                            yValue[i] = baleTable.Rows[i].Field<float>("Weight");
                        }
                    }
                    LastI = i + 1;
                }
                var bars1 = WpfPlot1.Plot.Add.Bars(SetXAxisTag(1), yValue);
                bars1.LegendText = "Drop 1";


                //Drop 2
                int LastII = LastI;
                for (int i = 0; i < ClassCommon.BaleInDrop; i++)
                {
                    if (baleTable.Rows[LastI + i].Field<int>("DropNumber") == DropNumList[1])
                    {
                        if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                        {
                            yValue[i] = baleTable.Rows[LastI + i].Field<float>("Moisture");
                        }
                        else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                        {
                            yValue[i] = baleTable.Rows[LastI + i].Field<float>("Weight");
                        }
                    }
                    LastII = LastI + i + 1;
                }
                var bars2 = WpfPlot1.Plot.Add.Bars(SetXAxisTag(2), yValue);
                bars2.LegendText = "Drop 2";

                //Drop 3
                int LastIII = LastII;
                for (int i = 0; i < ClassCommon.BaleInDrop; i++)
                {
                    if (baleTable.Rows[LastII + i].Field<int>("DropNumber") == DropNumList[2])
                    {
                        if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                        {
                            yValue[i] = baleTable.Rows[LastII + i].Field<float>("Moisture");
                        }
                        else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                        {
                            yValue[i] = baleTable.Rows[LastII + i].Field<float>("Weight");
                        }
                    }
                    LastIII = LastII + i + 1;
                }
                var bars3 = WpfPlot1.Plot.Add.Bars(SetXAxisTag(3), yValue);
                bars3.LegendText = "Drop 3";


                //Drop 4
                int LastIV = 0;
                if(ClassCommon.DropInChart >= 4)
                {
                    LastIV = LastIII;
                    for (int i = 0; i < ClassCommon.BaleInDrop; i++)
                    {
                        if (baleTable.Rows[LastIII + i].Field<int>("DropNumber") == DropNumList[3])
                        {
                            if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                            {
                                yValue[i] = baleTable.Rows[LastIII + i].Field<float>("Moisture");
                            }
                            else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                            {
                                yValue[i] = baleTable.Rows[LastIII + i].Field<float>("Weight");
                            }
                        }
                        LastIV = LastIII + i + 1;
                    }
                    var bars4 = WpfPlot1.Plot.Add.Bars(SetXAxisTag(4), yValue);
                    bars4.LegendText = "Drop 4";
                }

                //Drop 5
                int LastV = 0;
                if (ClassCommon.DropInChart == 5)
                {
                    LastV = LastIV;
                    for (int i = 0; i < ClassCommon.BaleInDrop; i++)
                    {

                        if (baleTable.Rows[LastIV + i].Field<int>("DropNumber") == DropNumList[4])
                        {
                            if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                            {
                                yValue[i] = baleTable.Rows[LastIV + i].Field<float>("Moisture");
                            }
                            else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                            {
                                yValue[i] = baleTable.Rows[LastIV + i].Field<float>("Weight");
                            }
                        }
                        LastV = LastIV + i + 1;
                    }
                    var bars5 = WpfPlot1.Plot.Add.Bars(SetXAxisTag(5), yValue);
                    bars5.LegendText = "Drop 5";
                }

                WpfPlot1.Refresh();

            }
            catch (Exception ex )
            {
                System.Windows.MessageBox.Show($"Error in PlotChart  {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in PlotChart  < {ex.Message}"); 
            }
           
        }

        int xAxisTag = ClassCommon.BaleInDrop;

   
        private double[] SetXValue()
        {
            double[] Value = new double[ClassCommon.BaleInDrop];

            for (int i = 0; i < ClassCommon.BaleInDrop; i++)
            {
                Value[i] = i + 1;
            }
            xAxisTag = (int)Value[ClassCommon.BaleInDrop - 1] + 2;

            return Value;
        }


        private double[] SetXAxisTag(int drop)
        {
            double[] Value = new double[ClassCommon.BaleInDrop];

            for (int i = 0; i < ClassCommon.BaleInDrop; i++)
            {
                if(drop == 1)
                {
                    Value[i] = i + 1;
                }
                else
                {
                    Value[i] = xAxisTag + i;
                }
            }
            xAxisTag = (int)Value[ClassCommon.BaleInDrop - 1] + 2;

            return Value;
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
