using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostic.ViewModels
{
    public class DiagnosticViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;

        public Action CloseAction { get; set; }

        public DiagnosticViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
    }
}
