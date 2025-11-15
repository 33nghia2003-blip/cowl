using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using cowl.Services;

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
            
            // Load user info
            LoadUserInfo();
            
            // Chọn trang mặc định là InputPage
            NavView.SelectedItem = InputNavItem;
            ContentFrame.Navigate(typeof(InputPage));
        }

        private async void LoadUserInfo()
        {
            var authService = AuthService.Instance;
            if (authService.CurrentUser != null)
            {
                UserNameText.Text = authService.CurrentUser.FullName;
                UserEmailText.Text = authService.CurrentUser.Email;
                
                // Load dữ liệu công ty của user
                await CompanyDataService.Instance.LoadCompaniesForCurrentUserAsync();
            }
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

        private void OnLogoutClicked(object sender, RoutedEventArgs e)
        {
            // Logout user
            var authService = AuthService.Instance;
            authService.Logout();

            // Navigate to Login page - use the Page's Frame
            this.Frame.Navigate(typeof(LoginPage));
        }
    }
}
