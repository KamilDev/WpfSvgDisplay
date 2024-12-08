using System;
using System.Windows;
using System.Windows.Input;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace WpfSvgDisplay
{
    public partial class WindowSharpVectors : Window, IDisplaySvg
    {
        private readonly SvgFileHandler _fileHandler;
        private readonly WpfDrawingSettings _settings;

        public WindowSharpVectors()
        {
            InitializeComponent();

            // Configure SVG rendering settings
            _settings = new WpfDrawingSettings
            {
                IncludeRuntime = true,
                TextAsGeometry = false // Better for text rendering
            };

            _fileHandler = new SvgFileHandler(DisplaySvg, () => DropHint.Visibility = Visibility.Collapsed);
        }

        public void DisplaySvg(string svgPath)
        {
            DropHint.Visibility = Visibility.Collapsed;
            try
            {
                // Convert the file path to a URI
                var uri = new Uri(svgPath, UriKind.Absolute);

                // Set the URI directly to the SvgViewbox
                SvgViewbox.Source = uri;
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
