using AppServices;
using Microsoft.VisualBasic.Logging;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WinForms = System.Windows.Forms;

namespace SystemInfo.ViewModels
{
    public  class SysInfoViewModel : BindableBase
    {

        protected readonly IEventAggregator _eventAggregator;
        private IEventAggregator eventAggregator;

        private string Debugpath = @"c:\ForteDebug\";


        private bool _wetLayerExist =  ClassCommon.WetLayerCheck;
        public bool WetLayerExist
        {
            get { return _wetLayerExist; }
            set { SetProperty(ref _wetLayerExist, value); }
        }


        private string _strFileLocation = @"c:\ForteDebug\";
        public string StrFileLocation
        {
            get { return _strFileLocation; }
            set { SetProperty(ref _strFileLocation, value); }
        }



        private string _zipFileLocation = @"c:\temp";
        public string ZipFileLocation
        {
            get { return _zipFileLocation; }
            set { SetProperty(ref _zipFileLocation, value); }
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

        private bool _bCSVCanwrite = false;
        public bool BCSVCanwrite
        {
            get { return _bCSVCanwrite; }
            set { SetProperty(ref _bCSVCanwrite, value); }
        }

        private string[] IniDatLines;

        #region  Local Wetlaye values from ini file

        private int _maxSampleBale;
        public int MaxSampleBale
        {
            get { return _maxSampleBale; }
            set { SetProperty(ref _maxSampleBale, value); }
        }

        private int _numberLayers;
        public int NumberLayers
        {
            get { return _numberLayers; }
            set { SetProperty(ref _numberLayers, value); }
        }

        private int _charStartCut;
        public int CharStartCut
        {
            get { return _charStartCut; }
            set { SetProperty(ref _charStartCut, value); }
        }

        private int _charEndCut;
        public int CharEndCut
        {
            get { return _charEndCut; }
            set { SetProperty(ref _charEndCut, value); }
        }

        private double _cycleTime;
        public double CycleTime
        {
            get { return _cycleTime; }
            set { SetProperty(ref _cycleTime, value); }
        }

        private int _sampleEntrance;
        public int SampleEntrance
        {
            get { return _sampleEntrance; }
            set { SetProperty(ref _sampleEntrance, value); }
        }

        private int _sampleExit;
        public int SampleExit
        {
            get { return _sampleExit; }
            set { SetProperty(ref _sampleExit, value); }
        }

        //Movement
        private int _sensorDistanceMM;
        public int SensorDistanceMM
        {
            get { return _sensorDistanceMM; }
            set { SetProperty(ref _sensorDistanceMM, value); }
        }
        private int _wLProcessTO;
        public int WLProcessTO
        {
            get { return _wLProcessTO; }
            set { SetProperty(ref _wLProcessTO, value); }
        }
        private int _rTRequestTO;
        public int RTRequestTO
        {
            get { return _rTRequestTO; }
            set { SetProperty(ref _rTRequestTO, value); }
        }
        private int _bbaleSpeedMaxMMPerSec;
        public int BaleSpeedMaxMMPerSec
        {
            get { return _bbaleSpeedMaxMMPerSec; }
            set { SetProperty(ref _bbaleSpeedMaxMMPerSec, value); }
        }
        private int _baleSpeedMinMMPerSec;
        public int BaleSpeedMinMMPerSec
        {
            get { return _baleSpeedMinMMPerSec; }
            set { SetProperty(ref _baleSpeedMinMMPerSec, value); }
        }
        private int _baleLengthMaxMM;
        public int BaleLengthMaxMM
        {
            get { return _baleLengthMaxMM; }
            set { SetProperty(ref _baleLengthMaxMM, value); }
        }

        private int _baleLengthMinMM;
        public int BaleLengthMinMM
        {
            get { return _baleLengthMinMM; }
            set { SetProperty(ref _baleLengthMinMM, value); }
        }

        private Visibility _showWLSetup = Visibility.Hidden;
        public Visibility ShowWLSetup
        {
            get { return _showWLSetup; }
            set { SetProperty(ref _showWLSetup, value); }
        }

        #endregion  Local Wetlaye values from ini file


        public SysInfoViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            
            if(WetLayerExist)
            {
                ShowWLSetup = Visibility.Visible;
                getLocalWetLayerSetUp();
            }              
        }


