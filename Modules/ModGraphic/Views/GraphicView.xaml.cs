using AppServices;
using ModGraphic.Properties;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModGraphic.Views
{
    /// <summary>
    /// Interaction logic for GraphicView.xaml
    /// </summary>
    public partial class GraphicView : UserControl
    {
        protected readonly IEventAggregator _eventAggregator;
        public static GraphicView _graphicView;

        private readonly Storyboard myStoryboard = new Storyboard();

        public GraphicView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            _graphicView = this;
            b1c0.Visibility = Visibility.Hidden;
        }

        public  void PlotChart(double[] dataX, double[] dataY, string chartTitle, string yTitle)
        {
            List<double> MData = new List<double>();
            List<double> WData = new List<double>();

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

                if (ClassCommon.GraphDarkMode)
                {
                    // change figure colors
                    WpfPlot2.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#07263b");
                    WpfPlot2.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#1f1f1f");

                    // change axis and grid colors
                    WpfPlot2.Plot.Axes.Color(ScottPlot.Color.FromHex("#d7d7d7"));
                    WpfPlot2.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#7A7E7D");
                }

                if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                {
                    WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphOneLimitHi);
                    WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphOneLimitLo);
                    WpfPlot2.Plot.Axes.SetLimitsY(mMin - 2, mMax + 2);
                }
                else
                {
                    WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphTwoLimitHi);
                    WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphTwoLimitLo);
                    WpfPlot2.Plot.Axes.SetLimitsY(mMin - 2, mMax + 2);
                }


             
                var sp = WpfPlot2.Plot.Add.Scatter(dataX, dataY);

                sp.LineWidth = 2;
                sp.MarkerSize = 10;


                //  WpfPlot2.Plot.Render();
                WpfPlot2.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR in GraphicView PlotChart {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GraphicView PlotChart < {ex.Message}");
            }  
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

        private void GraphHi_dclick(object sender, MouseButtonEventArgs e)
        {
            if (txtGrpHi.IsReadOnly == false)
            {
                txtGrpHi.Background = Brushes.Transparent;
                txtGrpHi.Foreground = Brushes.AntiqueWhite;
                txtGrpHi.IsReadOnly = true;
            }
            else
            {
                txtGrpHi.Background = Brushes.White;
                txtGrpHi.Foreground = Brushes.Black;
                txtGrpHi.IsReadOnly = false;
            }
        }

        private void GraphLo_dclick(object sender, MouseButtonEventArgs e)
        {
            if (txtGrpLo.IsReadOnly == false)
            {
                txtGrpLo.Background = Brushes.Transparent;
                txtGrpLo.Foreground = Brushes.AntiqueWhite;
                txtGrpLo.IsReadOnly = true;
            }
            else
            {
                txtGrpLo.Background = Brushes.White;
                txtGrpLo.Foreground = Brushes.Black;
                txtGrpLo.IsReadOnly = false;
            }
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

        public void Clearbale()
        {
            b1c0.Visibility = Visibility.Hidden;
        }
    }
}
