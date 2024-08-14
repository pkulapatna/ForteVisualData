using CSVReports.ViewModels;
using Prism.Events;
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
using System.Windows.Shapes;

namespace CSVReports.Views
{
    /// <summary>
    /// Interaction logic for CSVReport.xaml
    /// </summary>
    public partial class CSVReport : Window, IDisposable
    {
        protected readonly IEventAggregator _eventAggregator;
        private CSVReportViewModel _cSVReportViewModel;

        public static CSVReport CSVDialog;


        public CSVReport(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            CSVDialog = this;
            _eventAggregator = eventAggregator;
            _cSVReportViewModel = new (_eventAggregator);
            DataContext = _cSVReportViewModel;

            if (_cSVReportViewModel.CloseAction == null)
                _cSVReportViewModel.CloseAction = new Action(this.Close);
        }

        public void Dispose()
        {
            
        }

        public void InitCsv(DataTable MyData, string strtable, int strStart, int strEnd)
        {
            _cSVReportViewModel.MyDataTable = MyData;
            _cSVReportViewModel.StrFileName = strtable;
            _cSVReportViewModel.StrPathFile = _cSVReportViewModel.StrFileLocation + "\\" + _cSVReportViewModel.StrFileName + ".csv";
            _cSVReportViewModel.FindCreateDir(_cSVReportViewModel.StrFileLocation);
        }




    }
}
