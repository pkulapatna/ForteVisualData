using AppServices;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace DataFieldsSelect.ViewModels
{
    public class FieldSelectViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;
        private ClassSqlHandler _sqlhandler;
        private readonly ClassXml MyXml;
        public Action CloseAction { get; set; }

        private ObservableCollection<CheckedListItem> _hdrListboxList;
        public ObservableCollection<CheckedListItem> AvailableHdrList
        {
            get { return _hdrListboxList; }
            set { SetProperty(ref _hdrListboxList, value); }
        }

        private ObservableCollection<string> _selectedHdrList;
        public ObservableCollection<string> SelectedHdrList
        {
            get { return _selectedHdrList; }
            set { SetProperty(ref _selectedHdrList, value); }
        }

        private List<string> sqlHdrList;
        public List<string> SqlHdrList
        {
            get { return sqlHdrList; }
            set { SetProperty(ref sqlHdrList, value); }
        }

        private bool _bModSetup;
        public bool BModifySetup
        {
            get { return _bModSetup; }
            set { SetProperty(ref _bModSetup, value); }
        }

        private bool _bOpenSetup = false;
        public bool OpenSetup
        {
            get { return _bOpenSetup; }
            set { SetProperty(ref _bOpenSetup, value); }
        }

      
        public string SettingsDirectory
        { get { return System.AppDomain.CurrentDomain.BaseDirectory; } }


        private DelegateCommand _modifyCommand;
        public DelegateCommand ModifyCommand =>
            _modifyCommand ?? (_modifyCommand = new DelegateCommand(ModifyExecute));
        private void ModifyExecute()
        {
            BModifySetup = true;
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new DelegateCommand(CancelExecute));

        private void CancelExecute()
        {
            BModifySetup = false;
            CloseAction();
        }

        private DelegateCommand _onCheckCommand;
        public DelegateCommand OnCheckCommand =>
            _onCheckCommand ?? (_onCheckCommand = new DelegateCommand(OnCheckExecute).ObservesCanExecute(() => BModifySetup));

        private void OnCheckExecute()
        {
            ObservableCollection<string> NewList = new();
            ObservableCollection<string> orgList = SelectedHdrList;

            for (int i = 0; i < AvailableHdrList.Count; i++)
            {
                if (AvailableHdrList[i].IsChecked == true) NewList.Add(AvailableHdrList[i].Name);
            }

            if (orgList.Count > NewList.Count) //Remove item
            {
                IEnumerable<string> ItemRemove = orgList.Except(NewList);
                SelectedHdrList = RemoveHdrItem(orgList, ItemRemove.ElementAt(0).ToString());
            }
            else //add item
            {
                IEnumerable<string> ItemAdd = NewList.Except(orgList);
                SelectedHdrList = AddHdrItem(orgList, ItemAdd.ElementAt(0).ToString());
            }
        }

        private DelegateCommand _saveUpdateCommand;
        public DelegateCommand SaveUpdateCommand =>
            _saveUpdateCommand ?? (_saveUpdateCommand = new DelegateCommand(SaveUpdateExecute).ObservesCanExecute(() => BModifySetup));
        private void SaveUpdateExecute()
        {
            SaveModified_setting();
            SaveXmlcolumnList(SelectedHdrList);
            CloseAction();
            BModifySetup = false;
        }

        private void SaveXmlcolumnList(ObservableCollection<string> selectedHdrList)
        {
            MyXml.UpdateXMlcolumnList(selectedHdrList, MyXml.XMLHdrFilePath);
        }

        private void SaveModified_setting()
        {
            List<CheckedListItem> CustomHdrList = new();

            foreach (var item in AvailableHdrList)
            {
                if (item.IsChecked)
                    CustomHdrList.Add(new CheckedListItem(item.Id, item.Name, item.IsChecked, item.FieldType));
            }
            MyXml.WriteXmlGridView(CustomHdrList, MyXml.XMLHdrFilePath);

            CustomHdrList.Clear();

        }

        public FieldSelectViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            
            if(_sqlhandler == null)
                _sqlhandler = ClassSqlHandler.Instance;
            
            MyXml = new ();
            BModifySetup = false;

            SelectedHdrList = new ObservableCollection<string>();
            SelectedHdrList = GetSelectHrdCheckList();
         
        }

      
        private ObservableCollection<string> AddHdrItem(ObservableCollection<string> orgList, string newItem)
        {
            ObservableCollection<string> tempList = orgList;
            tempList.Add(newItem);
            return tempList;
        }

        private ObservableCollection<string> RemoveHdrItem(ObservableCollection<string> orgList, string removeitem)
        {
            ObservableCollection<string> tempList = orgList;
            tempList.Remove(removeitem);
            return tempList;
        }



        private ObservableCollection<string> GetSelectHrdCheckList()
        {
            ObservableCollection<string> XmlCheckedList = new ObservableCollection<string>();
            DataTable HdrTable = new ();
            List<string> XmlColumnList = new List<string>();

            try
            {
                HdrTable = new DataTable();
                HdrTable = _sqlhandler.GetSqlScema(_sqlhandler.GetCurrentRtTableName(ClassSqlHandler.BALE_ARCHIVE));

                XmlColumnList = GetXmlcolumnList(MyXml.XMLHdrFilePath);
                AvailableHdrList = new ObservableCollection<CheckedListItem>();


                foreach (DataRow item in HdrTable.Rows)
                {
                    if (AllowField(item[1].ToString()))
                    {
                        if (XmlColumnList.Contains(item[1].ToString()))
                            AvailableHdrList.Add(new CheckedListItem(Convert.ToInt32(item[0]), item[1].ToString(), true, item[2].ToString()));
                        else
                            AvailableHdrList.Add(new CheckedListItem(Convert.ToInt32(item[0]), item[1].ToString(), false, item[2].ToString()));
                    }
                }
                foreach (var item in XmlColumnList)
                {
                    XmlCheckedList.Add(item);
                }
                AvailableHdrList = new ObservableCollection<CheckedListItem>(AvailableHdrList.OrderBy(x => x.Name)); //Sort

               
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in GetSelectHrdCheckList {ex.Message}");
                ClsSerilog.LogMessage(ClsSerilog.ERROR, $"ERROR in GetSelectHdrCheckList < {ex.Message}");
            }
            return XmlCheckedList;
        }

        private List<string> GetXmlcolumnList(string xMLDropsGdvFile)
        {
            return MyXml.ReadXmlGridView(xMLDropsGdvFile);
        }

        private bool AllowField(string strItem)
        {
            foreach (var item in _sqlhandler.RemoveFieldsList)
            {
                if (item == strItem) return false;
            }
            return true;
        }
    }
}
    