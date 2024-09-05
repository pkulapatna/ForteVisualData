using Diagnostic.ViewModels;
using System.Windows;

namespace Diagnostic.Views
{
    /// <summary>
    /// Interaction logic for DiagnosticView.xaml
    /// </summary>
    public partial class DiagnosticView : Window
    {

        protected readonly IEventAggregator _eventAggregator;
        private DiagnosticViewModel _diagnosticViewModel;
        public static DiagnosticView? _diagnosticDialog;


        public DiagnosticView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _diagnosticDialog = this;
            _eventAggregator = eventAggregator;
            _diagnosticViewModel = new DiagnosticViewModel(_eventAggregator);
            this.DataContext = _diagnosticViewModel;

        }

   
    }
}
