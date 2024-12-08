using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace WpfSvgDisplay
{
    public class SvgFileHandler
    {
        private readonly Action<string> _onSvgFileSelected;
        private readonly Action _onFileAccepted;

        public SvgFileHandler(Action<string> onSvgFileSelected, Action onFileAccepted)
        {
            _onSvgFileSelected = onSvgFileSelected;
            _onFileAccepted = onFileAccepted;
        }

        public void HandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ShowFileDialog();
            }
        }

        public void HandleDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && IsSvgFile(files[0]))
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }
            e.Handled = true;
        }

        public void HandleDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && IsSvgFile(files[0]))
                {
                    try
                    {
                        _onSvgFileSelected(files[0]);
                        _onFileAccepted();
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex);
                    }
                }
            }
        }

        private void ShowFileDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "SVG files (*.svg)|*.svg|All files (*.*)|*.*",
                Title = "Select an SVG file"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _onSvgFileSelected(openFileDialog.FileName);
                    _onFileAccepted();
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                }
            }
        }

        private static bool IsSvgFile(string filePath)
        {
            return Path.GetExtension(filePath).ToLower() == ".svg";
        }

        private static void ShowError(Exception ex)
        {
            MessageBox.Show($"Error loading SVG: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}