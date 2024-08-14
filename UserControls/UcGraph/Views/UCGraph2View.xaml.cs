using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

namespace UcGraph.Views
{
    /// <summary>
    /// Interaction logic for UCGraph2View.xaml
    /// </summary>
    public partial class UCGraph2View : Window
    {
        public UCGraph2View(DataTable wlDatatable, string chartTitle)
        {
            InitializeComponent();

            PlotChart(wlDatatable, chartTitle);
        }

        private void BtnShowData_Click(object sender, RoutedEventArgs e)
        {

        }


        internal void PlotChart(DataTable myTable, string chartTitle)
        {
            List<double> xData = new();

            double sumOfDerivation = 0;

            if (myTable.Rows.Count > 0)
            {
                CreateCVdatatable(myTable);
                    
                double[] timeX = new double[myTable.Rows.Count];
                double[] dataY = new double[myTable.Rows.Count];

                WpfPlot4.Plot.Clear();

                WpfPlot4.Plot.Axes.Bottom.Label.Text = $"Consecutive {myTable.Rows.Count.ToString()} Bales";
                WpfPlot4.Plot.Axes.Left.Label.Text = "CV %";
                WpfPlot4.Plot.Title(chartTitle);

                WpfPlot4.Plot.Axes.AutoScale();
                WpfPlot4.Plot.Axes.SetLimitsX(1, timeX.Length);
                WpfPlot4.Interaction.Disable();

                try
                {
                    for (int i = 0; i < myTable.Rows.Count; i++)
                    {
                       xData.Add((double)myTable.Rows[i]["Deviation"]);

                        dataY[i] = (double)myTable.Rows[i]["Deviation"];
                        timeX[i] = i;

                        xData.Add(dataY[i]);
                    }
                    double mMax = xData.Max();
                    double mMin = xData.Min();

                    WpfPlot4.Plot.Axes.SetLimitsY(mMin - 2, mMax + 2);
                    WpfPlot4.Plot.Add.Scatter(timeX, dataY);
                    WpfPlot4.Refresh();

                    txtMax.Text = mMax.ToString();
                    txtMin.Text = mMin.ToString();

                    txtAvg.Text = xData.Average().ToString("#0.00");

                    foreach (var Value in xData)
                    {
                        sumOfDerivation += (Value - Convert.ToDouble(xData.Average())) * (Value - Convert.ToDouble(xData.Average()));
                    }
                    double Variance = sumOfDerivation / (xData.Count - 1);

                    txtStd.Text = Math.Sqrt(Variance).ToString("#0.00");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ERROR in PlotChart {ex.Message}");
                }
            }
        }

        private void CreateCVdatatable(DataTable wlDatatable)
        {
            DataTable CVDataTable = new DataTable();
          
            // Declare DataColumn and DataRow variables.
            DataColumn column;

            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.Int32"),
                ColumnName = "Number"
            };
            CVDataTable.Columns.Add(column);

            // Create second column.
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "ReadTime"
            };
            CVDataTable.Columns.Add(column);

            // Create third column.
            column = new DataColumn
            {
                DataType = System.Type.GetType("System.String"),
                ColumnName = "CV Value"
            };
            CVDataTable.Columns.Add(column);


            for(int i=0;i<wlDatatable.Rows.Count;i++)     
            {
                DataRow row = CVDataTable.NewRow();
                CVDataTable.Rows.InsertAt(row, i);
                row["Number"] = wlDatatable.Rows[i]["ID"];
                row["ReadTime"] = wlDatatable.Rows[i]["ReadTime"];
                row["CV Value"] = wlDatatable.Rows[i]["Deviation"];
            }
           
            CVDataTable.AcceptChanges();
            CVtable.DataContext = CVDataTable;
        }
    }
}
