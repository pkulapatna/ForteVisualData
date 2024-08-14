using AppServices;
using DataFieldsSelect.ViewModels;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DataFieldsSelect.Views
{
    /// <summary>
    /// Interaction logic for FieldSelectView.xaml
    /// </summary>
    public partial class FieldSelectView : Window
    {
        protected readonly IEventAggregator _eventAggregator;

        private FieldSelectViewModel _fieldSelectViewModel;

        private bool btnSaveClicked = false;

        public FieldSelectView(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            _fieldSelectViewModel = new FieldSelectViewModel(_eventAggregator);
            DataContext = _fieldSelectViewModel;

            if (_fieldSelectViewModel.CloseAction == null)
                _fieldSelectViewModel.CloseAction = new Action(this.Close);

            btnSaveClicked = false;
        }



        private void RightClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((SelectedHdrList.SelectedIndex > -1) & (SelectedHdrList.SelectedIndex + 1 < SelectedHdrList.Items.Count))
                {
                    ObservableCollection<string> newlist = (ObservableCollection<string>)SelectedHdrList.ItemsSource;
                    int NewIndex = SelectedHdrList.SelectedIndex + 1;
                    object selected = SelectedHdrList.SelectedItem;

                    // Removing removable element ItemsControl.ItemsSource
                    newlist.Remove(selected.ToString());
                    // Insert it in new position
                    newlist.Insert(NewIndex, selected.ToString());

                    _fieldSelectViewModel.SelectedHdrList = newlist;
                    SelectedHdrList.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in RightClick " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in RightClick < {ex.Message}");
            }
        }

        private void LeftClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedHdrList.SelectedIndex > 0)
                {
                    ObservableCollection<string> newlist = (ObservableCollection<string>)SelectedHdrList.ItemsSource;
                    int NewIndex = SelectedHdrList.SelectedIndex - 1;

                    if ((NewIndex > -1) || (NewIndex >= SelectedHdrList.Items.Count))
                    {
                        object selected = SelectedHdrList.SelectedItem;

                        // Removing removable element ItemsControl.ItemsSource
                        newlist.Remove(selected.ToString());
                        // Insert it in new position
                        newlist.Insert(NewIndex, selected.ToString());
                        // Restore selection
                        _fieldSelectViewModel.SelectedHdrList = newlist;

                        //SelectedHdrList.SelectedItem = selected;
                        SelectedHdrList.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERROR in LeftClick " + ex.Message);
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in LeftClick < {ex.Message}");
               
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (btnSaveClicked ==  true)
            {
                if (System.Windows.MessageBox.Show("Fields Modification Saved, Exit ?", "Shutdown Window", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    e.Cancel = true;
                else
                    e.Cancel = false;

                btnSaveClicked = false;
            }
        }

        private void Save_Clicked(object sender, RoutedEventArgs e)
        {
            btnSaveClicked = true;
        }
    }
}
