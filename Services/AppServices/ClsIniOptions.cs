using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    public  class ClsIniOptions
    {

        private string strFileName = "WetLayer.ini";
        private string strFilePath = @"C:\Fortesystem\Realtime";

        private enum IniGroup
        {
            Data,
            Movement,
            Port,
            Restoring,
            Simulation,
            Decision,
            Query,
            System,
            Device1,
            Photoeye,
            Unknown
        }
        private IniGroup iDatgroup;

        public List<string> DataGroupList;
        public List<string> MovementGroupList;
        public List<string> PortGroupList;
        public List<string> RestoringList;
        public List<string> SimulationList;
        public List<string> DecisionList;
        public List<string> QueryList;
        public List<string> SystemList;
        public List<string> Device1List;
        public List<string> PhotoeyeList;

        //[data]
        public int iMaxSamples;
        public int iBalesInGrid;
        public int iLogMessages;
        public int iLogFileSize;
        public int iHeadLen;
        public int iTailLen;
        public string sUseASCIISampleFile;
        public int iRecordsInASCIIFile;
        public int iASCIIRecordCount;

        //[Movement]
        public int iSensorDistanceMM;
        public int iCycleMSec;
        public int iBaleSpeedMaxMMPerSec;
        public int iBaleSpeedMinMMPerSec;
        public int iMaxSpeedVar;
        public int iBaleLengthMaxMM;
        public int iBaleLengthMinMM;
        public int iZeroConst;
        public int iScaleConst;
        public int iWLProcessTO;
        public int iSensorTConstMSec;
        public int iRTRequestTO;

        //[Port]
        public int iComPort;
        public string sBaud;
        public int iBits;
        public int iStopBit;
        public string sParity;
        public string sUseExpProtocol;

        //[Restoring]
        public int FilterConstant;
        public double dSourceDerivLimit;
        public double dRestoreDerivLimit;
        public double dSourceCorrMax;
        public double dRestoreCorrMax;
        public double dFilterConstantVar;
        public int iNoiseFilter;
        public int iNumberOfIterations;
        public double dMultCoef;
        public double dAddCoef;
        public int iCautionStep;
        public double dCautionCoef;
        public int iAfterRestoreFilter;
        public string bAutoRestore; //bool
        public double dDiveCoef;
        public double dDiveFilter;
        public int iFilterType;
        public int iRestoreLayers;
        public int iFilterConstIterations;
        public int iValuesIterations;
        public int iFilterConstApprMult;
        public int iValuesApprMult;
        public int iSpeedCorrLevel;
        public int iPartOfLayerChoppedAtEnds;
        public int iLayersForFCCalc;
        public int iBinaryPartitions;
        public string bUseRatioForEqual; //bool
        public int iLayersToChopStart;
        public int iLayersToChopEnd;
        public double dFilterConstLimitRatio;
        public int iMaxSpike;
        public string bUseSpikeFilter; //bool
        public double dSpikeFilter;

        //[Simulation]
        public int iSimSamples;
        public int iSimAmplitude;
        public int iSimTailsPrc;
        public int iSimWetCenter1Prc;
        public int SimWetCenter2Prc;
        public int iSimWetcenter3Prc;
        public int iSimWetWidth1Prc;
        public int iSimWetWidth2Prc;
        public int iSimWetWidth3Prc;
        public int iSimWetAmp1Prc;
        public int iSimWetAmp2Prc;
        public int iSimWetAmp3Prc;
        public int iSimTestFilter;
        public double dSimRestoreFilter;
        public int iSimNoisePrc;
        public double dSimNoiseFilter;
        public int iSimCreateFilter;
        public int iSimFilterType;
        public int iSimAverMoisture;

        //[Decision]
        public int iMaxDeviation;
        public int iMaxValue;
        public int iNumbOfSpots;
        public int iMinSpotValue;
        public int iMinSpotLenth;
        public string bUseDeviation; //bool
        public string bUseValue; //bool
        public string bUseSpots; //bool
        public string sMode;
        public int iHeadSamples;
        public int iEdge;
        public int iExclSpotValue;
        public int iExclSpotLength;
        public int iHighColor;
        public int iLowColor;
        public int iNormColor;
        public int iAlarmColor;
        public int iOKColor;

        //[Query]
        public double dUpdatePeriod;
        public string bAutoUpdate; //bool

        //[System]
        public string bStandAlone; //bool
        public string bBeforeTC; //bool
        public string bUseSQL; //bool
        public int iNumberOfBalers;
        public string sBaler1Name;
        public string sBaler2Name;

        //[Device1]
        //public int iComPort;

        //[Photoeye]
        public int iCalCyclemSec;
        //public int iHeadSamples;



        public ClsIniOptions()
        {

            DataGroupList = new List<string>();
            MovementGroupList = new List<string>();
            PortGroupList = new List<string>();
            RestoringList = new List<string>();
            SimulationList = new List<string>();
            DecisionList = new List<string>();
            QueryList = new List<string>();
            SystemList = new List<string>();
            Device1List = new List<string>();
            PhotoeyeList = new List<string>();


        }

        /// <summary>
        /// Read INI fille into string
        /// </summary>
        /// <returns></returns>
        public bool readinifile()
        {
            try
            {
                if (File.Exists(strFilePath + strFileName))
                {
                    using (StreamReader sr = new StreamReader(strFilePath + strFileName))
                    {
                        GetLinesItems(sr.ReadToEnd());
                    }
                }
                return true;
            }

            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error in readinifile-> " + ex);
                return false;
            }
        }

        /// <summary>
        /// Split String into lines with <CR><LF>
        /// So do not have problem with string all over the place in the file.
        /// </summary>
        /// <param name="strLine"></param>
        private void GetLinesItems(string strLine)
        {
            string[] stringSeparators = { "\r\n" };
            string[] IniLines = strLine.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                foreach (string sLine in IniLines)
                {
                    if ((!sLine.Contains(":")) & (sLine != null) & (sLine != string.Empty))
                    {
                        GetGroupHeader(sLine);
                        GetDataInGroup(iDatgroup, sLine);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error in ProcessLine-> " + ex);
            }
        }

        private void GetGroupHeader(string strLinedat)
        {
            // Console.WriteLine(strLinedat.Trim());

            if (strLinedat.Contains("[Data]"))
                iDatgroup = IniGroup.Data;

            if (strLinedat.Contains("[Movement]"))
                iDatgroup = IniGroup.Movement;

            if (strLinedat.Contains("[Port]"))
                iDatgroup = IniGroup.Port;

            if (strLinedat.Contains("[Restoring]"))
                iDatgroup = IniGroup.Restoring;

            if (strLinedat.Contains("[Simulation]"))
                iDatgroup = IniGroup.Simulation;

            if (strLinedat.Contains("[Decision]"))
                iDatgroup = IniGroup.Decision;

            if (strLinedat.Contains("[Query]"))
                iDatgroup = IniGroup.Query;

            if (strLinedat.Contains("[System]"))
                iDatgroup = IniGroup.System;

            if (strLinedat.Contains("[Device1]"))
                iDatgroup = IniGroup.Device1;

            if (strLinedat.Contains("[Photoeye]"))
                iDatgroup = IniGroup.Photoeye;
        }


        private void GetDataInGroup(IniGroup strLineData, string strIniLineDat)
        {

            switch (strLineData)
            {
                case IniGroup.Data:
                    if ((!strIniLineDat.Contains("[Data]")) & (strIniLineDat != string.Empty))
                        DataGroupList.Add(strIniLineDat.Trim());

                    if (strIniLineDat.Contains("MaxSamples"))
                        iMaxSamples = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("BalesInGrid"))
                        iBalesInGrid = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("LogMessages"))
                        iLogMessages = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("LogFileSize"))
                        iLogFileSize = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("HeadSamples"))
                        iHeadLen = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("TailSamples"))
                        iTailLen = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("UseASCIISampleFile"))
                        sUseASCIISampleFile = StrIniItem(strIniLineDat);

                    if (strIniLineDat.Contains("RecordsInASCIIFile"))
                        iRecordsInASCIIFile = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("ASCIIRecordCount"))
                        iASCIIRecordCount = Convert.ToInt16(StrIniItem(strIniLineDat));

                    break;

                case IniGroup.Movement:
                    if ((!strIniLineDat.Contains("[Movement]")) & (strIniLineDat != string.Empty))
                        MovementGroupList.Add(strIniLineDat.Trim());

                    if (strIniLineDat.Contains("SensorDistanceMM"))
                        iSensorDistanceMM = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("CycleMSec"))
                        iCycleMSec = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("BaleSpeedMaxMMPerSec"))
                        iBaleSpeedMaxMMPerSec = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("BaleSpeedMinMMPerSec"))
                        iBaleSpeedMinMMPerSec = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("MaxSpeedVar"))
                        iMaxSpeedVar = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("BaleLengthMaxMM"))
                        iBaleLengthMaxMM = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("BaleLengthMinMM"))
                        iBaleLengthMinMM = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("ZeroConst"))
                        iZeroConst = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("ScaleConst"))
                        iScaleConst = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("WLProcessTO"))
                        iWLProcessTO = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("SensorTConstMSec"))
                        iSensorTConstMSec = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("RTRequestTO"))
                        iRTRequestTO = Convert.ToInt16(StrIniItem(strIniLineDat));

                    break;

                case IniGroup.Port:
                    if ((!strIniLineDat.Contains("[Port]")) & (strIniLineDat != string.Empty))
                        PortGroupList.Add(strIniLineDat.Trim());

                    if (strIniLineDat.Contains("ComPort"))
                        iComPort = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("Baud"))
                        sBaud = StrIniItem(strIniLineDat);

                    if (strIniLineDat.Contains("Bits"))
                        iBits = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("StopBit"))
                        iStopBit = Convert.ToInt16(StrIniItem(strIniLineDat));

                    if (strIniLineDat.Contains("Parity"))
                        sParity = StrIniItem(strIniLineDat);

                    if (strIniLineDat.Contains("UseExpProtocol"))
                        sUseExpProtocol = StrIniItem(strIniLineDat);

                    break;

                case IniGroup.Restoring:

                    if ((!strIniLineDat.Contains("[Restoring]")) & (strIniLineDat != string.Empty))
                        RestoringList.Add(strIniLineDat.Trim());
                    break;

                case IniGroup.Simulation:
                    if ((!strIniLineDat.Contains("[Simulation]")) & (strIniLineDat != string.Empty))
                        SimulationList.Add(strIniLineDat.Trim());
                    break;


                case IniGroup.Decision:
                    if ((!strIniLineDat.Contains("[Decision]")) & (strIniLineDat != string.Empty))
                        DecisionList.Add(strIniLineDat.Trim());
                    break;


                case IniGroup.Query:
                    if ((!strIniLineDat.Contains("[Query]")) & (strIniLineDat != string.Empty))
                        QueryList.Add(strIniLineDat.Trim());
                    break;

                case IniGroup.System:
                    if ((!strIniLineDat.Contains("[System]")) & (strIniLineDat != string.Empty))
                        SystemList.Add(strIniLineDat.Trim());
                    break;

                case IniGroup.Device1:
                    if ((!strIniLineDat.Contains("[Device1]")) & (strIniLineDat != string.Empty))
                        Device1List.Add(strIniLineDat.Trim());
                    break;

                case IniGroup.Photoeye:
                    if ((!strIniLineDat.Contains("[Photoeye]")) & (strIniLineDat != string.Empty))
                        PhotoeyeList.Add(strIniLineDat.Trim());
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Get value from string after =
        /// </summary>
        /// <param name="strData"></param>
        /// <returns> string value </returns>
        private string StrIniItem(string strData)
        {
            string strRet = string.Empty;
            string[] stringSeparators = { "=" };

            try
            {
                string[] words = strData.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);

                if (words.Length > 1) { strRet = words[1]; }
                else strRet = string.Empty;

                return strRet;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error in StrIniItem-> " + ex);
                return strRet;
            }
        }

    }
}
