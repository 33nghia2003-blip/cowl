using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.RegularExpressions;

namespace cowl.Views
{
    /// <summary>
    /// Trang nhập liệu - nơi dán nội dung công ty
    /// </summary>
    public sealed partial class InputPage : Page
    {
        private Services.CompanyDataService DataService => Services.CompanyDataService.Instance;

        public InputPage()
        {
            this.InitializeComponent();
        }

        private void OnProcessClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                ShowInfoBar("Vui lòng nhập nội dung công ty", InfoBarSeverity.Warning);
                return;
            }

            // Tạo công ty mới
            var newCompany = new Models.CompanyInfo();
            
            // Xử lý text đã dán để trích xuất thông tin
            ProcessCompanyData(InputTextBox.Text, newCompany);
            
            // Thêm vào đầu danh sách (index 0) để hiển thị trên cùng
            DataService.Companies.Insert(0, newCompany);
            
            // Xóa ô input để chuẩn bị cho lần nhập tiếp theo
            InputTextBox.Text = string.Empty;
            
            ShowInfoBar($"Đã thêm công ty thành công! Tổng số: {DataService.Companies.Count} công ty", InfoBarSeverity.Success);
        }

        private void ProcessCompanyData(string input, Models.CompanyInfo company)
        {
            var lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Tìm tên công ty - dòng đầu tiên thường là tên
            if (lines.Length > 0)
            {
                var firstLine = lines[0].Trim();
                // Bỏ các ký tự đặc biệt ở đầu nếu có
                firstLine = System.Text.RegularExpressions.Regex.Replace(firstLine, @"^[^\w\s-]+", "").Trim();
                company.CompanyName = firstLine;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                var trimmedLine = lines[i].Trim();
                var lowerLine = trimmedLine.ToLower();

                // Tìm người đại diện
                if (lowerLine.Contains("người đại diện"))
                {
                    var name = ExtractValueAfterLabel(trimmedLine);
                    
                    // Nếu có tên ngay sau label, lấy phần trước "Ngoài ra" nếu có
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        // Tìm phần tên chính (viết hoa) trước "Ngoài ra"
                        var match = System.Text.RegularExpressions.Regex.Match(name, 
                            @"^([A-ZÀÁẠẢÃÂẦẤẬẨẪĂẰẮẶẲẴÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮỲÝỴỶỸĐ\s]+)");
                        
                        if (match.Success)
                        {
                            name = match.Groups[1].Value.Trim();
                        }
                    }
                    
                    company.RepresentativeName = name;
                }
                // Tìm số điện thoại
                else if (lowerLine.Contains("điện thoại") || lowerLine.Contains("phone"))
                {
                    company.PhoneNumber = ExtractPhoneNumber(trimmedLine);
                }
                // Tìm địa chỉ - chỉ lấy địa chỉ chính (không lấy địa chỉ thuế)
                else if (lowerLine.Contains("địa chỉ") && !lowerLine.Contains("thuế"))
                {
                    var address = ExtractValueAfterLabel(trimmedLine);
                    company.Address = address;
                }
                // Tìm tình trạng
                else if (lowerLine.Contains("tình trạng") || lowerLine.Contains("status"))
                {
                    company.Status = ExtractValueAfterLabel(trimmedLine);
                }
                // Tìm ngành nghề chính
                else if (lowerLine.Contains("ngành nghề chính") || lowerLine.Contains("business"))
                {
                    var sector = ExtractValueAfterLabel(trimmedLine);
                    // Lấy phần mô tả ngắn gọn trước dấu ngoặc nếu có
                    if (sector.Contains('('))
                    {
                        var mainPart = sector.Split('(')[0].Trim();
                        if (!string.IsNullOrEmpty(mainPart))
                        {
                            sector = mainPart;
                        }
                    }
                    company.BusinessSector = sector;
                }
                // Tìm ngày hoạt động
                else if (lowerLine.Contains("ngày hoạt động") || lowerLine.Contains("active date"))
                {
                    company.ActiveDate = ExtractValueAfterLabel(trimmedLine);
                }
            }
        }

        private string ExtractValueAfterLabel(string line)
        {
            // Tìm dấu : hoặc tab để tách giá trị
            var colonIndex = line.IndexOf(':');
            if (colonIndex > 0 && colonIndex < line.Length - 1)
            {
                return line.Substring(colonIndex + 1).Trim();
            }
            
            var tabIndex = line.IndexOf('\t');
            if (tabIndex > 0 && tabIndex < line.Length - 1)
            {
                return line.Substring(tabIndex + 1).Trim();
            }
            
            return line.Trim();
        }

        private string ExtractValue(string line)
        {
            return ExtractValueAfterLabel(line);
        }

        private string ExtractPhoneNumber(string line)
        {
            // Tìm số điện thoại trong text (10-11 chữ số)
            var phonePattern = @"(\+84|0)[\d\s\-\.]{8,12}";
            var match = Regex.Match(line, phonePattern);
            if (match.Success)
            {
                return match.Value.Trim();
            }
            return ExtractValue(line);
        }

        private void OnClearClicked(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text = string.Empty;
            DataService.Companies.Clear();
            
            ShowInfoBar("Đã xóa tất cả dữ liệu", InfoBarSeverity.Informational);
        }

        private void ShowInfoBar(string message, InfoBarSeverity severity)
        {
            NotificationInfoBar.Message = message;
            NotificationInfoBar.Severity = severity;
            NotificationInfoBar.IsOpen = true;
        }
    }
}
