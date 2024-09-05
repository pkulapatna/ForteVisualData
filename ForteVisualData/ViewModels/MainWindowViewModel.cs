using AppServices;
using ForteVisualData.HelpInfo;
using ForteVisualData.Properties;
using ModArchives.ViewModels;
using ModCombine.ViewModels;
using ModDropProfileChart;
using ModDualGraph.ViewModels;
using ModGraphic.ViewModels;
using ModRealTime.ViewModels;
using ModSetup.ViewModels;
using ModWetLayer.ViewModels;
using ModDropProfileChart.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Drawing;
using System.Windows;
using Unity;
using static AppServices.ClassApplicationService;
using ModDropLineChart;
using ModDropLineChart.ViewModels;
using ModWetLayerTrend.ViewModels;
using SystemInfo.Views;

namespace ForteVisualData.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;

        private readonly IUnityContainer _container;
        private IModuleManager moduleManager;
        private readonly IRegionManager _regionManager;

        private SetUpViewModel _setUpModel;
        private RealTimeViewModel _realTimeViewModel;
        private GraphicViewModel _graphicViewModel;
        private CombineViewModel _combineViewModel;
        private WetLayerViewModel _wetLayerViewModel;
        private DualGraphViewModel _dualGraphViewModel;
        private DropProfileChartViewModel _dropProfileChartViewModel;
        private DropLineChartViewModel _dropLineChartViewModel;
        private ArchiveViewModel _archiveViewModel;

        private WetLayerTrendViewModel _wetLayerTrendViewModel;


        private ClassSqlHandler _sqlhandler;// = ClassSqlHandler.Instance;

        public static bool WLCSVrunning { get; set; }

        private const int NormalTabHeight = 60;

        public string ProgramVersion
        {
            get { return GetLastModTime(); }
        }

        private string GetLastModTime()
        {

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly.Location);
            DateTime lastModified = fileInfo.LastWriteTime;
            return $"[ SW.Ver: {lastModified.ToString()} ]";
        }

        public string HostName
        {
            get => Settings.Default.HostName;
            set
            {
                Settings.Default.HostName = value;
                Settings.Default.Save();
            }
        }

        private string _mainWindowTitle = $"Forte Data From -> {ClassCommon.HostName}";
        public string MainWindowTitle
        {
            get { return _mainWindowTitle; }
            set { SetProperty(ref _mainWindowTitle, value); }
        }

        private string _appStatus = string.Empty;
        public string AppStatus
        {
            get { return _appStatus; }
            set { SetProperty(ref _appStatus, value); }
        }

        private string _rtUpDate = string.Empty;
        public string RtUpDate
        {
            get { return _rtUpDate; }
            set { SetProperty(ref _rtUpDate, value); }
        }

      
        private string _tapTitle = string.Empty;
        public string TapTitle
        {
            get { return _tapTitle; }
            set { SetProperty(ref _tapTitle, value); }
        }


        private int _tabWidth = ClassCommon.AllTabWidth;
        public int TabWidth
        {
            get { return _tabWidth; }
            set { SetProperty(ref _tabWidth, value); }
        }

        public string TabName { get; set; }



        #region LoadModules

        private void LoadRealTimeSummary(bool value)
        {
            if (value)
            {

                AppStatus = RealTimeViewModel.ModuleName;
                _realTimeViewModel = new RealTimeViewModel(this._eventAggregator);
                _realTimeViewModel.LoadPage();
                LoadModule("ModRealTimeModule");
                RtUpDate = string.Empty;

            }
            else
            {
                _realTimeViewModel.UnloadPage();
                _realTimeViewModel = null;
            }
        }

        private void LoadGraphic(bool value)
        {
            if (value)
            {

                AppStatus = GraphicViewModel.ModuleName;
                _graphicViewModel = new GraphicViewModel(this._eventAggregator);
                _graphicViewModel.LoadPage();
                LoadModule("ModGraphicModule");
                RtUpDate = string.Empty;
            }
            else
            {
                _graphicViewModel.UnloadPage();
                _graphicViewModel = null;
            }
        }

        private void LoadCombine(bool value)
        {
            if (value)
            {

                AppStatus = CombineViewModel.ModuleName;
                _combineViewModel = new CombineViewModel(this._eventAggregator);
                _combineViewModel.LoadPage();
                LoadModule("ModCombineModule");
                RtUpDate = string.Empty;
            }
            else
            {
                _combineViewModel.UnloadPage();
                _combineViewModel = null;
            }
        }

        private void LoadArchive(bool value)
        {
            if (value)
            {
                AppStatus = ArchiveViewModel.ModuleName;
                LoadModule("ModArchivesModule");
                RtUpDate = string.Empty;
            }
            else
            {
                _archiveViewModel.UnLoadPage();
                _archiveViewModel = null;
            }
        }

        private void LoadWetLayer(bool value)
        {
            if (value)
            {
                AppStatus = WetLayerViewModel.ModuleName;
                LoadModule("ModWetLayerModule");
            }
        }


        private void LoadWetLayerTrend(bool value)
        {
            if(value)
            {
                AppStatus = WetLayerTrendViewModel.ModuleName;
                LoadModule("ModWetLayerTrendModule");
            }
        }


        private void LoadDualGraph(bool value)
        {
            if (value)
            {
                AppStatus = DualGraphViewModel.ModuleName;
                LoadModule("ModDualGraphModule");
                _dualGraphViewModel =  new DualGraphViewModel(this._eventAggregator);
            }
            else
            {

            }
        }

        private void LaodDropChart(bool value)
        {
            if (value)
            {
                AppStatus = DropProfileChartViewModel.ModuleName;
                LoadModule("ModDropProfileChartModule");
                _dropProfileChartViewModel = new DropProfileChartViewModel(this._eventAggregator);
            }
            else
            {

            }
        }

        private void LaodDropPosition(bool value)
        {
            if(value)
            {
                AppStatus = DropLineChartViewModel.ModuleName;
                LoadModule("ModDropLineChartModule");
                _dropLineChartViewModel = new DropLineChartViewModel(this._eventAggregator);
            }
            else 
            { 
            
            }
        }

        #endregion LoadModules

        private Visibility _showDiagn = Visibility.Hidden;
        public Visibility ShowDiagn
        {
            get => _showDiagn;
            set => SetProperty(ref _showDiagn, value);
        }


        #region Tab 1

        // Tab 1
        private string _tab1ID = "Setup";
        public string Tab1ID
        {
            get { return _tab1ID; }
            set { SetProperty(ref _tab1ID, value); }
        }

        private bool _selectTapOne = false;
        public bool SelectTapOne
        {
            get => _selectTapOne;
            set 
            { 
                SetProperty(ref _selectTapOne, value); 
                if(value)
                {
                    ClassCommon.TabSelected = ClassCommon.SetUpTab;
                    AppStatus = SetUpViewModel.ModuleName;
                    LoadModule("ModSetupModule");
                    RtUpDate = string.Empty;
                    ShowDiagn = ClassCommon.LocalChecked ? Visibility.Visible : Visibility.Hidden;
                }
                else
                {
                    _setUpModel = null;
                }
            }
        }
        private Visibility _tabOneVisible = Visibility.Visible;
        public Visibility TabOneVisible
        {
            get => _tabOneVisible;
            set =>  SetProperty(ref _tabOneVisible, value); 
        }
        private int _tabOneHeight = NormalTabHeight;
        public int TabOneHeight
        {
            get { return _tabOneHeight; }
            set { SetProperty(ref _tabOneHeight, value); }
        }

        #endregion Tab 1

        #region Tab 2

        // Tab 2
        private string _tab2ID = "Archive";
        public string Tab2ID
        {
            get { return _tab2ID; }
            set { SetProperty(ref _tab2ID, value); }
        }

        private bool _selectTapTwo = false;
        public bool SelectTapTwo
        {
            get => _selectTapTwo;
            set
            {
                SetProperty(ref _selectTapTwo, value);
                if(value)
                {
                    ClassCommon.TabSelected = ClassCommon.ArchiveTab;
                    LoadArchive(value);
                    ShowDiagn = Visibility.Hidden;
                }
            }
        }

        private Visibility _tabTwoVisible = Visibility.Visible;
        public Visibility TabTwoVisible
        {
            get => _tabTwoVisible;
            set => SetProperty(ref _tabTwoVisible, value);
        }

        private int _tabTwoHeight = NormalTabHeight;
        public int TabTwoHeight
        {
            get { return _tabTwoHeight; }
            set { SetProperty(ref _tabTwoHeight, value); }
        }

        #endregion Tab2

        #region Tab 3

        // Tab 3
        private string _tab3ID = "RealTime";
        public string Tab3ID
        {
            get { return _tab3ID; }
            set { SetProperty(ref _tab3ID, value); }
        }

        private bool _selectTapThree = false;
        public bool SelectTapThree
        {
            get => _selectTapThree;
            set
            {
                SetProperty(ref _selectTapThree, value);

                if(value)
                {
                    ClassCommon.TabSelected = ClassCommon.RealtimeTab;
                    LoadRealTimeSummary(value);
                    ShowDiagn = Visibility.Hidden;
                }  
            }
        }

        private Visibility _tabThreeVisible = Visibility.Visible;
        public Visibility TabThreeVisible
        {
            get => _tabThreeVisible;
            set => SetProperty(ref _tabThreeVisible, value);
        }

        private int _tabThreeHeight = NormalTabHeight;
        public int TabThreeHeight
        {
            get { return _tabThreeHeight; }
            set { SetProperty(ref _tabThreeHeight, value); }
        }

        #endregion tab3

        #region tab 4

        // Tab 4
        private string _tab4ID = "Graph";
        public string Tab4ID
        {
            get { return _tab4ID; }
            set { SetProperty(ref _tab4ID, value); }
        }

        private bool _selectTapFour = false;
        public bool SelectTapFour
        {
            get => _selectTapFour;
            set
            {
                SetProperty(ref _selectTapFour, value);
                if(value)
                {
                    ClassCommon.TabSelected = ClassCommon.GraphTab;
                    LoadGraphic(value);
                    ShowDiagn = Visibility.Hidden;
                }
            }
        }
      

        private Visibility _tabFourVisible = Visibility.Visible;
        public Visibility TabFourVisible
        {
            get => _tabFourVisible;
            set => SetProperty(ref _tabFourVisible, value);
        }

        private int _tabFourHeight = NormalTabHeight;
        public int TabFourHeight
        {
            get { return _tabFourHeight; }
            set { SetProperty(ref _tabFourHeight, value); }
        }


        #endregion tab 4

        #region tab 5

        // Tab 5
        private string _tab5ID = "Combine";
        public string Tab5ID 
        {
            get { return _tab5ID; }
            set { SetProperty(ref _tab5ID, value); }
        }


        private bool _selectTapFive = false;
        public bool SelectTapFive
        {
            get => _selectTapFive;
            set
            {
                SetProperty(ref _selectTapFive, value);
                if(value)
                {
                    ClassCommon.TabSelected = ClassCommon.CombineTab;
                    LoadCombine(value);
                    ShowDiagn = Visibility.Hidden;
                }
            }
        }

        private int _tabFiveHeight = NormalTabHeight;
        public int TabFiveHeight
        {
            get { return _tabFiveHeight; }
            set { SetProperty(ref _tabFiveHeight, value); }
        }

        private Visibility _tabFiveVisible = Visibility.Visible;
        public Visibility TabFiveVisible
        {
            get => _tabFiveVisible;
            set => SetProperty(ref _tabFiveVisible, value);
        }

        #endregion tab 5

        #region tab 6

        // Tab 6  ////////////////////////////////////////////////////////////
        private string _tab6ID = "DualGraph";
        public string Tab6ID
        {
            get { return _tab6ID; }
            set { SetProperty(ref _tab6ID, value); }
        }

        private bool _selectTapSix = false;
        public bool SelectTapSix
        {
            get => _selectTapSix;
            set
            {
                SetProperty(ref _selectTapSix, value);
                if (value)
                {
                    ClassCommon.TabSelected = ClassCommon.DualGraphTab;
                    LoadDualGraph(value);
                    ShowDiagn = Visibility.Hidden;
                }
            }
        }

        private int _tabSixHeight = NormalTabHeight;
        public int TabSixHeight
        {
            get { return _tabSixHeight; }
            set { SetProperty(ref _tabSixHeight, value); }
        }

        private Visibility _tabSixVisible = Visibility.Visible;
        public Visibility TabSixVisible
        {
            get => _tabSixVisible;
            set => SetProperty(ref _tabSixVisible, value);
        }

        #endregion tab 6

        #region Tab 7
        /// <summary>
        /// tab7  ////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="modelName"></param>
        private string _tab7ID = "WetLayer";
        public string Tab7ID
        {
            get { return _tab7ID; }
            set { SetProperty(ref _tab7ID, value); }
        }
        private bool _selectTapSeven = false;
        public bool SelectTapSeven
        {
            get => _selectTapSeven;
            set
            {
                SetProperty(ref _selectTapSeven, value);
                if (value)
                {
                    ClassCommon.TabSelected = ClassCommon.WetLayerTab;
                    LoadWetLayer(value);
                    ShowDiagn = Visibility.Hidden;
                }
            }
        }

        private int _tabSevenHeight = NormalTabHeight;
        public int TabSevenHeight
        {
            get { return _tabSevenHeight; }
            set { SetProperty(ref _tabSevenHeight, value); }
        }

        private Visibility _tabSevenVisible = Visibility.Visible;
        public Visibility TabSevenVisible
        {
            get => _tabSevenVisible;
            set => SetProperty(ref _tabSevenVisible, value);
        }
        #endregion Tab 7

        #region Tab 8
        //Tab 8  ////////////////////////////////////////////////////////////
        private string _tab8ID = "DropChart";
        public string Tab8ID
        {
            get { return _tab8ID; }
            set { SetProperty(ref _tab8ID, value); }
        }
        private bool _selectTapEight = false;
        public bool SelectTapEight
        {
            get => _selectTapEight;
            set
            {
                SetProperty(ref _selectTapEight, value);
                if (value)
                {
                    ClassCommon.TabSelected = ClassCommon.DropChartTab;
                    LaodDropChart(value);
                }
            }
        }
        private int _tabEightHeight = NormalTabHeight;
        public int TabEightHeight
        {
            get { return _tabEightHeight; }
            set { SetProperty(ref _tabEightHeight, value); }
        }

        private Visibility _tabEightVisible = Visibility.Visible;
        public Visibility TabEightVisible
        {
            get => _tabEightVisible;
            set => SetProperty(ref _tabEightVisible, value);
        }
        #endregion Tab 8

        #region Tab 9
        //Tab 9  ////////////////////////////////////////////////////////////
        private string _tab9ID = "D-Position";
        public string Tab9ID
        {
            get { return _tab9ID; }
            set { SetProperty(ref _tab9ID, value); }
        }
        private bool _selectTapNine = false;
        public bool SelectTapNine
        {
            get => _selectTapNine;
            set
            {
                SetProperty(ref _selectTapNine, value);
                if (value)
                {
                   ClassCommon.TabSelected = ClassCommon.DropPositionTab;
                   LaodDropPosition(value);
                    ShowDiagn = Visibility.Hidden;
                }
            }
        }

        private int _tabNineHeight = NormalTabHeight;
        public int TabNineHeight
        {
            get { return _tabNineHeight; }
            set { SetProperty(ref _tabNineHeight, value); }
        }
        private Visibility _tabNineVisible = Visibility.Visible;
        public Visibility TabNineVisible
        {
            get => _tabNineVisible;
            set => SetProperty(ref _tabNineVisible, value);
        }

        ////////////////////////////////////////////////////////////
        #endregion Tab 9


        #region Tab 10
        //Tab 9  ////////////////////////////////////////////////////////////
        private string _tab10ID = "WL-Trend";
        public string Tab10ID
        {
            get { return _tab10ID; }
            set { SetProperty(ref _tab10ID, value); }
        }
        private bool _selectTapTen = false;
        public bool SelectTapTen
        {
            get => _selectTapTen;
            set
            {
                SetProperty(ref _selectTapTen, value);
                if (value)
                {
                    ClassCommon.TabSelected = ClassCommon.WetLayerTreadTab;
                    LoadWetLayerTrend(value);
                    ShowDiagn = Visibility.Hidden;
                }
            }
        }

     

        private int _tabTenHeight = NormalTabHeight;
        public int TabTenHeight
        {
            get { return _tabTenHeight; }
            set { SetProperty(ref _tabTenHeight, value); }
        }
        private Visibility _tabTenVisible = Visibility.Visible;
        public Visibility TabTenVisible
        {
            get => _tabTenVisible;
            set => SetProperty(ref _tabTenVisible, value);
        }

        ////////////////////////////////////////////////////////////
        #endregion Tab 10


        private void LoadModule(string modelName)
        {

            this.moduleManager = _container.Resolve<IModuleManager>();
            this.moduleManager.LoadModule(modelName); //run Initialize() of the other module 

            ClassCommon.AllTabWidth = 90;
            TabWidth = ClassCommon.AllTabWidth;
        }

        public MainWindowViewModel( IEventAggregator eventAggregator, IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
            this._eventAggregator = eventAggregator;

              if (_sqlhandler == null) 
                _sqlhandler = ClassSqlHandler.Instance;
           
            Setuptabs();

            _eventAggregator.GetEvent<UpdateAppRunEvents>().Subscribe(UpdateAppRunEvent);
            _eventAggregator.GetEvent<UpdateRealTimeEvents>().Subscribe(UpdateRealTimeData);
            _eventAggregator.GetEvent<SendMessageEvents>().Subscribe(UpdateMessage);
        }

        private void Setuptabs()
        {
            TabTwoHeight = ClassCommon.ArchiveCheck ? NormalTabHeight : 0;
            TabThreeHeight = ClassCommon.RealTimeCheck ? NormalTabHeight : 0;
            TabFourHeight = ClassCommon.GraphCheck ? NormalTabHeight : 0;
            TabFiveHeight = ClassCommon.CombineCheck ? NormalTabHeight : 0;
            TabSixHeight = ClassCommon.DualGraphCheck ? NormalTabHeight : 0;
            
            if (ClassCommon.WLOptions == false)
            {
                TabSevenHeight = 0;
                TabTenHeight = 0;
            }
            else
            {
                TabSevenHeight = ClassCommon.WetLayerCheck ? NormalTabHeight : 0;
                TabTenHeight = ClassCommon.WLTrendCheck ? NormalTabHeight : 0;
            }

            if(ClassCommon.DropOption == false)
            {
                TabEightHeight = 0;
                TabNineHeight = 0;
            }
            else
            {
                TabEightHeight = ClassCommon.DpProfileChecked ? NormalTabHeight : 0;
                TabNineHeight = ClassCommon.DpPostionChecked ? NormalTabHeight : 0;
            }
        }


        private void UpdateMessage(string obj)
        {
            if (obj == "Clear") RtUpDate = string.Empty;
            else
                RtUpDate = $"Query Return {obj}";
            }

        private void UpdateRealTimeData(DateTime time)
        {
            RtUpDate = $"Last Bale Data @ {time}";
        }

        private void UpdateAppRunEvent(bool obj)
        {
            if(obj) TabWidth = 0;
            else TabWidth = 90;

        }

        private DelegateCommand _setupCommand;
        public DelegateCommand SetupCommand =>
            _setupCommand ?? (_setupCommand = new DelegateCommand(SetupExecute));
        private void SetupExecute()
        {
            SysInfoView _sysInfoView = new(_eventAggregator);
            _sysInfoView.ShowDialog();
        }

        private DelegateCommand _infoCommand;
        public DelegateCommand InfoCommand =>
            _infoCommand ?? (_infoCommand = new DelegateCommand(InfoExecute));
        private void InfoExecute()
        {
            switch (ClassCommon.TabSelected)
            {
                case ClassCommon.SetUpTab:

                    SetUpWindow setUpWindow = new ()
                    {
                        Topmost = true,
                    };
                    setUpWindow.ShowDialog();
                    break;

                case ClassCommon.RealtimeTab:

                    RealTimeWindow realTimeWindow = new()
                    {
                        Topmost = true,
                    };
                    realTimeWindow.ShowDialog();
                    break;

                case ClassCommon.GraphTab:

                    GraphWindow graphWindow = new()
                    {
                        Topmost = true,
                    };
                    graphWindow.ShowDialog();
                    break;

                case ClassCommon.CombineTab:

                    CombineWindow combineWindow = new()
                    {
                        Topmost = true,
                    };
                    combineWindow.ShowDialog();

                    break;

                case ClassCommon.ArchiveTab:

                    ArchiveWindow archiveWindow = new()
                    {
                        Topmost = true,
                    };
                    archiveWindow.ShowDialog();

                    break;

                case ClassCommon.WetLayerTab:
                    WetLayerWindow wetLayerWindow = new();
                    wetLayerWindow.Topmost = true;
                    wetLayerWindow.ShowDialog();
                    break;


                case ClassCommon.WetLayerTreadTab:
                    WLTrendWindow wLTrendWindow = new();
                    wLTrendWindow.Topmost = true;
                    wLTrendWindow.ShowDialog();
                    break;

                case ClassCommon.DualGraphTab:
                    DualGraphWindow dualGraphWindow = new();
                    dualGraphWindow.Topmost = true;
                    dualGraphWindow.ShowDialog();
                    break;

                case ClassCommon.DropChartTab:
                    DropChartWindow dropChartWindow = new();
                    dropChartWindow.Topmost = true; 
                    dropChartWindow.ShowDialog();
                    break;

                case ClassCommon.DropPositionTab:
                    DropPositionWindow dropPositionWindow = new();
                    dropPositionWindow.Topmost = true; 
                    dropPositionWindow.ShowDialog();
                    break;
            }
        }

        public void CaptureScreen()
        {
            double screenLeft = SystemParameters.VirtualScreenLeft;
            double screenTop = SystemParameters.VirtualScreenTop;
            double screenWidth = SystemParameters.VirtualScreenWidth;
            double screenHeight = SystemParameters.VirtualScreenHeight;

            using (Bitmap bmp = new Bitmap((int)screenWidth,
                    (int)screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    String filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
                  //  Opacity = .0;
                    g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bmp.Size);
                    bmp.Save("C:\\temp\\" + filename);
                  //  Opacity = 1;
                }
            }
        }
    }
}
