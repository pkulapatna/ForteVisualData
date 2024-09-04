using CSVReports.Properties;
using CSVReports.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSVReports.ViewModels
{
    internal class CSVReportViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;

        public Action CloseAction { get; set; }

        private string _strFileLocation;
        public string StrFileLocation
        {
            get { return _strFileLocation; }
            set { SetProperty(ref _strFileLocation, value); }
        }
        private string _StrFileName;
        public string StrFileName
        {
            get { return _StrFileName; }
            set { SetProperty(ref _StrFileName, value); }
        }

        private string _strPathFile;
        public string StrPathFile
        {
            get { return _strPathFile; }
            set { SetProperty(ref _strPathFile, value); }
        }


        private bool _bCSVCanwrite;
        public bool BCSVCanwrite
        {
            get { return _bCSVCanwrite; }
            set { SetProperty(ref _bCSVCanwrite, value); }
        }

        public DataTable MyDataTable;

        public CSVReportViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            StrFileLocation = Settings.Default.CsvFileLocation;
            FindCreateDir(StrFileLocation);
           

        }

        public string QuoteValue(string value)
        {
            return string.Concat("" + value + "");
        }

        public void FindCreateDir(string dirname)
        {
            try
            {
                if (!Directory.Exists(dirname))
                {
                    DirectoryInfo Di = Directory.CreateDirectory(dirname);
                    Di.Attributes = FileAttributes.ReadOnly;
                    Di.Refresh();
                }
                BCSVCanwrite = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in findCreateDir " + ex);
            }
        }


        private DelegateCommand _browseCommand;
        public DelegateCommand BrowseCommand => 
        _browseCommand ?? (_browseCommand = new DelegateCommand(BrowseExecute));
        private void BrowseExecute()
        {
            try
            {
                using (System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog())
                {

                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        StrFileLocation = dlg.SelectedPath;
                    }
                    FindCreateDir(StrFileLocation);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in BrowseExecute " + ex);
            }
        }


        private DelegateCommand _writeCommand;
        public DelegateCommand WriteCommand =>
        _writeCommand ?? (_writeCommand = new DelegateCommand(WriteExecute).ObservesCanExecute(() => BCSVCanwrite));
        private void WriteExecute()
        {
            DataTable xDatatable = MyDataTable;
            StrPathFile = StrFileLocation + "\\" + StrFileName + ".csv";

            try
            {
                if (MyDataTable.Rows.Count > 0)
                {

                    StreamWriter outFile = new StreamWriter(StrPathFile);

                    List<string> headerValues = new List<string>();
                    foreach (DataColumn column in MyDataTable.Columns)
                    {
                        headerValues.Add(QuoteValue("'" + column.ColumnName));
                    }

                    //Header
                    outFile.WriteLine(string.Join(",", headerValues.ToArray()));

                    foreach (DataRow row in MyDataTable.Rows)
                    {
                        string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                        outFile.WriteLine(String.Join(",", fields));
                    }

                    outFile.Close();
                }

                //At the end
                Settings.Default.CsvFileLocation = StrFileLocation;
                Settings.Default.Save();
                BCSVCanwrite = false;
               
                if (CSVReport.CSVDialog != null)
                {
                    CSVReport.CSVDialog.Close();
                    CSVReport.CSVDialog = null;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in WriteExecute " + ex);
            }

            finally
            {
                MessageBox.Show("DONE!");
               
            }
        }
    }
}
