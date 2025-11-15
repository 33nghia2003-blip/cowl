<div align="center">

# 🦅 Cowl

### Enterprise Information Management System

*Ứng dụng quản lý và phân loại thông tin doanh nghiệp với giao diện Windows 11*

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![WinUI 3](https://img.shields.io/badge/WinUI-3.0-0078D4?style=flat-square&logo=windows)](https://learn.microsoft.com/windows/apps/winui/)
[![Platform](https://img.shields.io/badge/Platform-ARM64-00A4EF?style=flat-square&logo=windows)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/License-Proprietary-red?style=flat-square)](LICENSE)

</div>

---

## 📌 Giới thiệu

**Cowl** là ứng dụng desktop hiện đại xây dựng trên **WinUI 3** và **.NET 9.0**, giúp tự động hóa quản lý thông tin doanh nghiệp với khả năng:

- 🤖 **Trích xuất dữ liệu thông minh** từ văn bản không cấu trúc
- 📊 **Phân loại 3 cấp độ** (Có hẹn / Đang cân nhắc / Không cần)
- 📈 **Thống kê real-time** với dashboard trực quan
- 🎨 **Giao diện Windows 11** với Mica backdrop
- 🔐 **Hệ thống đăng nhập/đăng ký** với MongoDB
- ☁️ **Đồng bộ cloud** - dữ liệu riêng biệt cho từng user

---

## ✨ Tính năng chính

### 🤖 Trích xuất dữ liệu tự động
- **Regex parsing** hỗ trợ đầy đủ Unicode tiếng Việt
- Tự động nhận diện: Tên công ty, Người đại diện, Số điện thoại, Địa chỉ, Ngành nghề, Trạng thái
- Xử lý nhiều format: colon-separated, tab-separated, multi-line
- Độ chính xác: 85-99% tùy loại dữ liệu

### 📄 Xuất file Word
- **Export 2-column layout** - tối ưu không gian trang giấy
- **Định dạng chuyên nghiệp** - tiêu đề, border, spacing
- **Đầy đủ thông tin** - xuất toàn bộ dữ liệu từ cards
- **Auto-naming** - `DanhSachCongTy_YYYYMMDD_HHmmss.docx`
- **Keyboard shortcut** - Ctrl+E để xuất nhanh

### 📊 Hệ thống phân loại
- **🟢 Có hẹn** - Doanh nghiệp đã xác nhận lịch hẹn (Green border)
- **🟡 Đang cân nhắc** - Đang trong giai đoạn đánh giá (Yellow border)
- **🔴 Không cần** - Không phù hợp hoặc từ chối (Red border)
- Mutual exclusion logic - chỉ 1 trạng thái active
- Auto-sync real-time across all pages

### 🔐 Authentication & Security
- **Đăng ký/Đăng nhập** - Form validation đầy đủ
- **Password strength indicator** - Real-time feedback
- **BCrypt hashing** - Mã hóa mật khẩu an toàn (cost factor 11)
- **User isolation** - Mỗi user có dữ liệu riêng biệt
- **MongoDB Atlas** - Cloud database với replica set
- **Auto-save** - Tự động đồng bộ mọi thay đổi

### 📈 Dashboard & Analytics
- 3 KPI: **Tổng số công ty** | **Đã phân loại** | **Chưa phân loại**
- Live updates với ObservableCollection
- Newest-first ordering
- Filtered views cho từng category
- **User profile** - Hiển thị tên và email người dùng

### 🎨 Modern UI/UX
- **Mica backdrop** với transparency effect (Windows 11)
- **Custom TitleBar** với Tall height (48px)
- **NavigationView** với 5 pages
- **Fluent Design** components
- **Responsive layout** - adaptive cards

---

## 🛠️ Công nghệ

### Core Stack
- **.NET 9.0** - Runtime với C# 13
- **WinUI 3** - Modern UI framework
- **Windows App SDK 1.6+** - Native APIs
- **DocumentFormat.OpenXml 3.3.0** - Word export
- **MongoDB.Driver 2.28.0** - Cloud database
- **BCrypt.Net-Next 4.0.3** - Password hashing
- **ARM64** platform

### Architecture
- **MVVM** pattern
- **Singleton** DataService
- **Observer** pattern (INotifyPropertyChanged)
- **ObservableCollection** reactive UI
- **x:Bind** compiled binding (~10x performance)

### Key Features
- Regex text parsing
- LINQ filtering
- Mica/Acrylic materials
- Real-time statistics

---

## 🏗️ Cấu trúc dự án

```
cowl/
├── Models/
│   ├── CompanyInfo.cs              # Entity với INotifyPropertyChanged
│   └── User.cs                     # User & Company models cho MongoDB
├── Services/
│   ├── CompanyDataService.cs       # Singleton service + MongoDB sync
│   └── AuthService.cs              # Authentication với MongoDB
├── Converters/
│   └── EmptyStringToVisibilityConverter.cs
├── Views/
│   ├── LoginPage.xaml/.cs          # Đăng nhập
│   ├── RegisterPage.xaml/.cs       # Đăng ký
│   ├── InputPage.xaml/.cs          # Data entry + regex parsing
│   ├── DisplayPage.xaml/.cs        # Dashboard + statistics
│   ├── AppointmentPage             # Filtered: HasAppointment
│   ├── ConsideringPage             # Filtered: IsConsidering
│   ├── NoNeedPage                  # Filtered: NoNeed
│   └── MainPage                    # NavigationView container
├── MainWindow.xaml/.cs              # App window + TitleBar
└── App.xaml/.cs                    # Entry point
```

**Data Flow:**
```
User Input → Regex Parse → CompanyInfo → DataService (ObservableCollection)
  ↓
Display + Filtered Pages (LINQ) → Real-time Statistics
```

---

## 🚀 Cài đặt

### Yêu cầu
- **Windows 10** version 1809+ (khuyến nghị **Windows 11**)
- **.NET 9.0 SDK**
- **Visual Studio 2022** (17.8+) hoặc **Rider 2024.3+**
- **4GB RAM** (khuyến nghị 8GB)

### Clone & Run

```powershell
# Clone repository
git clone https://github.com/33nghia2003-blip/cowl.git
cd cowl

# Restore packages
dotnet restore

# Build & Run
dotnet build -c Release -r win-arm64
dotnet run
```

### Publish

```powershell
# Self-contained (không cần .NET runtime)
dotnet publish -c Release -r win-arm64 --self-contained true -p:PublishSingleFile=true

# Framework-dependent (nhỏ hơn, cần .NET runtime)
dotnet publish -c Release -r win-arm64 --self-contained false

# Multi-platform
dotnet publish -c Release -r win-x64 --self-contained true
dotnet publish -c Release -r win-x86 --self-contained true
```

---

## 📖 Hướng dẫn sử dụng

### Quick Start
1. **Đăng ký tài khoản**: Tạo tài khoản mới với username, email, mật khẩu
2. **Đăng nhập**: Sử dụng username/email và mật khẩu để đăng nhập
3. **Nhập dữ liệu**: Click **"Nhập liệu"** → paste văn bản công ty → **"Thêm công ty"**
4. **Xem kết quả**: Tại **"Hiển thị"** - dữ liệu tự động đồng bộ lên MongoDB

### Workflow cơ bản

#### 0. Đăng ký & Đăng nhập
**Đăng ký:**
- Nhập họ tên, username (tối thiểu 3 ký tự)
- Email hợp lệ (kiểm tra format và uniqueness)
- Mật khẩu (tối thiểu 6 ký tự) với strength indicator
- Xác nhận mật khẩu khớp
- Đồng ý điều khoản dịch vụ

**Đăng nhập:**
- Username hoặc Email
- Mật khẩu
- Tùy chọn: Ghi nhớ đăng nhập
- Dữ liệu tự động load từ MongoDB

#### 1. Nhập dữ liệu
Paste văn bản có format:
```
Tên doanh nghiệp: CÔNG TY TNHH ABC
Người đại diện: NGUYỄN VĂN A
Số điện thoại: 0901234567
Địa chỉ: 123 Đường XYZ, Quận 1, TP.HCM
Trạng thái: Đang hoạt động
Ngành nghề: Công nghệ thông tin
```

Hỗ trợ: `Label: Value`, `Label    Value`, multi-line

#### 2. Phân loại
Chọn 1 trong 3 checkbox cho mỗi công ty:
- ✅ **Có hẹn** → chuẩn bị cuộc họp
- ⏸️ **Đang cân nhắc** → follow-up
- ❌ **Không cần** → archive

Auto-uncheck 2 checkbox còn lại, statistics tự động update

#### 3. Xem filtered lists
- **Có hẹn** - Green border
- **Đang cân nhắc** - Yellow border
- **Không cần** - Red border

### Keyboard Shortcuts
- `Ctrl+E` - Export to Word
- `Ctrl+Enter` - Add Company
- `Ctrl+Delete` - Clear Input
- `Alt+Arrow` - Toggle Navigation

---

## 🔧 Cấu hình

### Window size
Chỉnh `MainWindow.xaml.cs`:
```csharp
size.Width = 1280;   // Default: 1280×960
size.Height = 960;
```

### Regex patterns
Tùy chỉnh trong `Views/InputPage.xaml.cs`:
```csharp
// Tên công ty (uppercase Vietnamese)
@"^([A-ZÀÁẠẢÃ...]+)"

// Số điện thoại
@"\b(0[0-9]{9,10})\b"
```

### Backdrop
Thay đổi trong `MainWindow.xaml.cs`:
```csharp
this.SystemBackdrop = new MicaBackdrop();           // Windows 11
this.SystemBackdrop = new DesktopAcrylicBackdrop(); // Alternative
```

---

## 🚧 Roadmap

### Version 2.0 (Completed) ✅
- [x] Export to Word (2-column layout)
- [x] MongoDB database persistence
- [x] User authentication system
- [x] Cloud sync per user
- [x] Password hashing with BCrypt
- [x] Auto-save on changes

### Version 2.1 (In Progress)
- [ ] Edit/Delete companies
- [ ] Export to Excel/CSV
- [ ] Import from file
- [ ] Full-text search
- [ ] Undo/Redo
- [ ] Backup & Restore
- [ ] Change password
- [ ] Forgot password via email

### Version 3.0 (Future)
- [ ] AI-powered classification
- [ ] OCR support
- [ ] Duplicate detection
- [ ] Predictive analytics
- [ ] CRM integration

---

## 📝 Giấy phép

**Proprietary License** - Copyright © 2025 Nghia. All Rights Reserved.

| Permission | Status |
|------------|--------|
| ✅ Use | Authorized only |
| ❌ Copy | Prohibited |
| ❌ Modify | Prohibited |
| ❌ Distribute | Prohibited |
| ❌ Reverse Engineer | Prohibited |

Xem chi tiết tại [LICENSE](LICENSE)

**⚠️ Lưu ý**: Ứng dụng chỉ dùng nội bộ, **KHÔNG ĐƯỢC** phân phối dưới mọi hình thức.

---

## 👤 Tác giả

**Nghia**
- GitHub: [@33nghia2003-blip](https://github.com/33nghia2003-blip)
- Repository: [github.com/33nghia2003-blip/cowl](https://github.com/33nghia2003-blip/cowl)

### Credits
- Microsoft WinUI 3 Team
- .NET Team
- Windows App SDK
- WinUI Gallery (design reference)

---

## 📊 Project Info

```
Language:      C# 13.0
Lines of Code: ~4,000
Files:         22
Target:        Windows 10.0.19041.0+
Platform:      ARM64, x64, x86
Version:       2.0.0
Released:      November 2025
Database:      MongoDB Atlas
```

## 🔌 Database Schema

### Users Collection
```json
{
  "_id": ObjectId,
  "username": String (unique),
  "email": String (unique),
  "passwordHash": String (BCrypt),
  "fullName": String,
  "createdAt": DateTime,
  "lastLogin": DateTime,
  "isActive": Boolean
}
```

### Companies Collection
```json
{
  "_id": ObjectId,
  "userId": ObjectId (indexed),
  "companyName": String,
  "representativeName": String,
  "phoneNumber": String,
  "address": String,
  "status": String,
  "businessSector": String,
  "activeDate": String,
  "hasAppointment": Boolean,
  "isConsidering": Boolean,
  "noNeed": Boolean,
  "createdAt": DateTime,
  "updatedAt": DateTime
}
```

---

<div align="center">

**Made with ❤️ using WinUI 3 and .NET 9**

⭐ Star repository nếu project hữu ích!

[⬆ Back to Top](#-cowl)

</div>
