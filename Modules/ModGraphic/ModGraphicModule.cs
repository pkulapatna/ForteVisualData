using ModGraphic.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModGraphic
{
    public class ModGraphicModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion4", typeof(GraphicView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}