using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Appearance;

namespace RimWorldLauncher.Controls
{
    public partial class CustomToolbar : UserControl
    {
        public static readonly DependencyProperty LeftContentProperty =
            DependencyProperty.Register("LeftContent", typeof(object), typeof(CustomToolbar), new PropertyMetadata(null));

        public static readonly DependencyProperty CenterContentProperty =
            DependencyProperty.Register("CenterContent", typeof(object), typeof(CustomToolbar), new PropertyMetadata(null));

        public static readonly DependencyProperty RightContentProperty =
            DependencyProperty.Register("RightContent", typeof(object), typeof(CustomToolbar), new PropertyMetadata(null));

        public object LeftContent
        {
            get => GetValue(LeftContentProperty);
            set => SetValue(LeftContentProperty, value);
        }

        public object CenterContent
        {
            get => GetValue(CenterContentProperty);
            set => SetValue(CenterContentProperty, value);
        }

        public object RightContent
        {
            get => GetValue(RightContentProperty);
            set => SetValue(RightContentProperty, value);
        }

        public CustomToolbar()
        {
            InitializeComponent();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null) return;

            if (e.ClickCount == 2)
            {
                // Double-click to maximize/restore (only for resizable windows)
                if (window.ResizeMode != ResizeMode.NoResize)
                {
                    window.WindowState = window.WindowState == WindowState.Maximized 
                        ? WindowState.Normal 
                        : WindowState.Maximized;
                }
            }
            else if (e.ClickCount == 1)
            {
                // Single click to drag the window
                try
                {
                    window.DragMove();
                }
                catch
                {
                    // DragMove can throw an exception if called at the wrong time
                    // Just ignore it
                }
            }
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // This method is no longer needed since we handle double-click in OnMouseLeftButtonDown
            // But keeping it for now in case we need it later
        }
    }
} 