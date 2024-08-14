using ModWetLayerTrend.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModWetLayerTrend
{
    public class ModWetLayerTrendModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion10", typeof(WetLayerTrendView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}