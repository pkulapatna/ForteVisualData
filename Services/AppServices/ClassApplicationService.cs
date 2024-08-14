using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices
{
    public sealed class ClassApplicationService
    {
        private ClassApplicationService() { }

        private static readonly ClassApplicationService _instance = new ClassApplicationService();
        public static ClassApplicationService Instance { get { return _instance; } }

        private Prism.Events.IEventAggregator _eventAggregator;

        public Prism.Events.IEventAggregator EventAggregator
        {
            get
            {
                if (_eventAggregator == null)
                    _eventAggregator = new Prism.Events.EventAggregator();
                return _eventAggregator;
            }
        }


        public class UpdateAppRunEvents : PubSubEvent<bool> { }

        public class UpdateRealTimeEvents : PubSubEvent<DateTime> { }


        public class UpdateAppCloseEvents : PubSubEvent<bool> { }

        public class SettingsChangedEvents : PubSubEvent<bool> { }

        public class SendMessageEvents : PubSubEvent<string> { }

        public class RestartAppEvents : PubSubEvent<bool> { }

        public class ChangeMenuEvents : PubSubEvent<int> { }

    }
}
