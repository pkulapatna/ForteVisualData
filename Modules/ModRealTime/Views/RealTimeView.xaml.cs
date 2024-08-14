using AppServices;
using ModRealTime.ViewModels;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModRealTime.Views
{
    /// <summary>
    /// Interaction logic for RealTimeView.xaml
    /// </summary>
    public partial class RealTimeView : UserControl
    {

        public static RealTimeView RTWindows;

        protected readonly IEventAggregator _eventAggregator;

        private RealTimeViewModel _realTimeViewModel;

        private readonly Storyboard myStoryboard = new Storyboard();

        private double IScreenWidth { get; set; }

        private double wdCoef = 0.0;

        public RealTimeView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            RTWindows = this;

            b1c0.Visibility = Visibility.Hidden;
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

            if (e.PropertyName.Equals("Deviation"))
                e.Column.Header = "%CV";

            if (e.PropertyName.Equals("Finish"))
                e.Column.Header = "Viscosity";

            if (e.PropertyName.Equals("FC_LotIdentString"))
                e.Column.Header = "CusLotNumber";

            if (e.PropertyName.Equals("CalibrationName"))
                e.Column.Header = "Calibration";

            if (ClassCommon.WLOptions)
            {
                if (e.PropertyName.Equals("SpareSngFld3"))
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


            RTGridView.SelectedIndex = 0;
            RTGridView.Focus();
        }

        private void TextBox_SizeChange(object sender, SizeChangedEventArgs e)
        {
            double xwidth = e.NewSize.Width * .25;

            txtbox1.FontSize = xwidth;
            txtbox2.FontSize = xwidth;
            txtbox3.FontSize = xwidth;
            txtbox4.FontSize = xwidth;
            txtbox5.FontSize = xwidth;
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

            b1c0.DataContext = 1; 

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
            b1c0.Visibility= Visibility.Hidden;
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

    }
}
