using AppServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;

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

        private int xAxisTag = ClassCommon.BaleInDrop;

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

        public void PlotChart_old(DataTable baleTable, string chartTitle, int graphHeightM, int graphHeightW, int graphLowM, int graphLowW)
        {
            ScottPlot.Plot myPlot = new();

            int MinLowM =0;// = graphLowM;
            int MinLowW = 0;// = graphLowW;

            if(graphLowM>0) MinLowM = graphLowM / 3;
            if (graphLowW > 0) MinLowW = graphLowW / 2;

            int GraphWidth = (ClassCommon.BaleInDrop * ClassCommon.DropInChart) + 6;// ClassCommon.BaleInDrop * Number of Drop + 1;

            double[] yValue = new double[ClassCommon.BaleInDrop];

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
                else if (ClassCommon.MenuChecked == ClassCommon.MenuBDWeight)
                {
                    WpfPlot1.Plot.Axes.SetLimitsY(MinLowW, graphHeightW + (graphHeightW *.10));
                }
                else if (ClassCommon.MenuChecked == ClassCommon.MenuADWeight)
                {
                    WpfPlot1.Plot.Axes.SetLimitsY(MinLowW, graphHeightW + (graphHeightW * .10));
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
                        else if (ClassCommon.MenuChecked == ClassCommon.MenuBDWeight)
                        {
                            yValue[i] = baleTable.Rows[i].Field<float>("BDWeight");
                        }
                        else if (ClassCommon.MenuChecked == ClassCommon.MenuADWeight)
                        {
                            yValue[i] = baleTable.Rows[i].Field<float>("BDWeight")/0.9;
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
                        else if (ClassCommon.MenuChecked == ClassCommon.MenuBDWeight)
                        {
                            yValue[i] = baleTable.Rows[LastI + i].Field<float>("BDWeight");
                        }
                        else if (ClassCommon.MenuChecked == ClassCommon.MenuADWeight)
                        {
                            yValue[i] = baleTable.Rows[LastI + i].Field<float>("BDWeight") / 0.9;
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
                        else if (ClassCommon.MenuChecked == ClassCommon.MenuBDWeight)
                        {
                            yValue[i] = baleTable.Rows[LastII + i].Field<float>("BDWeight");
                        }
                        else if (ClassCommon.MenuChecked == ClassCommon.MenuADWeight)
                        {
                            yValue[i] = baleTable.Rows[LastII + i].Field<float>("BDWeight") / 0.9;
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
                            else if (ClassCommon.MenuChecked == ClassCommon.MenuBDWeight)
                            {
                                yValue[i] = baleTable.Rows[LastIII + i].Field<float>("BDWeight");
                            }
                            else if (ClassCommon.MenuChecked == ClassCommon.MenuADWeight)
                            {
                                yValue[i] = baleTable.Rows[LastIII + i].Field<float>("BDWeight") / 0.9;
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
                            else if (ClassCommon.MenuChecked == ClassCommon.MenuBDWeight)
                            {
                                yValue[i] = baleTable.Rows[LastIV + i].Field<float>("BDWeight");
                            }
                            else if (ClassCommon.MenuChecked == ClassCommon.MenuADWeight)
                            {
                                yValue[i] = baleTable.Rows[LastIV + i].Field<float>("BDWeight") / 0.9;
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
           // WpfPlot1.Interaction.Disable();

            WpfPlot1.Plot.Axes.SetLimitsY(graphHeightW, graphHeightM + (graphHeightM / 2));

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

        internal void PlotChartbyDrop(int dropNum,double[] XAxisTag, double[] yValue)
        {
           
            switch (dropNum) 
            {
                case 1:

                    var bars1 = WpfPlot1.Plot.Add.Bars(XAxisTag, yValue);
                    bars1.LegendText = "Drop 1";

                    break;
                
                case 2:
                    var bars2 = WpfPlot1.Plot.Add.Bars(XAxisTag, yValue);
                    bars2.LegendText = "Drop 2";
                    break;

                case 3:
                    var bars3 = WpfPlot1.Plot.Add.Bars(XAxisTag, yValue);
                    bars3.LegendText = "Drop 3";
                    break;

                case 4:
                    var bars4 = WpfPlot1.Plot.Add.Bars(XAxisTag, yValue);
                    bars4.LegendText = "Drop 4";
                    break;

                case 5:
                    var bars5 = WpfPlot1.Plot.Add.Bars(XAxisTag, yValue);
                    bars5.LegendText = "Drop 5";
                    break;
            }


            WpfPlot1.Refresh();

        }

        internal void SetUpGraph(string chartTitlex, int graphHeightM, int graphHeightW, int graphLowM, int graphLowW)
        {
            string ChartTitle = string.Empty;


            WpfPlot1.Plot.Clear();
            WpfPlot1.Refresh();
            WpfPlot1.Plot.Axes.AutoScale();
            WpfPlot1.Interaction.Disable();
           

            ScottPlot.Plot myPlot = new();
            int MinLowM = graphLowM - (graphLowM/2);
            int MinLowW = graphLowW - (graphLowW/2);


            int GraphWidth = (ClassCommon.BaleInDrop * ClassCommon.DropInChart) + 6;// ClassCommon.BaleInDrop * Number of Drop + 1;

            if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
            {
                WpfPlot1.Plot.Axes.SetLimitsY(MinLowM, graphHeightM + (graphHeightM / 2));
                ChartTitle = $"Bar Chart of Moisture % in {ClassCommon.DropInChart} Consecutive Drops, Newest Drop on the left";
            }
            else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
            {
                WpfPlot1.Plot.Axes.SetLimitsY(MinLowW, graphHeightW + (graphHeightW / 4));
                ChartTitle = $"Bar Chart of Weight in {ClassCommon.DropInChart} Consecutive Drops, Newest Drop on the left";
            }
            else if (ClassCommon.MenuChecked == ClassCommon.MenuBDWeight)
            {
                WpfPlot1.Plot.Axes.SetLimitsY(MinLowW, graphHeightW + (graphHeightW * .05));
                ChartTitle = $"Bar Chart of Bone Dry Weight in  {ClassCommon.DropInChart} Consecutive Drops, Newest Drop on the left";
                Debug.WriteLine($"Min val = {MinLowW} High val = {graphHeightW + (graphHeightW * .05)}");
            }
            else if (ClassCommon.MenuChecked == ClassCommon.MenuADWeight)
            {
                WpfPlot1.Plot.Axes.SetLimitsY(MinLowW, graphHeightW + (graphHeightW * .05));
                ChartTitle = $"Bar Chart of Air Dry Weight in {ClassCommon.DropInChart} Consecutive Drops, Newest Drop on the left"; 
                Debug.WriteLine($"Min val = {MinLowW} High val = {graphHeightW + (graphHeightW * .05)}");
            }


            //   double[] yValue = new double[ClassCommon.BaleInDrop];

            WpfPlot1.Plot.Title(ChartTitle);
            WpfPlot1.Plot.Axes.SetLimitsX(0, GraphWidth);
          

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
           

        }
    }
}
