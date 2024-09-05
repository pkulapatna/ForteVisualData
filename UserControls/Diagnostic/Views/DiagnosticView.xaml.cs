using Diagnostic.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
