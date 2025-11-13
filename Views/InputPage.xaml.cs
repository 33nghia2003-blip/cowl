using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

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

        private async void OnExportToWordClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataService.Companies.Count == 0)
                {
                    ShowInfoBar("Không có dữ liệu để xuất", InfoBarSeverity.Warning);
                    return;
                }

                // Tạo FileSavePicker
                var savePicker = new FileSavePicker();
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("Word Document", new[] { ".docx" });
                savePicker.SuggestedFileName = $"DanhSachCongTy_{DateTime.Now:yyyyMMdd_HHmmss}";

                var file = await savePicker.PickSaveFileAsync();
                if (file == null)
                {
                    return; // User cancelled
                }

                // Tạo Word document
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(file.Path, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Tiêu đề
                    Paragraph titlePara = body.AppendChild(new Paragraph());
                    Run titleRun = titlePara.AppendChild(new Run());
                    titleRun.AppendChild(new Text($"DANH SÁCH CÔNG TY - {DateTime.Now:dd/MM/yyyy}"));
                    RunProperties titleRunProps = titleRun.InsertAt(new RunProperties(), 0);
                    titleRunProps.AppendChild(new Bold());
                    titleRunProps.AppendChild(new FontSize() { Val = "32" });
                    ParagraphProperties titleParaProps = titlePara.InsertAt(new ParagraphProperties(), 0);
                    titleParaProps.AppendChild(new Justification() { Val = JustificationValues.Center });
                    titleParaProps.AppendChild(new SpacingBetweenLines() { After = "400" });

                    // Tổng số công ty
                    Paragraph summaryPara = body.AppendChild(new Paragraph());
                    Run summaryRun = summaryPara.AppendChild(new Run());
                    summaryRun.AppendChild(new Text($"Tổng số: {DataService.Companies.Count} công ty"));
                    ParagraphProperties summaryParaProps = summaryPara.InsertAt(new ParagraphProperties(), 0);
                    summaryParaProps.AppendChild(new SpacingBetweenLines() { After = "400" });

                    // Tạo bảng 2 cột
                    Table table = new Table();

                    // Thuộc tính bảng
                    TableProperties tableProps = new TableProperties(
                        new TableBorders(
                            new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                            new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                            new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                            new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                            new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                            new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
                        ),
                        new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct }
                    );
                    table.AppendChild(tableProps);

                    // Chia công ty thành 2 cột
                    var companies = DataService.Companies.ToList();
                    var itemsPerColumn = (int)Math.Ceiling(companies.Count / 2.0);

                    for (int i = 0; i < itemsPerColumn; i++)
                    {
                        TableRow row = new TableRow();

                        // Cột 1
                        if (i < companies.Count)
                        {
                            TableCell cell1 = CreateCompanyCell(companies[i], i + 1);
                            row.AppendChild(cell1);
                        }
                        else
                        {
                            row.AppendChild(new TableCell(new Paragraph(new Run(new Text("")))));
                        }

                        // Cột 2
                        int secondIndex = i + itemsPerColumn;
                        if (secondIndex < companies.Count)
                        {
                            TableCell cell2 = CreateCompanyCell(companies[secondIndex], secondIndex + 1);
                            row.AppendChild(cell2);
                        }
                        else
                        {
                            row.AppendChild(new TableCell(new Paragraph(new Run(new Text("")))));
                        }

                        table.AppendChild(row);
                    }

                    body.AppendChild(table);
                }

                ShowInfoBar($"Đã xuất {DataService.Companies.Count} công ty ra file Word thành công!", InfoBarSeverity.Success);
            }
            catch (Exception ex)
            {
                ShowInfoBar($"Lỗi khi xuất file: {ex.Message}", InfoBarSeverity.Error);
            }
        }

        private TableCell CreateCompanyCell(Models.CompanyInfo company, int index)
        {
            TableCell cell = new TableCell();
            
            TableCellProperties cellProps = new TableCellProperties(
                new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "2500" },
                new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Top }
            );
            cell.AppendChild(cellProps);

            // STT và tên công ty
            Paragraph namePara = new Paragraph();
            Run nameRun = namePara.AppendChild(new Run());
            nameRun.AppendChild(new Text($"{index}. {company.CompanyName}"));
            RunProperties nameRunProps = nameRun.InsertAt(new RunProperties(), 0);
            nameRunProps.AppendChild(new Bold());
            nameRunProps.AppendChild(new FontSize() { Val = "22" });
            ParagraphProperties nameParaProps = namePara.InsertAt(new ParagraphProperties(), 0);
            nameParaProps.AppendChild(new SpacingBetweenLines() { After = "100" });
            cell.AppendChild(namePara);

            // Người đại diện
            if (!string.IsNullOrWhiteSpace(company.RepresentativeName))
            {
                cell.AppendChild(CreateInfoParagraph("Người đại diện:", company.RepresentativeName));
            }

            // Số điện thoại
            if (!string.IsNullOrWhiteSpace(company.PhoneNumber))
            {
                cell.AppendChild(CreateInfoParagraph("Điện thoại:", company.PhoneNumber));
            }

            // Địa chỉ
            if (!string.IsNullOrWhiteSpace(company.Address))
            {
                cell.AppendChild(CreateInfoParagraph("Địa chỉ:", company.Address));
            }

            // Tình trạng
            if (!string.IsNullOrWhiteSpace(company.Status))
            {
                cell.AppendChild(CreateInfoParagraph("Tình trạng:", company.Status));
            }

            // Ngành nghề
            if (!string.IsNullOrWhiteSpace(company.BusinessSector))
            {
                cell.AppendChild(CreateInfoParagraph("Ngành nghề:", company.BusinessSector));
            }

            // Ngày hoạt động
            if (!string.IsNullOrWhiteSpace(company.ActiveDate))
            {
                cell.AppendChild(CreateInfoParagraph("Ngày hoạt động:", company.ActiveDate));
            }

            // Phân loại
            var classifications = new System.Collections.Generic.List<string>();
            if (company.HasAppointment) classifications.Add("Có hẹn");
            if (company.IsConsidering) classifications.Add("Đang cân nhắc");
            if (company.NoNeed) classifications.Add("Không cần");

            if (classifications.Any())
            {
                cell.AppendChild(CreateInfoParagraph("Phân loại:", string.Join(", ", classifications)));
            }

            return cell;
        }

        private Paragraph CreateInfoParagraph(string label, string value)
        {
            Paragraph para = new Paragraph();
            ParagraphProperties paraProps = para.AppendChild(new ParagraphProperties());
            paraProps.AppendChild(new SpacingBetweenLines() { After = "80" });

            // Label in bold
            Run labelRun = para.AppendChild(new Run());
            labelRun.AppendChild(new Text(label + " "));
            RunProperties labelRunProps = labelRun.InsertAt(new RunProperties(), 0);
            labelRunProps.AppendChild(new Bold());
            labelRunProps.AppendChild(new FontSize() { Val = "20" });

            // Value
            Run valueRun = para.AppendChild(new Run());
            valueRun.AppendChild(new Text(value));
            RunProperties valueRunProps = valueRun.InsertAt(new RunProperties(), 0);
            valueRunProps.AppendChild(new FontSize() { Val = "20" });

            return para;
        }

        private void ShowInfoBar(string message, InfoBarSeverity severity)
        {
            NotificationInfoBar.Message = message;
            NotificationInfoBar.Severity = severity;
            NotificationInfoBar.IsOpen = true;
        }
    }
}
