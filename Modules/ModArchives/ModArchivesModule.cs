using ModArchives.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace ModArchives
{
    public class ModArchivesModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion2", typeof(ArchiveView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}