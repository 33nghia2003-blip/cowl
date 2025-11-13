using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace cowl.Views
{
    /// <summary>
    /// Main page với NavigationView để điều hướng giữa các trang
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            
            // Chọn trang mặc định là InputPage
            NavView.SelectedItem = InputNavItem;
            ContentFrame.Navigate(typeof(InputPage));
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                var tag = args.SelectedItemContainer.Tag?.ToString();
                
                switch (tag)
                {
                    case "Input":
                        ContentFrame.Navigate(typeof(InputPage));
                        break;
                    case "Display":
                        ContentFrame.Navigate(typeof(DisplayPage));
                        break;
                    case "Appointment":
                        ContentFrame.Navigate(typeof(AppointmentPage));
                        break;
                    case "Considering":
                        ContentFrame.Navigate(typeof(ConsideringPage));
                        break;
                    case "NoNeed":
                        ContentFrame.Navigate(typeof(NoNeedPage));
                        break;
                }
            }
        }

        public void ToggleNavigationPane()
        {
            NavView.IsPaneOpen = !NavView.IsPaneOpen;
        }
    }
}
