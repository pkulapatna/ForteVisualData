using ModDualGraph.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModDualGraph
{
    public class ModDualGraphModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion6", typeof(DualGraphView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}