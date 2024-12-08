using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Svg.Skia;
using SkiaSharp;

namespace WpfSvgDisplay
{
    public partial class WindowSvgSkia : Window
    {
        private readonly SvgFileHandler _fileHandler;
        private readonly SKSvg _skSvg;

        public WindowSvgSkia()
        {
            InitializeComponent();
            _skSvg = new SKSvg();
            _fileHandler = new SvgFileHandler(DisplaySvg, () => DropHint.Visibility = Visibility.Collapsed);
        }

        public void DisplaySvg(string svgPath)
        {
            DropHint.Visibility = Visibility.Collapsed;
            try
            {
                // Load and render the SVG using Skia
                using (var stream = File.OpenRead(svgPath))
                {
                    var picture = _skSvg.Load(stream);
                    if (picture != null)
                    {
                        // Get the dimensions
                        var width = (int)_skSvg.Picture.CullRect.Width;
                        var height = (int)_skSvg.Picture.CullRect.Height;

                        // Create an SKImageInfo with the desired dimensions
                        var imageInfo = new SKImageInfo(width, height);

                        // Create a bitmap surface to draw on
                        using (var surface = SKSurface.Create(imageInfo))
                        {
                            var canvas = surface.Canvas;
                            canvas.Clear(SKColors.Transparent);
                            canvas.DrawPicture(_skSvg.Picture);

                            // Get the image from the surface
                            using (var image = surface.Snapshot())
                            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                            using (var memStream = new MemoryStream())
                            {
                                data.SaveTo(memStream);
                                memStream.Position = 0;

                                var bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                bitmapImage.StreamSource = memStream;
                                bitmapImage.EndInit();
                                bitmapImage.Freeze();

                                SvgImage.Source = bitmapImage;
                            }
                        }
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
