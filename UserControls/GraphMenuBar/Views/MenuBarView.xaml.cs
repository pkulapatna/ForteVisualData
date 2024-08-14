using AppServices;
using GraphMenuBar.Properties;
using GraphMenuBar.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GraphMenuBar.Views
{
    /// <summary>
    /// Interaction logic for MenuBarView.xaml
    /// </summary>
    public partial class MenuBarView : UserControl
    {
        protected readonly IEventAggregator _eventAggregator;

    
        public MenuBarView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this._eventAggregator = eventAggregator;
            this.DataContext = new MenuBarViewModel(_eventAggregator);
        }

        private void On_PageLoad(object sender, RoutedEventArgs e)
        {
            switch (ClassCommon.MenuChecked)
            {
                case 0:
                    rbOne.IsChecked = true;
                    break;
                case 1:
                    rbTwo.IsChecked = true;
                    break;
                case 2:
                    if(rbThree.IsVisible) rbThree.IsChecked = true;
                    break;
                case 3:
                    if(rbFour.IsVisible) rbFour.IsChecked = true;
                    break;
                case 4:
                    if(rbFive.IsVisible) rbFive.  IsChecked = true;   
                    break;
            }
        }
    }
}
