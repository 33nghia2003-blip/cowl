using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using cowl.Services;
using System.Text.RegularExpressions;

namespace cowl.Views;

public sealed partial class RegisterPage : Page
{
    private bool isUsernameValid = false;
    private bool isEmailValid = false;
    private bool isPasswordMatch = false;

    public RegisterPage()
    {
        InitializeComponent();
    }

    private void OnTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            // Move focus to next field
            if (ReferenceEquals(sender, FullNameTextBox))
                UsernameTextBox.Focus(FocusState.Programmatic);
            else if (ReferenceEquals(sender, UsernameTextBox))
                EmailTextBox.Focus(FocusState.Programmatic);
            else if (ReferenceEquals(sender, EmailTextBox))
                PasswordBox.Focus(FocusState.Programmatic);
            
            e.Handled = true;
        }
    }

    private async void OnPasswordBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            if (ReferenceEquals(sender, PasswordBox))
            {
                ConfirmPasswordBox.Focus(FocusState.Programmatic);
            }
            else if (ReferenceEquals(sender, ConfirmPasswordBox) && RegisterButton.IsEnabled)
            {
                await RegisterAsync();
            }
            e.Handled = true;
        }
    }

    private async void OnUsernameTextChanged(object sender, TextChangedEventArgs e)
    {
        var username = UsernameTextBox.Text.Trim();
        
        if (string.IsNullOrWhiteSpace(username))
        {
            UsernameStatusText.Visibility = Visibility.Collapsed;
            isUsernameValid = false;
            UpdateRegisterButtonState();
            return;
        }

        if (username.Length < 3)
        {
            UsernameStatusText.Text = "Tên đăng nhập phải có ít nhất 3 ký tự";
            UsernameStatusText.Foreground = new SolidColorBrush(Colors.Red);
            UsernameStatusText.Visibility = Visibility.Visible;
            isUsernameValid = false;
            UpdateRegisterButtonState();
            return;
        }

        // Check if username is available
        var authService = AuthService.Instance;
        var isAvailable = await authService.IsUsernameAvailableAsync(username);
        
        if (isAvailable)
        {
            UsernameStatusText.Text = "✓ Tên đăng nhập khả dụng";
            UsernameStatusText.Foreground = new SolidColorBrush(Colors.Green);
            isUsernameValid = true;
        }
        else
        {
            UsernameStatusText.Text = "✗ Tên đăng nhập đã tồn tại";
            UsernameStatusText.Foreground = new SolidColorBrush(Colors.Red);
            isUsernameValid = false;
        }
        
        UsernameStatusText.Visibility = Visibility.Visible;
        UpdateRegisterButtonState();
    }

    private async void OnEmailTextChanged(object sender, TextChangedEventArgs e)
    {
        var email = EmailTextBox.Text.Trim();
        
        if (string.IsNullOrWhiteSpace(email))
        {
            EmailStatusText.Visibility = Visibility.Collapsed;
            isEmailValid = false;
            UpdateRegisterButtonState();
            return;
        }

        // Validate email format
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(email))
        {
            EmailStatusText.Text = "Email không hợp lệ";
            EmailStatusText.Foreground = new SolidColorBrush(Colors.Red);
            EmailStatusText.Visibility = Visibility.Visible;
            isEmailValid = false;
            UpdateRegisterButtonState();
            return;
        }

        // Check if email is available
        var authService = AuthService.Instance;
        var isAvailable = await authService.IsEmailAvailableAsync(email);
        
        if (isAvailable)
        {
            EmailStatusText.Text = "✓ Email khả dụng";
            EmailStatusText.Foreground = new SolidColorBrush(Colors.Green);
            isEmailValid = true;
        }
        else
        {
            EmailStatusText.Text = "✗ Email đã được sử dụng";
            EmailStatusText.Foreground = new SolidColorBrush(Colors.Red);
            isEmailValid = false;
        }
        
        EmailStatusText.Visibility = Visibility.Visible;
        UpdateRegisterButtonState();
    }

    private void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        var password = PasswordBox.Password;
        
        if (string.IsNullOrWhiteSpace(password))
        {
            PasswordStrengthPanel.Visibility = Visibility.Collapsed;
            CheckPasswordMatch();
            UpdateRegisterButtonState();
            return;
        }

        PasswordStrengthPanel.Visibility = Visibility.Visible;

        // Calculate password strength
        int strength = 0;
        if (password.Length >= 6) strength += 20;
        if (password.Length >= 8) strength += 20;
        if (Regex.IsMatch(password, @"[a-z]")) strength += 20;
        if (Regex.IsMatch(password, @"[A-Z]")) strength += 20;
        if (Regex.IsMatch(password, @"[0-9]")) strength += 10;
        if (Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>]")) strength += 10;

        PasswordStrengthBar.Value = strength;

        if (strength < 40)
        {
            PasswordStrengthText.Text = "Độ mạnh: Yếu";
            PasswordStrengthBar.Foreground = new SolidColorBrush(Colors.Red);
        }
        else if (strength < 70)
        {
            PasswordStrengthText.Text = "Độ mạnh: Trung bình";
            PasswordStrengthBar.Foreground = new SolidColorBrush(Colors.Orange);
        }
        else
        {
            PasswordStrengthText.Text = "Độ mạnh: Mạnh";
            PasswordStrengthBar.Foreground = new SolidColorBrush(Colors.Green);
        }

        CheckPasswordMatch();
        UpdateRegisterButtonState();
    }

    private void OnConfirmPasswordChanged(object sender, RoutedEventArgs e)
    {
        CheckPasswordMatch();
        UpdateRegisterButtonState();
    }

    private void CheckPasswordMatch()
    {
        if (string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password))
        {
            PasswordMatchText.Visibility = Visibility.Collapsed;
            isPasswordMatch = false;
            return;
        }

        if (PasswordBox.Password == ConfirmPasswordBox.Password)
        {
            PasswordMatchText.Text = "✓ Mật khẩu khớp";
            PasswordMatchText.Foreground = new SolidColorBrush(Colors.Green);
            isPasswordMatch = true;
        }
        else
        {
            PasswordMatchText.Text = "✗ Mật khẩu không khớp";
            PasswordMatchText.Foreground = new SolidColorBrush(Colors.Red);
            isPasswordMatch = false;
        }
        
        PasswordMatchText.Visibility = Visibility.Visible;
    }

    private void UpdateRegisterButtonState()
    {
        RegisterButton.IsEnabled = 
            !string.IsNullOrWhiteSpace(FullNameTextBox.Text) &&
            isUsernameValid &&
            isEmailValid &&
            !string.IsNullOrWhiteSpace(PasswordBox.Password) &&
            PasswordBox.Password.Length >= 6 &&
            isPasswordMatch &&
            AgreeTermsCheckBox.IsChecked == true;
    }

    private void OnAgreeTermsChanged(object sender, RoutedEventArgs e)
    {
        UpdateRegisterButtonState();
    }

    private async void OnRegisterClicked(object sender, RoutedEventArgs e)
    {
        await RegisterAsync();
    }

    private async Task RegisterAsync()
    {
        if (!RegisterButton.IsEnabled)
            return;

        // Validate all fields
        if (!AgreeTermsCheckBox.IsChecked == true)
        {
            ShowMessage("Vui lòng đồng ý với điều khoản dịch vụ!", InfoBarSeverity.Warning);
            return;
        }

        // Show loading
        SetLoadingState(true);

        try
        {
            var authService = AuthService.Instance;
            var (success, message) = await authService.RegisterAsync(
                UsernameTextBox.Text.Trim(),
                EmailTextBox.Text.Trim(),
                PasswordBox.Password,
                FullNameTextBox.Text.Trim()
            );

            if (success)
            {
                ShowMessage(message, InfoBarSeverity.Success);
                
                // Clear form
                await Task.Delay(1500);
                
                // Navigate back to login
                this.Frame.Navigate(typeof(LoginPage));
            }
            else
            {
                ShowMessage(message, InfoBarSeverity.Error);
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

    private void OnLoginClicked(object sender, RoutedEventArgs e)
    {
        // Navigate to Login page
        this.Frame.Navigate(typeof(LoginPage));
    }

    private void ShowMessage(string message, InfoBarSeverity severity)
    {
        RegisterInfoBar.Message = message;
        RegisterInfoBar.Severity = severity;
        RegisterInfoBar.IsOpen = true;
    }

    private void SetLoadingState(bool isLoading)
    {
        RegisterButton.IsEnabled = !isLoading;
        FullNameTextBox.IsEnabled = !isLoading;
        UsernameTextBox.IsEnabled = !isLoading;
        EmailTextBox.IsEnabled = !isLoading;
        PasswordBox.IsEnabled = !isLoading;
        ConfirmPasswordBox.IsEnabled = !isLoading;
        AgreeTermsCheckBox.IsEnabled = !isLoading;
        LoadingRing.IsActive = isLoading;
        LoadingRing.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
    }
}
