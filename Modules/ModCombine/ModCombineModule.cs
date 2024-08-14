using ModCombine.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModCombine
{
    public class ModCombineModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion5", typeof(CombineView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}