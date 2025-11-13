using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace cowl
{
    /// <summary>
    /// Main Window with Windows 11 style TitleBar and Mica backdrop
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            SetWindowProperties();
        }

        private void SetWindowProperties()
        {
            // Set window title
            this.Title = "Cowl - Quản lý thông tin công ty";
            
            // Enable custom title bar
            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);
            
            // Set preferred title bar height (Tall for Windows 11 style)
            this.AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            
            // Set minimum window size
            var size = new Windows.Graphics.SizeInt32();
            size.Width = 1280;
            size.Height = 960;
            this.AppWindow.Resize(size);
            
            // Navigate to MainPage
            ContentFrame.Navigate(typeof(Views.MainPage));
        }

        private void AppTitleBar_PaneToggleRequested(TitleBar sender, object args)
        {
            // Find MainPage and toggle navigation pane
            if (ContentFrame.Content is Views.MainPage mainPage)
            {
                mainPage.ToggleNavigationPane();
            }
        }
    }
}
