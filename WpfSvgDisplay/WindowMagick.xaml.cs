using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Linq;
using ImageMagick;

namespace WpfSvgDisplay
{
    /// <summary>
    /// Interaction logic for WindowMagick.xaml
    /// </summary>
    public partial class WindowMagick : Window, IDisplaySvg
    {
        private readonly SvgFileHandler _fileHandler;
        private readonly Window[] _windows;
        private readonly WindowSetAll _windowSetAll;
        private readonly WindowManager _windowManager;

        public WindowMagick()
        {
            InitializeComponent();
            _fileHandler = new SvgFileHandler(DisplaySvg, () => DropHint.Visibility = Visibility.Collapsed);

            // Create and store all windows in array
            _windows = new Window[]
            {
                this,
                new WindowSharpVectors(),
                new WindowSvgSkia(),
            };

            // Create WindowSetAll using the windows array
            _windowSetAll = new WindowSetAll(_windows);

            // Initialize window manager with all windows including WindowSetAll
            _windowManager = new WindowManager(_windows.Concat(new[] { _windowSetAll }).ToArray());
            _windowManager.InitializeWindows();
        }

        public void DisplaySvg(string svgPath)
        {
            using (var image = new MagickImage(svgPath))
            {
                // Convert to PNG for display
                var memoryStream = new MemoryStream();
                image.Write(memoryStream, MagickFormat.Png);
                memoryStream.Position = 0;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = memoryStream;
                bitmap.EndInit();
                bitmap.Freeze();

                SvgImage.Source = bitmap;
                DropHint.Visibility = Visibility.Collapsed;
            }
        }

        #region File handling
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _fileHandler.HandleMouseDown(sender, e);
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            _fileHandler.HandleDragOver(sender, e);
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            _fileHandler.HandleDrop(sender, e);
        }
        #endregion
    }
}
