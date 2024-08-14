using ModRealTime.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModRealTime
{
    public class ModRealTimeModule : IModule
    {



        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion3", typeof(RealTimeView));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}