        private void getLocalWetLayerSetUp()
        {
            StrFileLocation = @"c:\ForteDebug\";
            StrFileName = $"WetLayerIni.csv";
            ZipFileLocation = @"C:\temp";

            ClsIniOptions WetLayerini = new();

            WetLayerini.readinifile();
            IniDatLines = WetLayerini.IniDatLines;

            if (IniDatLines.Length > 1) BCSVCanwrite = true;
            else BCSVCanwrite = false;

            MaxSampleBale = WetLayerini.iMaxSamples;
            SampleEntrance = WetLayerini.iHeadLen;
            SampleExit = WetLayerini.iTailLen;

            //Restoring group
            NumberLayers = WetLayerini.iRestoreLayers;
            CharStartCut = WetLayerini.iLayersToChopStart;
            CharEndCut = WetLayerini.iLayersToChopEnd;

            //Movement
            CycleTime = WetLayerini.iCycleMSec;
            SensorDistanceMM = WetLayerini.iSensorDistanceMM;
            WLProcessTO = WetLayerini.iWLProcessTO;
            RTRequestTO = WetLayerini.iRTRequestTO;

            BaleSpeedMaxMMPerSec = WetLayerini.iBaleSpeedMaxMMPerSec;
            BaleSpeedMinMMPerSec = WetLayerini.iBaleSpeedMinMMPerSec;
            BaleLengthMaxMM = WetLayerini.iBaleLengthMaxMM;
            BaleLengthMinMM = WetLayerini.iBaleLengthMinMM;

        }

        private void SetUpWetLayer()
        {
            ClsIniOptions WetLayerini = new();
            WetLayerini.readinifile();
            IniDatLines = WetLayerini.IniDatLines;
        }

