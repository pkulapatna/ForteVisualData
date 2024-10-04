using AppServices;
using ModWetLayer.Properties;
using ScottPlot;
using ScottPlot.Palettes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ModWetLayer.Views
{
    /// <summary>
    /// Interaction logic for WetLayerView.xaml
    /// </summary>
    public partial class WetLayerView : UserControl
    {
        public static WetLayerView _wetLayerView;

        private ScottPlot.Color colorSnowWhite = ScottPlot.Color.FromHex("FFFAFA");
        private ScottPlot.Color colorWhite = ScottPlot.Color.FromHex("FFFFFF");
        private ScottPlot.Color colorBlack = ScottPlot.Color.FromHex("000000");

        private ScottPlot.Color colorBlue = ScottPlot.Color.FromHex("0088cc");
        private ScottPlot.Color colorOrange = ScottPlot.Color.FromHex("ff884d");
        private ScottPlot.Color colorGreen = ScottPlot.Color.FromHex("00b300");
        private ScottPlot.Color colorRed = ScottPlot.Color.FromHex("e60000");
        private ScottPlot.Color colorPurple = ScottPlot.Color.FromHex("b399ff");

        private ScottPlot.Color colorBrown = ScottPlot.Color.FromHex("cb8034");

        public WetLayerView()
        {
            InitializeComponent();
            _wetLayerView = this;

            NorColor.Fill = BrushList[Settings.Default.WLNormColorIndex];// Brushes.LightGreen;
            MaxColor.Fill = BrushList[Settings.Default.WLHiColorIndex];  //Brushes.Orange;
            MinColor.Fill = BrushList[Settings.Default.WLLoColorIndex]; //Brushes.LightBlue;
        }

        private List<Brush> BrushList = new List<Brush>
        {
             Brushes.BlanchedAlmond,
                Brushes.Red,
                Brushes.Yellow,
                Brushes.Blue,
                Brushes.Green,
                Brushes.Brown,
                Brushes.Gray,
                Brushes.Purple,
                Brushes.Pink,
                Brushes.Orange,
                Brushes.Olive,
                Brushes.White,
                Brushes.Beige,
                Brushes.SlateGray,
                Brushes.SpringGreen,
                Brushes.LightGreen,
                Brushes.LightSteelBlue,
                Brushes.Salmon,
                Brushes.Azure,
                Brushes.Aqua,
                Brushes.Aquamarine
        };

        List<string> ColorList = new List<string>
        {
            "BlanchedAlmond",
            "Red",
            "Yellow",
            "Blue",
            "Green",
            "Brown",
            "Gray",
            "Puple",
            "Pink",
            "Orange",
            "Olive",
            "White",
            "Beige",
            "SlateGray",
            "SpringGreen",
            "LightGreen",
            "LightSteelBlue",
            "Salmon",
            "Azure",
            "Aqua",
            "Aquamarine"
        };

    public void PlotChart(double[] dataX, double[] dataY, string chartTitle, string yTitle)
    {

            ScottPlot.Palettes.DarkPastel palette = new();
            ScottPlot.Bar[] bars = new Bar[dataX.Length];

            double LimitHiVal = Settings.Default.WLMaxLimit;
            double LimitLoVal = Settings.Default.WLMinLimit; ;

            bool AlarmOn = false; 
          
            List<double> ValLst = new List<double>();
            double ValMax = 0;
            double ValMin = 0;

            Tick[] labels = { new Tick(1,"Layer1"), new Tick(2, "Layer2"), new Tick(3, "Layer3"), new Tick(4, "Layer4"),
                new Tick(5, "Layer5"), new Tick(6, "Layer6") ,new Tick(7, "Layer7"), new Tick(8, "Layer8"),
                new Tick(9, "Layer9"),new Tick(10, "Layer10"),new Tick(11, "Layer11"), new Tick(12, "Layer12"),
                new Tick(13, "Layer13"),new Tick(14, "Layer14"),new Tick(15, "Layer15"), new Tick(16, "Layer16") };


            for (int i = 0; i < dataX.Length; i++)
            {
                ValLst.Add(dataY[i]);
            }

            ValMax = ValLst.Max();
            ValMin = ValLst.Min();



            for (int i = 0; i < dataX.Length; i++)
            {
                var txt = dataY[i].ToString();

               
                if (dataY[i] > LimitHiVal)
                {
                    bars[i] = new() 
                    { 
                        Position = i + 1, 
                        Value = dataY[i], 
                        FillColor = palette.GetColor(1), 
                        Label = txt, 
 
                        BorderLineWidth = 2, 
                        BorderColor = ScottPlot.Color.FromHex("#FFFFFF")
                    };
                    AlarmOn = true;
                }
                else if (dataY[i] < LimitLoVal)
                {
                    bars[i] = new() 
                    { 
                        Position = i + 1, 
                        Value = dataY[i], 
                        FillColor = palette.GetColor(2), 
                        Label = txt, 
                        BorderLineWidth = 2, 
                        BorderColor = ScottPlot.Color.FromHex("#FFFFFF")
                    };
                    AlarmOn = true;
                }
                else
                {
                    bars[i] = new() 
                    { 
                        Position = i + 1, 
                        Value = dataY[i], 
                        FillColor = palette.GetColor(4), 
                        Label= txt, 
                        BorderLineWidth = 2,
                        BorderColor = ScottPlot.Color.FromHex("#FFFFFF")
                    };
                }
            }          
            if (AlarmOn)
            {
                tbAlarm.Background = BrushList[Settings.Default.WLAlarmIndex];
                txtAlarm.Text = "Alarm";
                txtAlarm.Foreground = Brushes.Yellow;
            }
            else
            {
                tbAlarm.Background = BrushList[Settings.Default.WLOkIndex];
                txtAlarm.Text = "OK";
                txtAlarm.Foreground = Brushes.Blue;
            }

            

            if (ClassCommon.GraphDarkMode) 
            {
                // change figure colors
                WpfPlot3.Plot.FigureBackground.Color =  ScottPlot.Color.FromHex("#07263b");
                WpfPlot3.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#1f1f1f");
                // change axis and grid colors
                WpfPlot3.Plot.Axes.Color(ScottPlot.Color.FromHex("#d7d7d7"));
                WpfPlot3.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#7A7E7D");
            }


            WpfPlot3.Plot.Axes.AutoScale();
            WpfPlot3.Plot.Axes.SetLimitsX(0, 17);
           
            WpfPlot3.Plot.Title(chartTitle);
            WpfPlot3.Plot.YLabel(yTitle);
            

            if(Settings.Default.GraphAutoScale)
            {
                if ((ValMin > 0) & (ValMax > 0))
                {
                    WpfPlot3.Plot.Axes.SetLimitsY(ValMin / 2, ValMax + ValMax / 4);
                }
            }
            else
                WpfPlot3.Plot.Axes.SetLimitsY(Settings.Default.WLMinYAxis, Settings.Default.WLMaxYAxis);

            WpfPlot3.Plot.Clear();
            WpfPlot3.Refresh();
            WpfPlot3.Interaction.Disable();
            WpfPlot3.Plot.ShowGrid();
            WpfPlot3.Plot.Add.Bars(bars);

        
            WpfPlot3.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(labels);
            WpfPlot3.Refresh();

        }


        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName.StartsWith("ID"))
                e.Column.Header = "Number";

            if (e.PropertyName.StartsWith("BalerID"))
                e.Column.Header = "Baler";

            if (e.PropertyName.StartsWith("Param1"))
                e.Column.Header = "MAX";

            if (e.PropertyName.StartsWith("Param2"))
                e.Column.Header = "MIN";

            if (e.PropertyName.StartsWith("Deviation"))
                e.Column.Header = "CV";

            if (e.PropertyName.StartsWith("Time1"))
                e.Column.Header = "Inp";

            if (e.PropertyName.StartsWith("Time2"))
                e.Column.Header = "Mdl";

            if (e.PropertyName.StartsWith("Time3"))
                e.Column.Header = "Out";

            if (e.PropertyName.StartsWith("Sample"))
                e.Column.Header = "Total";

            if (e.PropertyName.StartsWith("Layers"))
                e.Column.Visibility = Visibility.Hidden;

            if (e.PropertyName.StartsWith("Moisture"))
                e.Column.Header = ClassCommon.MoistureUnitLst[ClassCommon.MoistureType];
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

        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9.]+");
            return reg.IsMatch(str);
        }
    }
}
