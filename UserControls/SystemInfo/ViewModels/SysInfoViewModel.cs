using AppServices;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Data;
using System.IO;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace SystemInfo.ViewModels
{
    public  class SysInfoViewModel : BindableBase
    {

        protected readonly IEventAggregator _eventAggregator;
        private IEventAggregator eventAggregator;


        private bool _wetLayerExist =  ClassCommon.WetLayerCheck;
        public bool WetLayerExist
        {
            get { return _wetLayerExist; }
            set { SetProperty(ref _wetLayerExist, value); }
        }


        private string _strFileLocation = "C:\\temp";
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
            StrFileLocation = @"C:\temp";
            StrFileName = $"WetLayerIni";

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

            StrFileLocation = @"C:\temp";
            StrFileName = $"WetLayerIni";

            WetLayerini.readinifile();

            IniDatLines = WetLayerini.IniDatLines;

        }


        private DelegateCommand _writeCommand;
        public DelegateCommand WriteCommand =>
        _writeCommand ?? (_writeCommand = new DelegateCommand(WriteExecute).ObservesCanExecute(() => BCSVCanwrite));

        private void WriteExecute()
        {
            string timeNow = DateTime.Now.ToString("MM.dd.yy.H.m");
            StrFileName = $"{StrFileName} {timeNow}.csv";
            StrPathFile = StrFileLocation + @"\" + StrFileName;

            SaveArrayAsCSV(IniDatLines, StrPathFile);

            MessageBox.Show($"Writed CSV file to {StrPathFile} DONE!");
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
                        dlg.InitialDirectory = StrFileLocation;
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
