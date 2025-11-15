using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using cowl.Models;

namespace cowl.Views
{
    public sealed partial class TimelinePage : Page
    {
        private Services.CompanyDataService DataService => Services.CompanyDataService.Instance;

        public TimelinePage()
        {
            this.InitializeComponent();
            this.Loaded += TimelinePage_Loaded;
        }

        private void TimelinePage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllAppointments();
        }

        private void LoadAllAppointments()
        {
            TimelinePanel.Children.Clear();

            // Lấy tất cả công ty có lịch hẹn
            var companiesWithAppointments = DataService.Companies
                .Where(c => c.AppointmentDate.HasValue)
                .OrderBy(c => c.AppointmentDate)
                .ToList();

            if (companiesWithAppointments.Count == 0)
            {
                var noDataStack = new StackPanel
                {
                    Spacing = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 60, 0, 0)
                };

                var icon = new FontIcon
                {
                    Glyph = "\uE787",
                    FontSize = 48,
                    Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"],
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var noDataText = new TextBlock
                {
                    Text = "Chưa có lịch hẹn nào",
                    FontSize = 18,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"],
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var hintText = new TextBlock
                {
                    Text = "Thêm lịch hẹn cho công ty ở trang Thông tin công ty",
                    FontSize = 14,
                    Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorTertiaryBrush"],
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                noDataStack.Children.Add(icon);
                noDataStack.Children.Add(noDataText);
                noDataStack.Children.Add(hintText);
                TimelinePanel.Children.Add(noDataStack);
                return;
            }

            // Tổng số lịch hẹn
            var summaryCard = new Border
            {
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(0x33, 0x10, 0xB9, 0x81)),
                BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x10, 0xB9, 0x81)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 24)
            };

