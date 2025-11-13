using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace cowl
{
    /// <summary>
    /// Converter để hiển thị/ẩn dựa trên chuỗi rỗng
    /// </summary>
    public class EmptyStringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.IsNullOrWhiteSpace(value as string) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
