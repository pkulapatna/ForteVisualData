using ModDropLineChart.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModDropLineChart
{
    public class ModDropLineChartModule : IModule
    {
        private IEventAggregator eventAggregator;

        

        public void OnInitialized(IContainerProvider containerProvider)
        {

            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion9", typeof(DropLineChartView));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}