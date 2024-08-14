using ModWetLayer.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModWetLayer
{
    public class ModWetLayerModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion7", typeof(WetLayerView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}