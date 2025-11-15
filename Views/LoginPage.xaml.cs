using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using cowl.Services;

namespace cowl.Views;

public sealed partial class LoginPage : Page
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, RoutedEventArgs e)
    {
        await LoginAsync();
    }

    private void OnTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            PasswordBox.Focus(FocusState.Programmatic);
            e.Handled = true;
        }
    }

    private async void OnPasswordBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            await LoginAsync();
            e.Handled = true;
        }
    }

    private async Task LoginAsync()
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
        {
            ShowMessage("Vui lòng nhập tên đăng nhập hoặc email!", InfoBarSeverity.Warning);
            UsernameTextBox.Focus(FocusState.Programmatic);
            return;
        }

        if (string.IsNullOrWhiteSpace(PasswordBox.Password))
        {
            ShowMessage("Vui lòng nhập mật khẩu!", InfoBarSeverity.Warning);
            PasswordBox.Focus(FocusState.Programmatic);
            return;
        }

        // Show loading
        SetLoadingState(true);

        try
        {
            var authService = AuthService.Instance;
            var (success, message, user) = await authService.LoginAsync(
                UsernameTextBox.Text.Trim(),
                PasswordBox.Password
            );

            if (success && user != null)
            {
                ShowMessage(message, InfoBarSeverity.Success);
                
                // Navigate to main page after a short delay
                await Task.Delay(500);
                
                // Navigate to MainPage
                this.Frame.Navigate(typeof(MainPage));
            }
            else
            {
                ShowMessage(message, InfoBarSeverity.Error);
                PasswordBox.Password = string.Empty;
                PasswordBox.Focus(FocusState.Programmatic);
            }
        }
        catch (Exception ex)
        {
            ShowMessage($"Lỗi kết nối: {ex.Message}", InfoBarSeverity.Error);
        }
        finally
        {
            SetLoadingState(false);
        }
    }

    private void OnRegisterClicked(object sender, RoutedEventArgs e)
    {
        // Navigate to Register page
        this.Frame.Navigate(typeof(RegisterPage));
    }

    private void ShowMessage(string message, InfoBarSeverity severity)
    {
        LoginInfoBar.Message = message;
        LoginInfoBar.Severity = severity;
        LoginInfoBar.IsOpen = true;
    }

    private void SetLoadingState(bool isLoading)
    {
        LoginButton.IsEnabled = !isLoading;
        UsernameTextBox.IsEnabled = !isLoading;
        PasswordBox.IsEnabled = !isLoading;
        LoadingRing.IsActive = isLoading;
        LoadingRing.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
    }
}
