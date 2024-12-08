using System;
using System.Windows;
using System.Windows.Input;

namespace WpfSvgDisplay
{
    public partial class WindowSetAll : Window
    {
        private readonly SvgFileHandler _fileHandler;
        private readonly Window[] _windows;

        public WindowSetAll(Window[] windows)
        {
            InitializeComponent();
            _windows = windows;

            // Initialize file handler with our custom display method
            _fileHandler = new SvgFileHandler(DisplaySvgInAllWindows, () => { });
        }

        private void DisplaySvgInAllWindows(string svgPath)
        {
            try
            {
                // Display in all windows that implement DisplaySvg
                foreach (var window in _windows)
                {
                    if (window is IDisplaySvg displayWindow)
                    {
                        displayWindow.DisplaySvg(svgPath);
                    }
                }
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
