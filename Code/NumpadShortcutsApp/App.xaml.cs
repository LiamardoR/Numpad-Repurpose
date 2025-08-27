using System;
using System.Windows;
using System.Windows.Forms;   // For NotifyIcon
using System.Drawing;        // For Icon

namespace NumpadShortcutsApp
{
    public partial class App : System.Windows.Application

    {
        private NotifyIcon _notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create NotifyIcon
            _notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("Assets\\icon.ico"),

                Visible = true,
                Text = "Numpad Shortcuts"
            };

            // Create context menu
            var menu = new ContextMenu();
            menu.MenuItems.Add("Open", (s, args) => ShowMainWindow());
            menu.MenuItems.Add("Exit", (s, args) => ExitApp());

            _notifyIcon.ContextMenu = menu;

            // Hide main window at startup
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Hide();
        }

        private void ShowMainWindow()
        {
            if (Current.MainWindow != null)
            {
                Current.MainWindow.Show();
                Current.MainWindow.WindowState = WindowState.Normal;
                Current.MainWindow.Activate();
            }
        }

        private void ExitApp()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
            Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
            base.OnExit(e);
        }
    }
}
