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

namespace ForteVisualData.HelpInfo
{
    /// <summary>
    /// Interaction logic for SetUpWindow.xaml
    /// </summary>
    public partial class SetUpWindow : Window
    {
      //  Image HostImage = new Image();


        public SetUpWindow()
        {
            InitializeComponent();

            BitmapImage myBitmapImage = new BitmapImage();

            /*
            myBitmapImage.BeginInit();

            myBitmapImage.UriSource = new Uri(@"../Images/Host.PNG");

            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            HostImage.Source = myBitmapImage;
            */
        }
    }
}
