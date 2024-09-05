using Prism.Events;
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
using SystemInfo.ViewModels;

namespace SystemInfo.Views
{
    /// <summary>
    /// Interaction logic for SysInfoView.xaml
    /// </summary>
    public partial class SysInfoView : Window
    {
        protected readonly IEventAggregator _eventAggregator;

        private SysInfoViewModel _sysInfoViewModel;

        public SysInfoView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            _sysInfoViewModel = new SysInfoViewModel(_eventAggregator);
            DataContext = _sysInfoViewModel;


        }
    }
}
