using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using RimWorldLauncher.ViewModels;

namespace RimWorldLauncher.Windows
{
    public partial class LogsWindow : Window
    {
        private bool _isResizing = false;
        private ResizeDirection _resizeDirection = ResizeDirection.None;
        private Point _startPoint;
        private double _startWidth;
        private double _startHeight;
        private double _startLeft;
        private double _startTop;

        private const double MIN_WIDTH = 400;
        private const double MIN_HEIGHT = 300;

        public LogsWindow(LogsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void ResizeBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle resizeBorder)
            {
                _isResizing = true;
                _startPoint = e.GetPosition(this);
                _startWidth = Width;
                _startHeight = Height;
                _startLeft = Left;
                _startTop = Top;

                // Determine resize direction based on which border was clicked
                if (resizeBorder == TopLeftResize)
                    _resizeDirection = ResizeDirection.TopLeft;
                else if (resizeBorder == TopResize)
                    _resizeDirection = ResizeDirection.Top;
                else if (resizeBorder == TopRightResize)
                    _resizeDirection = ResizeDirection.TopRight;
                else if (resizeBorder == RightResize)
                    _resizeDirection = ResizeDirection.Right;
                else if (resizeBorder == BottomRightResize)
                    _resizeDirection = ResizeDirection.BottomRight;
                else if (resizeBorder == BottomResize)
                    _resizeDirection = ResizeDirection.Bottom;
                else if (resizeBorder == BottomLeftResize)
                    _resizeDirection = ResizeDirection.BottomLeft;
                else if (resizeBorder == LeftResize)
                    _resizeDirection = ResizeDirection.Left;

                resizeBorder.CaptureMouse();
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isResizing)
            {
                Point currentPoint = e.GetPosition(this);
                double deltaX = currentPoint.X - _startPoint.X;
                double deltaY = currentPoint.Y - _startPoint.Y;

                double newWidth = _startWidth;
                double newHeight = _startHeight;
                double newLeft = _startLeft;
                double newTop = _startTop;

                switch (_resizeDirection)
                {
                    case ResizeDirection.Left:
                        newWidth = Math.Max(MIN_WIDTH, _startWidth - deltaX);
                        newLeft = _startLeft + (_startWidth - newWidth);
                        break;
                    case ResizeDirection.TopLeft:
                        newWidth = Math.Max(MIN_WIDTH, _startWidth - deltaX);
                        newHeight = Math.Max(MIN_HEIGHT, _startHeight - deltaY);
                        newLeft = _startLeft + (_startWidth - newWidth);
                        newTop = _startTop + (_startHeight - newHeight);
                        break;
                    case ResizeDirection.Top:
                        newHeight = Math.Max(MIN_HEIGHT, _startHeight - deltaY);
                        newTop = _startTop + (_startHeight - newHeight);
                        break;
                    case ResizeDirection.TopRight:
                        newWidth = Math.Max(MIN_WIDTH, _startWidth + deltaX);
                        newHeight = Math.Max(MIN_HEIGHT, _startHeight - deltaY);
                        newTop = _startTop + (_startHeight - newHeight);
                        break;
                    case ResizeDirection.Right:
                        newWidth = Math.Max(MIN_WIDTH, _startWidth + deltaX);
                        break;
                    case ResizeDirection.BottomRight:
                        newWidth = Math.Max(MIN_WIDTH, _startWidth + deltaX);
                        newHeight = Math.Max(MIN_HEIGHT, _startHeight + deltaY);
                        break;
                    case ResizeDirection.Bottom:
                        newHeight = Math.Max(MIN_HEIGHT, _startHeight + deltaY);
                        break;
                    case ResizeDirection.BottomLeft:
                        newWidth = Math.Max(MIN_WIDTH, _startWidth - deltaX);
                        newHeight = Math.Max(MIN_HEIGHT, _startHeight + deltaY);
                        newLeft = _startLeft + (_startWidth - newWidth);
                        break;
                }

                Width = newWidth;
                Height = newHeight;
                Left = newLeft;
                Top = newTop;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_isResizing)
            {
                _isResizing = false;
                _resizeDirection = ResizeDirection.None;
                
                // Release mouse capture from all resize borders
                TopLeftResize.ReleaseMouseCapture();
                TopResize.ReleaseMouseCapture();
                TopRightResize.ReleaseMouseCapture();
                RightResize.ReleaseMouseCapture();
                BottomRightResize.ReleaseMouseCapture();
                BottomResize.ReleaseMouseCapture();
                BottomLeftResize.ReleaseMouseCapture();
                LeftResize.ReleaseMouseCapture();
            }

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            if (_isResizing)
            {
                _isResizing = false;
                _resizeDirection = ResizeDirection.None;
            }

            base.OnLostMouseCapture(e);
        }
    }

    public enum ResizeDirection
    {
        None,
        Left,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft
    }
} 