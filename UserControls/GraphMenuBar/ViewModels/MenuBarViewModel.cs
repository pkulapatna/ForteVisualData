using AppServices;
using GraphMenuBar.Properties;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static AppServices.ClassApplicationService;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GraphMenuBar.ViewModels
{
    public class MenuBarViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;

      
        #region Menu options


        private bool _menuEnable = true;
        public bool MenuEnable
        {
            get => _menuEnable;
            set => SetProperty(ref _menuEnable, value);
        }

        private bool _menuOneChecked;
        public bool MenuOneChecked
        {
            get => _menuOneChecked;
            set
            {
                SetProperty(ref _menuOneChecked, value);
                if (value == true)
                {
                    SaveMenuSelected(0);
                }
            }
        }
        private string _menuOneHdr = ClassCommon.MoistureUnitLst[ClassCommon.MoistureType];
        public string MenuOneHdr
        {
            get { return _menuOneHdr; }
            set { SetProperty(ref _menuOneHdr, value); }
        }



        private bool _menuTwoChecked;
        public bool MenuTwoChecked
        {
            get => _menuTwoChecked;
            set
            {
                SetProperty(ref _menuTwoChecked, value);
                if (value == true)
                {
                    SaveMenuSelected(1);
                }
            }
        }
        private string _menuTwoHdr = ClassCommon.WeightTypeLst[ClassCommon.WeightUnit];
        public string MenuTwoHdr
        {
            get { return _menuTwoHdr; }
            set { SetProperty(ref _menuTwoHdr, value); }
        }


        //Menu 3
        private bool _menuThreeChecked;
        public bool MenuThreeChecked
        {
            get => _menuThreeChecked;
            set
            {
                SetProperty(ref _menuThreeChecked, value);
                if (value == true)
                {
                    SaveMenuSelected(2);
                }
            }
        }
        private Visibility _showMenuThree = Visibility.Hidden;
        public Visibility ShowMenuThree
        {
            get => _showMenuThree;
            set => SetProperty(ref _showMenuThree, value);
        }

        private string _menuThreeHdr;
        public string MenuThreeHdr
        {
            get { return _menuThreeHdr; }
            set { SetProperty(ref _menuThreeHdr, value); }
        }

        private bool _menuFourChecked;
        public bool MenuFourChecked
        {
            get => _menuFourChecked;
            set
            {
                SetProperty(ref _menuFourChecked, value);
                if (value == true)
                {
                    SaveMenuSelected(3);
                }
            }
        }
        private Visibility _showMenuFour = Visibility.Hidden;
        public Visibility ShowMenuFour
        {
            get => _showMenuFour;
            set => SetProperty(ref _showMenuFour, value);
        }

        private string _menuFourHdr;
        public string MenuFourHdr
        {
            get { return _menuFourHdr; }
            set { SetProperty(ref _menuFourHdr, value); }
        }


        private bool _menuFiveChecked;
        public bool MenuFiveChecked
        {
            get => _menuFiveChecked;
            set
            {
                SetProperty(ref _menuFiveChecked, value);
                if (value == true)
                {
                    SaveMenuSelected(4);
                }
            }
        }
        private Visibility _showMenuFive = Visibility.Hidden;
        public Visibility ShowMenuFive
        {
            get => _showMenuFive; 
            set => SetProperty(ref _showMenuFive, value); 
        }

        private string _menuFiveHdr;
        public string MenuFiveHdr
        {
            get { return _menuFiveHdr; }
            set { SetProperty(ref _menuFiveHdr, value); }
        }


        #endregion Menu options


        public MenuBarViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<UpdateAppRunEvents>().Subscribe(UpdateAppRunning);
            _eventAggregator.GetEvent<SettingsChangedEvents>().Subscribe(UpdateSettings);

            if (ClassCommon.WeightUnit == 0)
            {
                MenuThreeHdr = "BDWeight(kg.)";
                MenuFourHdr = "ADWeight(kg.)";
            }
            else if (ClassCommon.WeightUnit == 1) 
            {
                MenuThreeHdr = "BDWeight(lb.)";
                MenuFourHdr = "ADWeight(lb.)";
            }
                

            switch (ClassCommon.MenuChecked)
            {
                case ClassCommon.MenuMoisture:
                    MenuOneChecked = true;
                    break;

                case ClassCommon.MenuWeight:
                    MenuTwoChecked = true;  
                    break;

                case ClassCommon.MenuBDWeight:
                    MenuThreeChecked = true;    
                    break;

                case ClassCommon.MenuADWeight:
                    MenuFourChecked = true; 
                    break;
                case 4:
                    MenuFiveChecked = true;
                    break;

            }

        }

        private void UpdateSettings(bool obj)
        {
            MenuOneHdr = ClassCommon.MoistureUnitLst[ClassCommon.MoistureType];
            MenuTwoHdr = ClassCommon.WeightTypeLst[ClassCommon.WeightUnit];

           
        }

        private void UpdateAppRunning(bool obj)
        {
            if (obj == true) MenuEnable = false; else MenuEnable = true;   
          
        }

        private void SaveMenuSelected(int idxSel)
        {
            Settings.Default.MenuSelectIndex = idxSel;
            Settings.Default.Save();
            ClassCommon.MenuChecked = idxSel;
            _eventAggregator.GetEvent<ChangeMenuEvents>().Publish(idxSel);

        }


        public void SetMenuHeader(int  idxHeader) 
        {
        
            switch (idxHeader)
            {
                case 0:
                    break; 
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }
        
        }


    }
}
