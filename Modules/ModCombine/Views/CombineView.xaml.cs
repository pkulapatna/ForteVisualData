using Prism.Events;
using AppServices;
using ModCombine.Properties;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Windows.Media.Animation;
using System;

namespace ModCombine.Views
{
    /// <summary>
    /// Interaction logic for CombineView.xaml
    /// </summary>
    public partial class CombineView : UserControl
    {
        protected readonly IEventAggregator _eventAggregator;
        private double IScreenWidth { get; set; }
        private double wdCoef = 0.0;
        public static CombineView _combineView;
        private readonly Storyboard myStoryboard = new Storyboard();

      
        public CombineView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            _combineView = this;
            b1c0.Visibility = Visibility.Hidden;
        }


        public void PlotChart(double[] dataX, double[] dataY, string chartTitle, string yTitle)
        {
            List<double> MData = new List<double>();
            List<double> WData = new List<double>();

            WpfPlot2.Plot.Clear();

            WpfPlot2.Plot.Axes.Bottom.Label.Text = "Number of Bales";
            WpfPlot2.Plot.Axes.Left.Label.Text = "Vertical Axis";
            WpfPlot2.Plot.Title(chartTitle);
            WpfPlot2.Plot.YLabel(yTitle);

            WpfPlot2.Plot.Axes.AutoScale();
            WpfPlot2.Plot.Axes.SetLimitsX(1, ClassCommon.ComBineSample);

            WpfPlot2.Interaction.Disable();


            if (ClassCommon.GraphDarkMode)
            {
                // change figure colors
                WpfPlot2.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#07263b");
                WpfPlot2.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#1f1f1f");

                // change axis and grid colors
                WpfPlot2.Plot.Axes.Color(ScottPlot.Color.FromHex("#d7d7d7"));
                WpfPlot2.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#7A7E7D");
            }

            for (int i = 0; i < ClassCommon.ComBineSample; i++)
            {
                MData.Add(dataY[i]);
            }

            try
            {
                if (MData.Max() > 0)
                {
                    if (ClassCommon.MenuChecked == ClassCommon.MenuMoisture)
                    {
                        //  for (int i = 0; i < ClassCommon.ComBineSample; i++)
                        //  {
                        //      MData.Add(dataY[i]);
                        //   }
                        double mMax = MData.Max();
                        double mMin = MData.Min();

                        WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphOneLimitHi);
                        WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphOneLimitLo);
                        WpfPlot2.Plot.Axes.SetLimitsY(mMin - 2, mMax + 2);
                    }
                    else if (ClassCommon.MenuChecked == ClassCommon.MenuWeight)
                    {
                        for (int i = 0; i < ClassCommon.ComBineSample; i++)
                        {
                            WData.Add(dataY[i]);
                        }

                        double wMax = WData.Max();
                        double wMin = WData.Min();


                        WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphTwoLimitHi);
                        WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphTwoLimitLo);
                        WpfPlot2.Plot.Axes.SetLimitsY(wMin - 10, wMax + 10);
                    }

                    else if (ClassCommon.MenuChecked == ClassCommon.MenuBDWeight)
                    {
                        for (int i = 0; i < ClassCommon.ComBineSample; i++)
                        {
                            WData.Add(dataY[i]);
                        }

                        double wMax = WData.Max();
                        double wMin = WData.Min();


                        WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphThreeLimitHi);
                        WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphThreeLimitLo);
                        WpfPlot2.Plot.Axes.SetLimitsY(wMin - 10, wMax + 10);
                    }
                    else if (ClassCommon.MenuChecked == ClassCommon.MenuADWeight)
                    {
                        for (int i = 0; i < ClassCommon.ComBineSample; i++)
                        {
                            WData.Add(dataY[i]);
                        }

                        double wMax = WData.Max();
                        double wMin = WData.Min();

                        WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphFourLimitHi);
                        WpfPlot2.Plot.Add.Crosshair(ClassCommon.ComBineSample, Settings.Default.GraphFourLimitLo);
                        WpfPlot2.Plot.Axes.SetLimitsY(wMin - 10, wMax + 10);
                    }
                    var sp = WpfPlot2.Plot.Add.Scatter(dataX, dataY);

                    sp.LineWidth = 2;
                    sp.MarkerSize = 8;

                    // WpfPlot2.Plot.Render();
                    WpfPlot2.Refresh();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in PlotChart {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in PlotChart < {ex.Message}");
                
            }


        
        }



        private void GridView_sidechanged(object sender, SizeChangedEventArgs e)
        {
            IScreenWidth = e.NewSize.Width + 10;
            wdCoef = e.NewSize.Width * 0.08;

            double dxGvHdrSize = e.NewSize.Width * .029;
            double dxGvRwHeight = e.NewSize.Width * .025;
            double dxGvFontSz = e.NewSize.Width * .012;
            double dCmbHeight = e.NewSize.Width * .02;
            // double txtBxHeight = e.NewSize.Width * .030;

            RTGridView.FontSize = dxGvFontSz;
            RTGridView.ColumnHeaderHeight = dxGvHdrSize;
            RTGridView.RowHeight = dxGvRwHeight;
            RTGridView.UpdateLayout();
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            RTGridView.Columns[0].Visibility = Visibility.Collapsed;

            if (e.PropertyName.Equals("Moisture"))
            {
                e.Column.Header = ClassCommon.MoistureUnitLst[ClassCommon.MoistureType];
            }
            if (e.PropertyName.Equals("Weight"))
            {
                e.Column.Header = ClassCommon.WeightTypeLst[ClassCommon.WeightUnit];
            }


            if (e.PropertyName.StartsWith("Deviation"))
                e.Column.Header = "%CV";

            if (e.PropertyName.StartsWith("Finish"))
                e.Column.Header = "Viscosity";

            if (e.PropertyName.StartsWith("FC_LotIdentString"))
                e.Column.Header = "CusLotNumber";

            if (e.PropertyName.StartsWith("CalibrationName"))
                e.Column.Header = "Calibrration";

            if (ClassCommon.WLOptions)
            {
                if (e.PropertyName.StartsWith("SpareSngFld3"))
                    e.Column.Header = "%CV";
            }
            if ((e.PropertyType == typeof(System.Single)) || (e.PropertyType == typeof(System.Double)))
            {
                e.Column.ClipboardContentBinding.StringFormat = "{0:0.##}";
                e.Column.Width = e.Column.Header.ToString().Length + wdCoef;
                //e.Column.Width = 110;
            }
            else if (e.PropertyType == typeof(System.DateTime))
            {
                e.Column.ClipboardContentBinding.StringFormat = "MM-dd-yyyy HH:mm";
                e.Column.Width = e.Column.Header.ToString().Length + wdCoef * 1.2;
            }
            else
                e.Column.Width = e.Column.Header.ToString().Length + wdCoef;


            //RTGridView.SelectedIndex = 0;
            //RTGridView.Focus();

        }

        private void TextLoLim_dclick(object sender, MouseButtonEventArgs e)
        {

        }

        private void TextHiLim_dclick(object sender, MouseButtonEventArgs e)
        {

        }

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
        }

        private void SampleBox_dclick(object sender, MouseButtonEventArgs e)
        {
           
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

        private void TextBox_SizeChange(object sender, SizeChangedEventArgs e)
        {
            double xwidth = e.NewSize.Width * .25;

            txtbox1.FontSize = xwidth;
            txtbox2.FontSize = xwidth;
            txtbox3.FontSize = xwidth;
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

        public void ShowStartBtn(bool bHide)
        {
            if(bHide)
            btnStart.Opacity = 1;
            else btnStart.Opacity = .3;
        }

        public void ShowStopBtn(bool bHide)
        {
            if (bHide)
                btnStop.Opacity = 1;
            else btnStop.Opacity = .3;
        }

    }
}
