
using AppServices;
using ForteVisualData.ViewModels;
using ForteVisualData.Views;
using ModArchives;
using ModCombine;
using ModDropLineChart;
using ModDropProfileChart;
using ModDualGraph;
using ModGraphic;
using ModRealTime;
using ModSetup;
using ModWetLayer;
using ModWetLayerTrend;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Reflection;
using System.Windows;


namespace ForteVisualData
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private string swVersion = string.Empty;

        private ClsSerilog LogMessage;

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

      


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            swVersion = GetLastModTime();

            LogMessage = new ClsSerilog();
            ClsSerilog.LogMessage(ClsSerilog.INFO, $"-------------------------------------------------");
            ClsSerilog.LogMessage(ClsSerilog.INFO, $"Start Application < {swVersion} >  at {DateTime.Now}");
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return base.CreateModuleCatalog();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {

            try
            {    
                Type SetupModuleType = typeof(ModSetupModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = SetupModuleType.Name,
                    ModuleType = SetupModuleType.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });

                ClsSerilog.LogMessage(ClsSerilog.INFO, $"Init - SetupModule");


                Type RealTimeModuleType = typeof(ModRealTimeModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = RealTimeModuleType.Name,
                    ModuleType = RealTimeModuleType.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });
                ClsSerilog.LogMessage(ClsSerilog.INFO, $"Init - RealTimeModule");

                Type GraphicModuleType = typeof(ModGraphicModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = GraphicModuleType.Name,
                    ModuleType = GraphicModuleType.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });
                ClsSerilog.LogMessage(ClsSerilog.INFO, $"Init - GraphicModule");

                Type CombineModuleType = typeof(ModCombineModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = CombineModuleType.Name,
                    ModuleType = CombineModuleType.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });
                ClsSerilog.LogMessage(ClsSerilog.INFO, $"Init - CombineModule");


                Type ArchiveModuleType = typeof(ModArchivesModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = ArchiveModuleType.Name,
                    ModuleType = ArchiveModuleType.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });
                ClsSerilog.LogMessage(ClsSerilog.INFO, $"Init - ArchiveModule");

                Type WetLayerModuleType = typeof(ModWetLayerModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = WetLayerModuleType.Name,
                    ModuleType = WetLayerModuleType.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });
                ClsSerilog.LogMessage(ClsSerilog.INFO, $"Init - WetLayerModule");


                Type WetLayerTrendModuleType = typeof(ModWetLayerTrendModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = WetLayerTrendModuleType.Name,
                    ModuleType = WetLayerTrendModuleType.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });
                ClsSerilog.LogMessage(ClsSerilog.INFO, $"Init - WetLayerTrendModule");


                Type ModDualGraphType = typeof(ModDualGraphModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = ModDualGraphType.Name,
                    ModuleType = ModDualGraphType.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });
                ClsSerilog.LogMessage(ClsSerilog.INFO, $"Init - DualGraphModule");

                
                Type ModDropProfileCharttype = typeof(ModDropProfileChartModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = ModDropProfileCharttype.Name,
                    ModuleType = ModDropProfileCharttype.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });
                ClsSerilog.LogMessage(ClsSerilog.INFO, $"Init - DropProfileChartModule");



                Type ModDropLineCharttype = typeof(ModDropLineChartModule);
                moduleCatalog.AddModule(new ModuleInfo()
                {
                    ModuleName = ModDropLineCharttype.Name,
                    ModuleType = ModDropLineCharttype.AssemblyQualifiedName,
                    InitializationMode = InitializationMode.OnDemand
                });
                ClsSerilog.LogMessage(ClsSerilog.INFO, $"Init - ModDropLineChartModule");



                moduleCatalog.Initialize();
                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("ERROR in ConfigureModuleCatalog " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in ConfigureModuleCatalog < {ex.Message}");
            }
        }

        private string GetLastModTime()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly.Location);
            DateTime lastModified = fileInfo.LastWriteTime;
            return $"SW.Ver: {lastModified.ToString()}";
        }
    }

}