            var summaryGrid = new Grid();
            summaryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            summaryGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var summaryIcon = new FontIcon
            {
                Glyph = "\uE787",
                FontSize = 32,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x10, 0xB9, 0x81)),
                Margin = new Thickness(0, 0, 16, 0)
            };
            Grid.SetColumn(summaryIcon, 0);

            var summaryStack = new StackPanel { Spacing = 4 };
            var summaryTitle = new TextBlock
            {
                Text = "TỔNG SỐ LỊCH HẸN",
                FontSize = 11,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x10, 0xB9, 0x81))
            };
            var summaryCount = new TextBlock
            {
                Text = $"{companiesWithAppointments.Count} cuộc hẹn",
                FontSize = 24,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x10, 0xB9, 0x81))
            };
            summaryStack.Children.Add(summaryTitle);
            summaryStack.Children.Add(summaryCount);
            Grid.SetColumn(summaryStack, 1);

            summaryGrid.Children.Add(summaryIcon);
            summaryGrid.Children.Add(summaryStack);
            summaryCard.Child = summaryGrid;
            TimelinePanel.Children.Add(summaryCard);

            // Nhóm theo ngày
            var groupedByDate = companiesWithAppointments
                .GroupBy(c => c.AppointmentDate!.Value.Date)
                .OrderBy(g => g.Key);

            foreach (var dateGroup in groupedByDate)
            {
                // Header ngày
                var dateHeader = new Border
                {
                    Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"],
                    BorderBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"],
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(20, 16, 20, 16),
                    Margin = new Thickness(0, 0, 0, 12)
                };

                var dateHeaderGrid = new Grid();
                dateHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                dateHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                dateHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var calendarIcon = new FontIcon
                {
                    Glyph = "\uE787",
                    FontSize = 20,
                    Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["AccentFillColorDefaultBrush"],
                    Margin = new Thickness(0, 0, 12, 0)
                };
                Grid.SetColumn(calendarIcon, 0);

                var dateTextStack = new StackPanel();
                var dateText = new TextBlock
                {
                    Text = dateGroup.Key.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("vi-VN")),
                    FontSize = 18,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
                };
                var relativeDateText = new TextBlock
                {
                    Text = GetRelativeDateText(dateGroup.Key),
                    FontSize = 12,
                    Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
                };
                dateTextStack.Children.Add(dateText);
                dateTextStack.Children.Add(relativeDateText);
                Grid.SetColumn(dateTextStack, 1);

                var countBadge = new Border
                {
                    Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["AccentFillColorDefaultBrush"],
                    CornerRadius = new CornerRadius(12),
                    Padding = new Thickness(12, 4, 12, 4)
                };
                var countText = new TextBlock
                {
                    Text = $"{dateGroup.Count()}",
                    FontSize = 14,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
                };
                countBadge.Child = countText;
                Grid.SetColumn(countBadge, 2);

                dateHeaderGrid.Children.Add(calendarIcon);
                dateHeaderGrid.Children.Add(dateTextStack);
                dateHeaderGrid.Children.Add(countBadge);
                dateHeader.Child = dateHeaderGrid;
                TimelinePanel.Children.Add(dateHeader);

                // Các công ty trong ngày, sắp xếp theo giờ
                var companiesInDay = dateGroup.OrderBy(c => c.AppointmentDate!.Value.TimeOfDay).ToList();
                foreach (var company in companiesInDay)
                {
                    var card = CreateCompanyCard(company);
                    TimelinePanel.Children.Add(card);
                }
            }
        }

        private string GetRelativeDateText(DateTime date)
        {
            var today = DateTime.Today;
            var diff = (date - today).Days;

            if (diff == 0) return "Hôm nay";
            if (diff == 1) return "Ngày mai";
            if (diff == -1) return "Hôm qua";
            if (diff > 0 && diff <= 7) return $"Còn {diff} ngày nữa";
            if (diff < 0 && diff >= -7) return $"{-diff} ngày trước";
            if (diff < 0) return "Đã qua";
            return $"Còn {diff} ngày";
        }

        private Border CreateCompanyCard(CompanyInfo company)
        {
            var card = new Border
            {
                Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"],
                BorderBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"],
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 12)
            };

            var mainStack = new StackPanel { Spacing = 12 };

            // Company name, status badge, and copy button
            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var companyName = new TextBlock
            {
                Text = company.CompanyName,
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            };
            Grid.SetColumn(companyName, 0);

            var statusBadge = new Border
            {
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(8, 0, 0, 0)
            };
            Grid.SetColumn(statusBadge, 1);

            var statusText = new TextBlock { FontSize = 12, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold };

            if (company.HasAppointment)
            {
                statusBadge.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(0x33, 0x10, 0xB9, 0x81));
                statusText.Text = "CÓ HẸN";
                statusText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x10, 0xB9, 0x81));
            }
            else if (company.IsConsidering)
            {
                statusBadge.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(0x33, 0xF5, 0x9E, 0x0B));
                statusText.Text = "ĐANG PHÂN VÂN";
                statusText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(0xFF, 0xF5, 0x9E, 0x0B));
            }
            else if (company.NoNeed)
            {
                statusBadge.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(0x33, 0xEF, 0x44, 0x44));
                statusText.Text = "KHÔNG CÓ NHU CẦU";
                statusText.Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(0xFF, 0xEF, 0x44, 0x44));
            }

            statusBadge.Child = statusText;
            headerGrid.Children.Add(companyName);
            headerGrid.Children.Add(statusBadge);

            // Copy button
            var copyButton = new Button
            {
                Content = new SymbolIcon(Symbol.Copy),
                Margin = new Thickness(8, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            copyButton.Click += async (s, e) => await CopyCompanyInfo(company);
            Grid.SetColumn(copyButton, 2);
            headerGrid.Children.Add(copyButton);

            mainStack.Children.Add(headerGrid);

            // Representative
            var repStack = new StackPanel { Spacing = 4 };
            var repLabel = new TextBlock
            {
                Text = "Người đại diện",
                FontSize = 11,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            };
            var repValue = new TextBlock
            {
                Text = company.RepresentativeName,
                FontSize = 14
            };
            repStack.Children.Add(repLabel);
            repStack.Children.Add(repValue);
            mainStack.Children.Add(repStack);

            // Phone
            var phoneStack = new StackPanel { Spacing = 4 };
            var phoneLabel = new TextBlock
            {
                Text = "Số điện thoại",
                FontSize = 11,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            };
            var phoneValue = new TextBlock
            {
                Text = company.PhoneNumber,
                FontSize = 14
            };
            phoneStack.Children.Add(phoneLabel);
            phoneStack.Children.Add(phoneValue);
            mainStack.Children.Add(phoneStack);

            // Address
            if (!string.IsNullOrWhiteSpace(company.Address))
            {
                var addressStack = new StackPanel { Spacing = 4 };
                var addressLabel = new TextBlock
                {
                    Text = "Địa chỉ",
                    FontSize = 11,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
                };
                var addressValue = new TextBlock
                {
                    Text = company.Address,
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap
                };
                addressStack.Children.Add(addressLabel);
                addressStack.Children.Add(addressValue);
                mainStack.Children.Add(addressStack);
            }

            // Note
            if (!string.IsNullOrWhiteSpace(company.Note))
            {
                var noteStack = new StackPanel { Spacing = 4 };
                var noteLabel = new TextBlock
                {
                    Text = "Ghi chú",
                    FontSize = 11,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
                };
                var noteValue = new TextBlock
                {
                    Text = company.Note,
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap
                };
                noteStack.Children.Add(noteLabel);
                noteStack.Children.Add(noteValue);
                mainStack.Children.Add(noteStack);
            }

            // Appointment time with icon
            if (company.AppointmentDate.HasValue)
            {
                var timeGrid = new Grid();
                timeGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                timeGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var clockIcon = new FontIcon
                {
                    Glyph = "\uE121",
                    FontSize = 16,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x10, 0xB9, 0x81)),
                    Margin = new Thickness(0, 0, 8, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(clockIcon, 0);

                var timeValue = new TextBlock
                {
                    Text = company.AppointmentDate.Value.ToString("HH:mm"),
                    FontSize = 18,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Microsoft.UI.ColorHelper.FromArgb(0xFF, 0x10, 0xB9, 0x81)),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(timeValue, 1);

                timeGrid.Children.Add(clockIcon);
                timeGrid.Children.Add(timeValue);
                mainStack.Children.Add(timeGrid);
            }

            card.Child = mainStack;
            return card;
        }

        private async System.Threading.Tasks.Task CopyCompanyInfo(CompanyInfo company)
        {
            var info = new System.Text.StringBuilder();
            info.AppendLine(company.CompanyName);
            info.AppendLine();
            
            if (!string.IsNullOrWhiteSpace(company.RepresentativeName))
            {
                info.AppendLine($"Người đại diện: {company.RepresentativeName}");
            }
            
            if (!string.IsNullOrWhiteSpace(company.PhoneNumber))
            {
                info.AppendLine($"Số điện thoại: {company.PhoneNumber}");
            }
            
            if (!string.IsNullOrWhiteSpace(company.Address))
            {
                info.AppendLine($"Địa chỉ: {company.Address}");
            }
            
            if (company.AppointmentDate.HasValue)
            {
                info.AppendLine($"Ngày hẹn: {company.AppointmentDate.Value:dd/MM/yyyy}");
                info.AppendLine($"Giờ hẹn: {company.AppointmentDate.Value:HH:mm}");
            }
            
            if (!string.IsNullOrWhiteSpace(company.Note))
            {
                info.AppendLine($"Ghi chú: {company.Note}");
            }

            var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dataPackage.SetText(info.ToString());
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);

            // Show notification
            var notificationGrid = new Grid
            {
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(0xE6, 0x10, 0xB9, 0x81)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16, 12, 16, 12),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var notificationStack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8
            };

            var checkIcon = new FontIcon
            {
                Glyph = "\uE73E",
                FontSize = 16,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
            };

            var notificationText = new TextBlock
            {
                Text = "Đã sao chép thông tin công ty",
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
            };

            notificationStack.Children.Add(checkIcon);
            notificationStack.Children.Add(notificationText);
            notificationGrid.Children.Add(notificationStack);

            // Add to page temporarily
            var rootGrid = this.Content as Grid;
            if (rootGrid != null)
            {
                rootGrid.Children.Add(notificationGrid);
                
                // Remove after 2 seconds
                await System.Threading.Tasks.Task.Delay(2000);
                rootGrid.Children.Remove(notificationGrid);
            }
        }
    }
}
