using AppServices;
using Prism.Events;
using System.Printing;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using static AppServices.ClassApplicationService;

namespace ForteVisualData.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow AppWindows;

        protected readonly IEventAggregator _eventAggregator = new EventAggregator();
        private System.Threading.Mutex objMutex = null;

      

        public MainWindow()
        {
            InitializeComponent();
            AppWindows = this;
            AppWindows.Focus();
            AppWindows.Focusable = true;

            _eventAggregator.GetEvent<RestartAppEvents>().Subscribe(UpdateCloseWindow);

            //  _eventAggregator.GetEvent<UpdateAppCloseEvents>().Subscribe(UpdateCloseWindow);

        }

        private void UpdateCloseWindow(bool obj)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (System.Windows.MessageBox.Show("Exit Application?", "Shutdown Application", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                e.Cancel = true;
            else
                e.Cancel = false;      
        }

        private void OnClosed(object sender, System.EventArgs e)
        {
            ClsSerilog.LogMessage(ClsSerilog.INFO, $"Closed - ForteVisualData App.");
            ClsSerilog.CloseLogger();
            AppWindows?.Close();
        }


        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog print = new PrintDialog();
            if (print.ShowDialog() == true)
            {

                PrintCapabilities capabilities = print.PrintQueue.GetPrintCapabilities(print.PrintTicket);

                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / RrintPanel.ActualWidth,
                                        capabilities.PageImageableArea.ExtentHeight / RrintPanel.ActualHeight);

                Transform oldTransform = RrintPanel.LayoutTransform;

                RrintPanel.LayoutTransform = new ScaleTransform(scale, scale);

                Size oldSize = new Size(RrintPanel.ActualWidth, RrintPanel.ActualHeight);
                Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
                RrintPanel.Measure(sz);
                ((UIElement)RrintPanel).Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight),
                    sz));

                print.PrintVisual(RrintPanel, "Print Results");
                RrintPanel.LayoutTransform = oldTransform;
                RrintPanel.Measure(oldSize);

                ((UIElement)RrintPanel).Arrange(new Rect(new Point(0, 0),
                    oldSize));
            }
        }

        private void Caption()
        {
            //Set scrollviewer's Content property as UI element to capture full content
           // UIElement element = scrollViewer.Content as UIElement;
          //  Uri path = new Uri(@"d:\temp\screenshot.png");
          //  CaptureScreen(element, path);
        }


        private void CaptureScreen(UIElement source, Uri destination)
        {

            try
            {
                double Height, renderHeight, Width, renderWidth;

                Height = renderHeight = source.RenderSize.Height;
                Width = renderWidth = source.RenderSize.Width;

                //Specification for target bitmap like width/height pixel etc.
                RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
                //creates Visual Brush of UIElement
                VisualBrush visualBrush = new VisualBrush(source);

                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    //draws image of element
                    drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(0, 0), new Point(Width, Height)));
                }
                //renders image
                renderTarget.Render(drawingVisual);

                //PNG encoder for creating PNG file
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));
                using (FileStream stream = new FileStream(destination.LocalPath, FileMode.Create, FileAccess.Write))
                {
                    encoder.Save(stream);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }




    }
}
