using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemInfo.ViewModels
{
    public  class SysInfoViewModel : BindableBase
    {

        protected readonly IEventAggregator _eventAggregator;
        private IEventAggregator eventAggregator;

        public SysInfoViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }
    }
}
