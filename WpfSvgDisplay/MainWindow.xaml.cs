using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using ImageMagick;

namespace WpfSvgDisplay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SvgFileHandler _fileHandler;
        private readonly WindowSharpVectors _window2;
        private readonly WindowSvgSkia _window3;
        private readonly WindowSetAll _windowSetAll;

        public MainWindow()
        {
            InitializeComponent();
            _fileHandler = new SvgFileHandler(DisplaySvg, () => DropHint.Visibility = Visibility.Collapsed);

            // Create additional windows
            _window2 = new WindowSharpVectors();
            _window3 = new WindowSvgSkia();
            _windowSetAll = new WindowSetAll(this, _window2, _window3);

            // Calculate window positions
            PositionWindows();

            // Show windows after positioning
            _window2.Show();
            _window3.Show();
            _windowSetAll.Show();

            // Handle window closing
            this.Closed += (s, e) =>
            {
                _window2.Close();
                _window3.Close();
                _windowSetAll.Close();
            };
            _window2.Closed += (s, e) => this.Close();
            _window3.Closed += (s, e) => this.Close();
            _windowSetAll.Closed += (s, e) => this.Close();
        }

        private void PositionWindows()
        {
            const double spacing = 10; // pixels between windows
            double screenWidth = SystemParameters.WorkArea.Width;
            double screenHeight = SystemParameters.WorkArea.Height;

            // Calculate total width needed for all windows
            double totalWidth = this.Width + _window2.Width + _window3.Width + _windowSetAll.Width + (spacing * 5);

            // If total width fits on screen, position in one row
            if (totalWidth <= screenWidth)
            {
                double startX = (screenWidth - totalWidth) / 2;
                double centerY = (screenHeight - this.Height) / 2;

                // Position all windows in a row
                this.Left = startX;
                this.Top = centerY;

                _window2.Left = startX + this.Width + spacing;
                _window2.Top = centerY;

                _window3.Left = startX + this.Width + _window2.Width + (spacing * 2);
                _window3.Top = centerY;

                _windowSetAll.Left = startX + this.Width + _window2.Width + _window3.Width + (spacing * 3);
                _windowSetAll.Top = centerY;
            }
            else
            {
                // Position in two rows
                double rowWidth = Math.Max(
                    this.Width + _window2.Width + spacing,
                    _window3.Width + _windowSetAll.Width + spacing
                );
                double startX = (screenWidth - rowWidth) / 2;
                double startY = (screenHeight - (this.Height * 2 + spacing)) / 2;

                // First row
                this.Left = startX;
                this.Top = startY;

                _window2.Left = startX + this.Width + spacing;
                _window2.Top = startY;

                // Second row
                _window3.Left = startX;
                _window3.Top = startY + this.Height + spacing;

                _windowSetAll.Left = startX + _window3.Width + spacing;
                _windowSetAll.Top = startY + this.Height + spacing;
            }
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
