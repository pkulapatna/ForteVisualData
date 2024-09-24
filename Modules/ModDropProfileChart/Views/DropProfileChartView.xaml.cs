using AppServices;
using ScottPlot;
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

        private string ChartTitle = string.Empty;

        private ScottPlot.Color colorSnowWhite = ScottPlot.Color.FromHex("FFFAFA");
        private ScottPlot.Color colorWhite = ScottPlot.Color.FromHex("FFFFFF");
        private ScottPlot.Color colorBlack = ScottPlot.Color.FromHex("000000");

        private ScottPlot.Color colorBlue = ScottPlot.Color.FromHex("0088cc");
        private ScottPlot.Color colorOrange = ScottPlot.Color.FromHex("ff884d");
        private ScottPlot.Color colorGreen = ScottPlot.Color.FromHex("00b300");
        private ScottPlot.Color colorRed = ScottPlot.Color.FromHex("e60000");
        private ScottPlot.Color colorPurple = ScottPlot.Color.FromHex("b399ff");

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

        [Obsolete]
        public void PlotChart(DataTable baleTable, int graphHeightM, int graphHeightW, int graphLowM, int graphLowW)
        {
            ScottPlot.Plot myPlot = new();

            int MinLowM =0;// = graphLowM;
            int MinLowW = 0;// = graphLowW;

            if(graphLowM>0) MinLowM = graphLowM / 3;
            if (graphLowW > 0) MinLowW = graphLowW / 2;

            int GraphWidth = (ClassCommon.BaleInDrop * ClassCommon.DropInChart) + 6;// ClassCommon.BaleInDrop * Number of Drop + 1;

            double[] yValue = new double[ClassCommon.BaleInDrop];
            Tick[] ticks = new Tick[5];

            try
            {
                WpfPlot1.Plot.Clear();
                WpfPlot1.Refresh();

                if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                {
                    ChartTitle = $"Bar Chart of Moisture % in {ClassCommon.DropInChart} Consecutive Drops, Newest Drop on the left";
                    WpfPlot1.Plot.Axes.SetLimitsY(MinLowM, graphHeightM + (graphHeightM / 2));
                }
                else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                {
                    ChartTitle = $"Bar Chart of Weight in {ClassCommon.DropInChart} Consecutive Drops, Newest Drop on the left";
                    WpfPlot1.Plot.Axes.SetLimitsY(MinLowW, graphHeightW + (graphHeightW / 4));
                }
                else if (ClassCommon.MenuChecked == ClassCommon.MenuBDWeight)
                {
                    ChartTitle = $"Bar Chart of Bone Dry Weight in  {ClassCommon.DropInChart} Consecutive Drops, Newest Drop on the left";
                    WpfPlot1.Plot.Axes.SetLimitsY(MinLowW, graphHeightW + (graphHeightW *.05));
                }
                else if (ClassCommon.MenuChecked == ClassCommon.MenuADWeight)
                {
                    ChartTitle = $"Bar Chart of Air Dry Weight in {ClassCommon.DropInChart} Consecutive Drops, Newest Drop on the left";
                    WpfPlot1.Plot.Axes.SetLimitsY(MinLowW, graphHeightW + (graphHeightW * .05));
                }

                WpfPlot1.Plot.Axes.SetLimitsX(0, GraphWidth);
                WpfPlot1.Plot.Axes.AutoScale();
                WpfPlot1.Interaction.Disable();
                WpfPlot1.Plot.Title(ChartTitle);

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

                        // add a label and customize it as desired
                        var txt = WpfPlot1.Plot.Add.Text(yValue[i].ToString("00.0"), i+1, yValue[i]);
                        txt.Color = ClassCommon.GraphDarkMode ? colorSnowWhite : colorBlack;
                        txt.Alignment = Alignment.LowerCenter;
                        txt.FontSize = 12;
                        txt.Bold= true;
                    }
                    LastI = i + 1;
                }
                var bars1 = WpfPlot1.Plot.Add.Bars(SetXAxisTag(1), yValue);
                bars1.LegendText = "Drop 1";
                ticks[0] = new Tick(3, "Drop No 1");
               
                foreach (var bar in bars1.Bars)
                {
                    bar.BorderLineWidth = 2;
                    bar.FillColor = colorBlue;
                    bar.BorderColor = ClassCommon.GraphDarkMode ? colorWhite : colorBlack;
                }
               
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

                        // add a label and customize it as desired
                        var txt = WpfPlot1.Plot.Add.Text(yValue[i].ToString("00.0"), LastI+i + 2, yValue[i]);
                        txt.Color = ClassCommon.GraphDarkMode ? colorSnowWhite : colorBlack;
                        txt.Alignment = Alignment.LowerCenter;
                        txt.FontSize = 12;
                        txt.Bold = true;
                    }
                    LastII = LastI + i + 1;
                }
                var bars2 = WpfPlot1.Plot.Add.Bars(SetXAxisTag(2), yValue);
                bars2.LegendText = "Drop 2";
                ticks[1] = new Tick(LastI + 4, "Drop No 2");
                foreach (var bar in bars2.Bars)
                {
                    bar.FillColor = colorOrange;
                    bar.BorderLineWidth = 2;
                    bar.BorderColor = ClassCommon.GraphDarkMode ? colorWhite : colorBlack;
                }

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

                        // add a label and customize it as desired
                        var txt = WpfPlot1.Plot.Add.Text(yValue[i].ToString("00.0"), LastII + i + 3, yValue[i]);
                        txt.Color = ClassCommon.GraphDarkMode ? colorSnowWhite : colorBlack;
                        txt.Alignment = Alignment.LowerCenter;
                        txt.FontSize = 12;
                        txt.Bold = true;
                    }
                    LastIII = LastII + i + 1;
                }
                var bars3 = WpfPlot1.Plot.Add.Bars(SetXAxisTag(3), yValue);
                bars3.LegendText = "Drop 3";
                ticks[2] = new Tick(LastII + 5, "Drop No 3");
                foreach (var bar in bars3.Bars)
                {
                    bar.FillColor = colorGreen;
                    bar.BorderLineWidth = 2;
                    bar.BorderColor = ClassCommon.GraphDarkMode ? colorWhite : colorBlack;
                }

               

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

                            // add a label and customize it as desired
                            var txt = WpfPlot1.Plot.Add.Text(yValue[i].ToString("00.0"), LastIII + i + 4, yValue[i]);
                            txt.Color = ClassCommon.GraphDarkMode ? colorSnowWhite : colorBlack;
                            txt.Alignment = Alignment.LowerCenter;
                            txt.FontSize = 12;
                            txt.Bold = true;
                        }
                        LastIV = LastIII + i + 1;
                    }
                    var bars4 = WpfPlot1.Plot.Add.Bars(SetXAxisTag(4), yValue);
                    bars4.LegendText = "Drop 4";
                    ticks[3] = new Tick(LastIII + 6, "Drop No 4");
                    foreach (var bar in bars4.Bars)
                    {
                        bar.FillColor = colorRed;
                        bar.BorderLineWidth = 2;
                        bar.BorderColor = ClassCommon.GraphDarkMode ? colorWhite : colorBlack;
                    }
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

                            // add a label and customize it as desired
                            var txt = WpfPlot1.Plot.Add.Text(yValue[i].ToString("00.0"), LastIV+i + 5, yValue[i]);
                            txt.Color = ClassCommon.GraphDarkMode ? colorSnowWhite : colorBlack;
                            txt.Alignment = Alignment.LowerCenter;
                            txt.FontSize = 12;
                            txt.Bold = true;
                        }
                        LastV = LastIV + i + 1;
                    }
                    var bars5 = WpfPlot1.Plot.Add.Bars(SetXAxisTag(5), yValue);
                    bars5.LegendText = "Drop 5";
                    ticks[4] = new Tick(LastIV + 7, "Drop No 5");
                    foreach (var bar in bars5.Bars)
                    {
                        bar.FillColor = colorPurple; 
                        bar.BorderLineWidth = 2;
                        bar.BorderColor = ClassCommon.GraphDarkMode ? colorWhite : colorBlack;
                    }
                }

                WpfPlot1.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
                WpfPlot1.Plot.Axes.Bottom.MajorTickStyle.Length = 0;
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

       // Tick[] ticks = new Tick[5];

       
        internal void SetUpGraph(string chartTitlex, int graphHeightM, int graphHeightW, int graphLowM, int graphLowW)
        {
            

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
