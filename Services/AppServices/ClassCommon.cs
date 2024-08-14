using AppServices.Properties;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AppServices
{
    public static class ClassCommon
    {


        public static string HostName
        {
            get => Settings.Default.Host;
            set
            {
                Settings.Default.Host = value;
                Settings.Default.Save();
            }
        }


        public static List<string> RtMonth = new()
        {
            "BaleArchiveJan",
            "BaleArchiveFeb",
            "BaleArchiveMar",
            "BaleArchiveApr",
            "BaleArchiveMay",
            "BaleArchiveJun",
            "BaleArchiveJul",
            "BaleArchiveAug",
            "BaleArchiveSep",
            "BaleArchiveOct",
            "BaleArchiveNov",
            "BaleArchiveDec"
        };

        public static List<string> MoistureUnitLst = new()
        {
            "MC %",
            "MR %",
            "AD %",
            "BD %"
        };

        public static List<string> MoistureTypeLst = new()
        {
            "Moisture Content %",
            "Moisture Regain %",
            "Air Dry %",
            "Bone Dry %"
        };


        public static List<string> WeightTypeLst = new()
        {
            "Weight (kg)",
            "Weight (lb)"
        };

        public static List<string> WeightUnitLst = new()
        {
            "Kg",
            "Lb"
        };

        public static ObservableCollection<string> SelectedHdrList = new();

        //Header, ShotUnit, LongUnit
        public static List<Tuple<string, string, string>> MoitureTypes = new()
        {
            new Tuple<string, string, string>("Moisture Content %","MC %","Moisture %" ),
            new Tuple<string, string, string>("Moisture Regain %","MR %","Regain %" ),
            new Tuple<string, string, string>("Air Dry %","AD %","AirDry %" ),
            new Tuple<string, string, string>("Bone Dry %","BD %","BoneDry %" )
        };




        #region Menu
        public static int MenuChecked
        {
            get => Settings.Default.MenuChecked;
            set
            {
                Settings.Default.MenuChecked = value;
                Settings.Default.Save();
            }
        }
        #endregion Menu


        public static bool GraphDarkMode
        {
            get => Settings.Default.UseDefaultFields;
            set
            {
                Settings.Default.UseDefaultFields = value;
                Settings.Default.Save();
            }
        }



        public static bool UseDefaultFields
        {
            get => Settings.Default.UseDefaultFields;
            set
            {
                Settings.Default.UseDefaultFields = value;
                Settings.Default.Save();
            }
        }



        public static int MoistureType
        {
            get => Settings.Default.MoistureType;
            set
            {
                Settings.Default.MoistureType = value;
                Settings.Default.Save();
            }
        }
        public static int MoistureUnit
        {
            get => Settings.Default.MoistureUnit;
            set
            {
                Settings.Default.MoistureUnit = value;
                Settings.Default.Save();
            }
        }

        public static int WeightUnit
        {
            get => Settings.Default.WeightUnit;
            set
            {
                Settings.Default.WeightUnit = value;
                Settings.Default.Save();
            }
        }


        public static int ScanSec
        {
            get => Settings.Default.ScanSec;
            set
            {
                Settings.Default.ScanSec = value;
                Settings.Default.Save();
            }
        }

        public static DateTime ProdDayEnd
        {
            get => Settings.Default.ProdDayEnd;
            set
            {
                Settings.Default.ProdDayEnd = value;
                Settings.Default.Save();
            }
        }



        public static bool WLOptions
        {
            get => Settings.Default.bWLOption;
            set
            {
                Settings.Default.bWLOption = value;
                Settings.Default.Save();
            }
        }

        public static bool DropOption
        {
            get => Settings.Default.bDropOption;
            set
            {
                Settings.Default.bDropOption = value;
                Settings.Default.Save();
            }
        }

        public static int BaleInDrop
        {
            get => Settings.Default.BaleInDrop;
            set
            {
                Settings.Default.BaleInDrop = value;
                Settings.Default.Save();
            }
        }




        public static int DropInChart
        {
            get => Settings.Default.DropInChart;
            set
            {
                Settings.Default.DropInChart = value;
                Settings.Default.Save();
            }
        }



        public static int ComBineSample
        {
            get => Settings.Default.ComBineSample;
            set 
            {
                Settings.Default.ComBineSample = value;
                Settings.Default.Save();
            }
        }

        #region Modules

        public static bool RealTimeCheck
        {
            get => Settings.Default.RealTimeCheck;
            set
            {
                Settings.Default.RealTimeCheck = value;
                Settings.Default.Save();
            }
        }
        public static bool GraphCheck
        {
            get => Settings.Default.GraphCheck;
            set
            {
                Settings.Default.GraphCheck = value;
                Settings.Default.Save();
            }
        }

        public static bool CombineCheck
        {
            get => Settings.Default.CombineCheck;
            set
            {
                Settings.Default.CombineCheck = value;
                Settings.Default.Save();
            }
        }
        public static bool DualGraphCheck
        {
            get => Settings.Default.DualGraphCheck;
            set
            {
                Settings.Default.DualGraphCheck = value;
                Settings.Default.Save();
            }
        }

        public static bool ArchiveCheck
        {
            get => Settings.Default.ArchiveCheck;
            set
            {
                Settings.Default.ArchiveCheck = value;
                Settings.Default.Save();
            }
        }

        public static bool WetLayerCheck
        {
            get => Settings.Default.WetLayerCheck;
            set
            {
                Settings.Default.WetLayerCheck = value;
                Settings.Default.Save();
            }
        }


        public static bool WLTrendCheck
        {
            get => Settings.Default.WLTrendCheck;
            set
            {
                Settings.Default.WLTrendCheck = value;
                Settings.Default.Save();
            }
        }


        public static int WLTopGraph
        {
            get => Settings.Default.WLTopGraph;
            set
            {
                Settings.Default.WLTopGraph = value;
                Settings.Default.Save();
            }
        }

        public static bool DropHitoLow
        {
            get => Settings.Default.DropHitoLow;
            set
            {
                Settings.Default.DropHitoLow = value;
                Settings.Default.Save();
            }
        }

        public static bool DpProfileChecked
        {
            get => Settings.Default.DpProfileChecked;
            set
            {
                Settings.Default.DpProfileChecked = value;
                Settings.Default.Save();
            }
        }

        public static bool DpPostionChecked
        {
            get => Settings.Default.DpPostionChecked;
            set
            {
                Settings.Default.DpPostionChecked = value;
                Settings.Default.Save();
            }
        }


        public const int SetUpTab = 0;
        public const int ArchiveTab = 1;
        public const int RealtimeTab = 2;
        public const int GraphTab = 3;
        public const int CombineTab = 4;
       
        public const int WetLayerTab = 5;
        public const int DualGraphTab = 6;

        public const int DropChartTab = 7;
        public const int DropPositionTab = 8;

        public const int WetLayerTreadTab = 9;

        public static int TabSelected = 0;

        public const int BALE_ARCHIVE = 0;
        public const int LOT_ARCHIVE = 1;
        public const int UNIT_ARCHIVE = 2;
        public const int WET_LAYER = 3;
        public const int QULITY_ARCHIVE = 4;


        #endregion Modules



        #region Menu options

        public const int MenuMoisture = 0;
        public const int MenuWeight = 1;
        public const int MenuBDWeight = 2;


        #endregion Menu options;



        public static int AllTabWidth { get; set; }

        public static string CalulateMoisture(string data, int mtype)
        {
            string Newdata = string.Empty;
            float ftMoisture = 0;

            switch (mtype)
            {
                case 0: // %MC = moisture from Sql database
                    ftMoisture = Convert.ToSingle(data);
                    break;

                case 1: // %MR = Moisture / ( 1- Moisture / 100)
                    ftMoisture = Convert.ToSingle(data) / (1 - Convert.ToSingle(data) / 100);
                    break;

                case 2: // %AD = (100 - moisture) / 0.9
                    ftMoisture = (float)((100 - Convert.ToSingle(data)) / 0.9);

                    break;

                case 3: // %BD = 100 - moisture
                    ftMoisture = 100 - Convert.ToSingle(data);
                    break;
            }
            return ftMoisture.ToString("0.##");
        }


        public static double ConvertWeight(double wValue, int wtype)
        {
            double cValue = 0.0;

            if(wtype == 0) cValue = wValue;
            else if(wtype == 1) cValue = wValue * 2.20462;
            
            return cValue;
        }
       
        public static double ConvertMoisture( double mValue, int mtype)
        {
            double cValue = 0.0;

            switch (mtype)
            {
                case 0: // %MC = moisture from Sql database
                    cValue = mValue;
                    break;

                case 1: // %MR = Moisture / ( 1- Moisture / 100)
                    cValue = mValue / (1 - mValue / 100);
                    break;

                case 2: // %AD = (100 - moisture) / 0.9
                    cValue = (100 - mValue) / 0.9;

                    break;

                case 3: // %BD = 100 - moisture
                    cValue = 100 - mValue;
                    break;
            }
            return cValue;
        }

        /// <summary>
        /// Update moisture type and Weight Unit as selected in settings
        /// </summary>
        /// <param name="baleDatatable"></param>
        public static void UpdataTableUnits(DataTable baleDatatable)
        {
            try
            {
                if (baleDatatable.Columns.Contains("Moisture"))
                {
                    for (int i = 0; i < baleDatatable.Rows.Count; i++)
                    {
                        baleDatatable.Rows[i]["Moisture"] = ConvertMoisture(baleDatatable.Rows[i].Field<float>("Moisture"), ClassCommon.MoistureType);
                    }
                }
                if (baleDatatable.Columns.Contains("Weight"))
                {
                    for (int i = 0; i < baleDatatable.Rows.Count; i++)
                    {
                        baleDatatable.Rows[i]["Weight"] = ConvertWeight(baleDatatable.Rows[i].Field<float>("Weight"), ClassCommon.WeightUnit);
                    }
                }
                baleDatatable.AcceptChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in UpdataTableUnits (ClassCommon) {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in UpdataTableUnits (ClassCommon) < {ex.Message}");
            }
        }


    }
}
