using ModSetup.Views;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Unity;

namespace ModSetup
{
    
    public class ModSetupModule : IModule
    {
       

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion1", typeof(SetUpView));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}