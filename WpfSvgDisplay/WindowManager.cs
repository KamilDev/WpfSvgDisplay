using System;
using System.Windows;
using System.Linq;

namespace WpfSvgDisplay
{
    public class WindowManager
    {
        private readonly Window[] _windows;

        public WindowManager(Window[] windows)
        {
            _windows = windows;
        }

        public void InitializeWindows()
        {
            // Position windows before showing them
            PositionWindows();

            // Show all windows except the main window (which is already shown)
            foreach (var window in _windows.Skip(1))
            {
                window.Show();
            }

            // Set up window closing handlers
            SetupWindowClosingHandlers();
        }

        private void SetupWindowClosingHandlers()
        {
            var mainWindow = _windows[0];

            // When main window closes, close all other windows
            mainWindow.Closed += (s, e) =>
            {
                foreach (var window in _windows.Skip(1))
                {
                    window.Close();
                }
            };

            // When any other window closes, close the main window
            foreach (var window in _windows.Skip(1))
            {
                window.Closed += (s, e) => mainWindow.Close();
            }
        }

        private void PositionWindows()
        {
            const double spacing = 10; // pixels between windows
            double screenWidth = SystemParameters.WorkArea.Width;
            double screenHeight = SystemParameters.WorkArea.Height;

            // Calculate total width needed for all windows
            double totalWidth = _windows.Sum(w => w.Width) + (spacing * (_windows.Length + 1));

            // If total width fits on screen, position in one row
            if (totalWidth <= screenWidth)
            {
                PositionWindowsInOneRow(screenWidth, screenHeight, spacing);
            }
            else
            {
                PositionWindowsInTwoRows(screenWidth, screenHeight, spacing);
            }
        }

        private void PositionWindowsInOneRow(double screenWidth, double screenHeight, double spacing)
        {
            double totalWidth = _windows.Sum(w => w.Width) + (spacing * (_windows.Length + 1));
            double startX = (screenWidth - totalWidth) / 2;
            double centerY = (screenHeight - _windows[0].Height) / 2;

            // Position all windows in a row
            double currentX = startX;
            foreach (var window in _windows)
            {
                window.Left = currentX;
                window.Top = centerY;
                currentX += window.Width + spacing;
            }
        }

        private void PositionWindowsInTwoRows(double screenWidth, double screenHeight, double spacing)
        {
            var firstRow = _windows.Take(_windows.Length / 2).ToArray();
            var secondRow = _windows.Skip(_windows.Length / 2).ToArray();

            double rowWidth = Math.Max(
                firstRow.Sum(w => w.Width) + (spacing * (firstRow.Length - 1)),
                secondRow.Sum(w => w.Width) + (spacing * (secondRow.Length - 1))
            );
            double startX = (screenWidth - rowWidth) / 2;
            double startY = (screenHeight - (_windows[0].Height * 2 + spacing)) / 2;

            // Position first row
            double currentX = startX;
            foreach (var window in firstRow)
            {
                window.Left = currentX;
                window.Top = startY;
                currentX += window.Width + spacing;
            }

            // Position second row
            currentX = startX;
            foreach (var window in secondRow)
            {
                window.Left = currentX;
                window.Top = startY + _windows[0].Height + spacing;
                currentX += window.Width + spacing;
            }
        }
    }
}