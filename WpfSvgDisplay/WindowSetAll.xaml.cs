using System;
using System.Windows;
using System.Windows.Input;

namespace WpfSvgDisplay
{
    public partial class WindowSetAll : Window
    {
        private readonly SvgFileHandler _fileHandler;
        private readonly MainWindow _mainWindow;
        private readonly WindowSharpVectors _window2;
        private readonly WindowSvgSkia _window3;

        public WindowSetAll(MainWindow mainWindow, WindowSharpVectors window2, WindowSvgSkia window3)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _window2 = window2;
            _window3 = window3;

            // Initialize file handler with our custom display method
            _fileHandler = new SvgFileHandler(DisplaySvgInAllWindows, () => { });
        }

        private void DisplaySvgInAllWindows(string svgPath)
        {
            try
            {
                // Display in all windows
                _mainWindow.DisplaySvg(svgPath);
                _window2.DisplaySvg(svgPath);
                _window3.DisplaySvg(svgPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading SVG: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