        private DelegateCommand _writeZipCommand;
        public DelegateCommand WriteZipCommand =>
        _writeZipCommand ?? (_writeZipCommand = new DelegateCommand(WriteZipExecute).ObservesCanExecute(() => BCSVCanwrite));
        private void WriteZipExecute()
        {
            bool bGood = false;

            CreateFolder(Debugpath);

            //Delete all files from the target directory first. 
            System.IO.DirectoryInfo di = new DirectoryInfo(Debugpath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            //Check for Wet Layer System
            if (ClassCommon.WetLayerCheck)
            {
                WriteCSVWLiniFile();
            }

            if (CopyLogFiles()) bGood = true;
            if (CopyBackUpfiles()) bGood = true;

            if(File.Exists(ZipFileLocation + @"\ForteDebug.zip"))
            {
                File.Delete(ZipFileLocation + @"\ForteDebug.zip");
            }
            ZipFile.CreateFromDirectory(@"c:\ForteDebug", ZipFileLocation +  @"\ForteDebug.zip");

            if(bGood) MessageBox.Show("Create Zip file Done.");
        }

        private bool CopyLogFiles()
        {
            bool bCopy = false;
            string LogPath = @"c:\ForteSystem\Realtime\ASCIILog\";

            try
            {

                System.IO.DirectoryInfo di = new DirectoryInfo(LogPath);
                FileInfo[] files = di.GetFiles().OrderByDescending(p => p.LastWriteTime).ToArray();
                List<FileInfo> logFiles = new List<FileInfo>();

                for (int i =0 ; i < files.Length; i++)
                {
                    if (files[i].Name.Contains("RealTime"))
                    {
                        logFiles.Add(files[i]);

                        if (logFiles.Count < 5) 
                        {
                            File.Copy(files[i].ToString(), Debugpath + Path.GetFileName(files[i].ToString()));
                        }
                    }      
                }
                bCopy = true;
            }
            catch (Exception ex )
            {
                MessageBox.Show("Error in CopyLogFiles " + ex);
            }
            return bCopy;
        }


        private bool CopyBackUpfiles() 
        {
            bool bDone = false;

            string basePath = @"c:\ForteSystem\";

            string archivePath = basePath + @"\Archives\";
            string calPath = basePath + @"\Calibrations\";
            string gradePath = basePath + @"\Grades\";
            string realtimePath = basePath + @"\Realtime\";
            string reportsPath = basePath + @"\Reports\";
            string SystemPath = basePath + @"\System\";
            string SecurityPath = basePath + @"\Security\";

            try
            {

                File.Copy(archivePath + @"\OutputSys.mdb", Debugpath + Path.GetFileName(archivePath + @"\OutputSys.mdb"));

                File.Copy(calPath + @"\Calibrate.mdb", Debugpath + Path.GetFileName(calPath + @"\Calibrate.mdb"));

                File.Copy(gradePath + @"\PulpGrade.mdb", Debugpath + Path.GetFileName(gradePath + @"\PulpGrade.mdb"));

                File.Copy(realtimePath + @"\7760.mdb", Debugpath + Path.GetFileName(realtimePath + @"\7760.mdb"));
                File.Copy(realtimePath + @"\Cfg7760.mdb", Debugpath + Path.GetFileName(realtimePath + @"\Cfg7760.mdb"));
                File.Copy(realtimePath + @"\CustomConfig.mdb", Debugpath + Path.GetFileName(realtimePath + @"\CustomConfig.mdb"));

                File.Copy(reportsPath + @"\LVFormats.mdb", Debugpath + Path.GetFileName(reportsPath + @"\LVFormats.mdb"));
                File.Copy(reportsPath + @"\MarkerFormats.mdb", Debugpath + Path.GetFileName(reportsPath + @"\MarkerFormats.mdb"));
                File.Copy(reportsPath + @"\PrinterFormats.mdb", Debugpath + Path.GetFileName(reportsPath + @"\PrinterFormats.mdb"));
                File.Copy(reportsPath + @"\ReportsJ4.mdb", Debugpath + Path.GetFileName(reportsPath + @"\ReportsJ4.mdb"));

                File.Copy(SystemPath + @"\FSys.ini", Debugpath + Path.GetFileName(SystemPath + @"\FSys.ini"));
                File.Copy(SecurityPath + @"\UPG.DAT", Debugpath + Path.GetFileName(SecurityPath + @"\UPG.DAT"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in CopyBackUpfiles " + ex);                
            }
            return bDone;
        }



        private void GetAccessTable()
        {
          
            StrPathFile = Debugpath + "\\BaleArchive.csv";

            CreateFolder(Debugpath);

            ClassAccessHandler accessHandler = ClassAccessHandler.Instance;
            DataTable dt = accessHandler.GetAccessArchiveTable2();

            if (dt.Rows.Count > 0)
            {
                StreamWriter outFile = new StreamWriter(StrPathFile);
                List<string> headerValues = new List<string>();
                foreach (DataColumn column in dt.Columns)
                {
                    headerValues.Add(QuoteValue("'" + column.ColumnName));
                }
                //Header
                outFile.WriteLine(string.Join(",", headerValues.ToArray()));

                foreach (DataRow row in dt.Rows)
                {
                    string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                    outFile.WriteLine(String.Join(",", fields));
                }
                outFile.Close();
            }
        }

        public string QuoteValue(string value)
        {
            return string.Concat("" + value + "");
        }


        private void CreateFolder(string path)
        {
            try
            {
                // Determine whether the directory exists.
                if (Directory.Exists(path))
                {
                    Debug.WriteLine("That path exists already.");
                    return;
                }
                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                Debug.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in CreateFolder " + ex);
            }
        }


        private DelegateCommand _writeCommand;
        public DelegateCommand WriteCommand =>
        _writeCommand ?? (_writeCommand = new DelegateCommand(WriteExecute).ObservesCanExecute(() => BCSVCanwrite));

        private void WriteExecute()
        {
          //  WriteCSVWLiniFile();
        }

        private bool WriteCSVWLiniFile()
        {
            bool bDone = false;

            try
            {
                StrPathFile = Debugpath + @"\" + StrFileName;
                SaveArrayAsCSV(IniDatLines, StrPathFile);
                bDone = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in WriteCSVWLiniFile " + ex);
            }
            return  bDone;
        }

        public static void SaveArrayAsCSV<T>(T[] arrayToSave, string fileName)
        {
            using (StreamWriter file = new StreamWriter(fileName))
            {
                foreach (T item in arrayToSave)
                {
                    file.WriteLine(item + ",");
                }
            }
        }

        private DelegateCommand _browseCommand;
        public DelegateCommand BrowseCommand =>
        _browseCommand ?? (_browseCommand = new DelegateCommand(BrowseExecute));
        private void BrowseExecute()
        {
            try
            {
                using (WinForms.FolderBrowserDialog dlg = new WinForms.FolderBrowserDialog())
                {
                    if (dlg.ShowDialog() == WinForms.DialogResult.OK)
                    {
                        dlg.InitialDirectory = ZipFileLocation;
                        ZipFileLocation = dlg.SelectedPath;
                    }
                    FindCreateDir(ZipFileLocation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in BrowseExecute " + ex);
            }
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
               // BCSVCanwrite = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in findCreateDir " + ex);
            }
        }
    }
}
