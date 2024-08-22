using AppServices;
using ModDualGraph.Properties;
using Prism.Events;
using ScottPlot;
using ScottPlot.AxisLimitManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModDualGraph.Views
{
    /// <summary>
    /// Interaction logic for DualGraphView.xaml
    /// </summary>
    public partial class DualGraphView : UserControl
    {
        protected readonly IEventAggregator _eventAggregator;
        public static DualGraphView _dualGraphView;

        private readonly Storyboard myStoryboard = new Storyboard();

        public DualGraphView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            _dualGraphView = this;
            b1c0.Visibility = Visibility.Hidden;
        }

        public void PlotChartOne(double[] dataX, double[] dataY, string chartTitle, string yTitle)
        {

            List<double> MData = new List<double>();
            List<double> WData = new List<double>();

         
            WpfPlot1.Plot.Clear();

            WpfPlot1.Plot.Axes.Bottom.Label.Text = $"Consecutive Number of Bales";
            WpfPlot1.Plot.Axes.Left.Label.Text = "Vertical Axis";
            WpfPlot1.Plot.Title(chartTitle);
            WpfPlot1.Plot.YLabel(yTitle);

            WpfPlot1.Plot.Axes.AutoScale();
            WpfPlot1.Plot.Axes.SetLimitsX(1, ClassCommon.ComBineSample);

           
            ScottPlot.Palettes.Category10 palette = new();

            try
            {
                WpfPlot1.Interaction.Disable();

                for (int i = 0; i < ClassCommon.ComBineSample; i++)
                {
                    MData.Add(dataY[i]);
                }
                double mMax = MData.Max();
                double mMin = MData.Min();

                txtGphHi1.Text = mMax.ToString("#0.00");
                txtavrg1.Text = MData.Average().ToString("00.00");
                txtGphLow1.Text = mMin.ToString("#0.00");

                txtbox1.Text = dataY[0].ToString("#0.00");

                var cross = WpfPlot1.Plot.Add.Crosshair(ClassCommon.ComBineSample, MData.Average());
                cross.LineWidth = 2;
                cross.LineColor = ScottPlot.Colors.Gray;
                // each line's styles can be individually accessed as well
                cross.HorizontalLine.LinePattern = LinePattern.Dotted;

                WpfPlot1.Plot.Axes.SetLimitsY(mMin - 2, mMax + 2);


                if (ClassCommon.GraphDarkMode)
                {
                    // change figure colors
                    WpfPlot1.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#07263b");
                    WpfPlot1.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#1f1f1f");

                    // change axis and grid colors
                    WpfPlot1.Plot.Axes.Color(ScottPlot.Color.FromHex("#d7d7d7"));
                    WpfPlot1.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#7A7E7D");
                }
                  

                var sp = WpfPlot1.Plot.Add.Scatter(dataX, dataY);
                sp.LineWidth = 2;
                sp.MarkerSize = 8;
                sp.Color = palette.GetColor(3);
                //  WpfPlot2.Plot.Render();
                WpfPlot1.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in PlotChartOne {ex.Message}");
            }

        }

        public void PlotChartTwo(double[] dataX, double[] dataY, string chartTitle, string yTitle)
        {

            List<double> MData = new List<double>();
            List<double> WData = new List<double>();

            ScottPlot.Palettes.Category10 palette = new();

            WpfPlot2.Plot.Clear();

            WpfPlot2.Plot.Axes.Bottom.Label.Text = $"Consecutive Number of Bales";
            WpfPlot2.Plot.Axes.Left.Label.Text = "Vertical Axis";
            WpfPlot2.Plot.Title(chartTitle);
            WpfPlot2.Plot.YLabel(yTitle);

            WpfPlot2.Plot.Axes.AutoScale();
            WpfPlot2.Plot.Axes.SetLimitsX(1, ClassCommon.ComBineSample);

            try
            {
                WpfPlot2.Interaction.Disable();


                for (int i = 0; i < ClassCommon.ComBineSample; i++)
                {
                    MData.Add(dataY[i]);
                }
                double mMax = MData.Max();
                double mMin = MData.Min();

                txtGphHi2.Text = mMax.ToString("#0.00");
                txtAvg2.Text = MData.Average().ToString("00.00");
                txtGphLow2.Text = mMin.ToString("#0.00");

                txtbox2.Text = dataY[0].ToString("#0.00");

                var cross = WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, MData.Average());
                cross.LineWidth = 2;
                cross.LineColor = ScottPlot.Colors.Gray;
                // each line's styles can be individually accessed as well
                cross.HorizontalLine.LinePattern = LinePattern.Dotted;


                //    WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphOneLimitHi);
                //    WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphOneLimitLo);
                WpfPlot2.Plot.Axes.SetLimitsY(mMin - mMin/10, mMax + mMax/10);

                if (ClassCommon.GraphDarkMode)
                {
                    // change figure colors
                    WpfPlot2.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#07263b");
                    WpfPlot2.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#1f1f1f");

                    // change axis and grid colors
                    WpfPlot2.Plot.Axes.Color(ScottPlot.Color.FromHex("#d7d7d7"));
                    WpfPlot2.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#7A7E7D");
                }


                var sp = WpfPlot2.Plot.Add.Scatter(dataX, dataY);
                sp.LineWidth = 2;
                sp.MarkerSize = 8;
                sp.Color = palette.GetColor(0);
                WpfPlot2.Refresh();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in PlotChartTwo {ex.Message}");
                
            }
        }

        internal void MoveBaleOne(int iBaleNum)
        {
            SetBaleProp(iBaleNum);

            DoubleAnimation myDoubleAnimation = new DoubleAnimation
            {
                From = 0,
                To = 240,
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

        internal void ShowStartBtn(bool bHide)
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

        private void TextBox_SizeChange(object sender, SizeChangedEventArgs e)
        {
            double xwidth = e.NewSize.Width * .30; //.25

            txtbox1.FontSize = xwidth;
            txtbox2.FontSize = xwidth;
          

        }

        public void Clearbale()
        {
            b1c0.Visibility = Visibility.Hidden;
        }

       
    }
}
