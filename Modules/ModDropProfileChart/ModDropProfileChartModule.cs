using ModDropProfileChart.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModDropProfileChart
{
    public class ModDropProfileChartModule : IModule
    {
     
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion8", typeof(DropProfileChartView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}