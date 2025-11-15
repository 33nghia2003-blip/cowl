using System.Collections.ObjectModel;
using MongoDB.Driver;
using cowl.Models;

namespace cowl.Services
{
    /// <summary>
    /// Service để quản lý dữ liệu công ty trên MongoDB
    /// </summary>
    public class CompanyDataService
    {
        private static CompanyDataService? _instance;
        private readonly IMongoCollection<Company> _companiesCollection;

        public static CompanyDataService Instance => _instance ??= new CompanyDataService();

        public ObservableCollection<Models.CompanyInfo> Companies { get; } = new ObservableCollection<Models.CompanyInfo>();

        private CompanyDataService()
        {
            var connectionString = "mongodb+srv://nghia:Huong1505@cowl.fwmasrq.mongodb.net/?appName=cowl";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("cowl");
            _companiesCollection = database.GetCollection<Company>("companies");

            // Tạo index cho userId để tìm kiếm nhanh
            var indexKeysDefinition = Builders<Company>.IndexKeys.Ascending(c => c.UserId);
            var indexModel = new CreateIndexModel<Company>(indexKeysDefinition);
            _companiesCollection.Indexes.CreateOne(indexModel);
        }

        /// <summary>
        /// Lấy tất cả công ty của user hiện tại
        /// </summary>
        public async Task LoadCompaniesForCurrentUserAsync()
        {
            var authService = AuthService.Instance;
            if (authService.CurrentUser == null)
                return;

            var companies = await _companiesCollection
                .Find(c => c.UserId == authService.CurrentUser.Id)
                .SortByDescending(c => c.UpdatedAt)
                .ToListAsync();

            Companies.Clear();
            foreach (var company in companies)
            {
                Companies.Add(ConvertToCompanyInfo(company));
            }
        }

        /// <summary>
        /// Thêm công ty mới
        /// </summary>
        public async Task<(bool success, string message)> AddCompanyAsync(Models.CompanyInfo companyInfo)
        {
            try
            {
                var authService = AuthService.Instance;
                if (authService.CurrentUser == null)
                    return (false, "Bạn chưa đăng nhập!");

                var company = ConvertToCompany(companyInfo);
                company.UserId = authService.CurrentUser.Id!;
                company.CreatedAt = DateTime.UtcNow;
                company.UpdatedAt = DateTime.UtcNow;

                await _companiesCollection.InsertOneAsync(company);
                Companies.Add(companyInfo);

                return (true, "Thêm công ty thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật thông tin công ty
        /// </summary>
        public async Task<(bool success, string message)> UpdateCompanyAsync(string companyId, Models.CompanyInfo companyInfo)
        {
            try
            {
                var authService = AuthService.Instance;
                if (authService.CurrentUser == null)
                    return (false, "Bạn chưa đăng nhập!");

                var company = ConvertToCompany(companyInfo);
                company.Id = companyId;
                company.UserId = authService.CurrentUser.Id!;
                company.UpdatedAt = DateTime.UtcNow;

                var filter = Builders<Company>.Filter.And(
                    Builders<Company>.Filter.Eq(c => c.Id, companyId),
                    Builders<Company>.Filter.Eq(c => c.UserId, authService.CurrentUser.Id)
                );

                var result = await _companiesCollection.ReplaceOneAsync(filter, company);
                
                if (result.ModifiedCount == 0)
                    return (false, "Không tìm thấy công ty hoặc bạn không có quyền sửa!");

                return (true, "Cập nhật công ty thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa công ty
        /// </summary>
        public async Task<(bool success, string message)> DeleteCompanyAsync(string companyId)
        {
            try
            {
                var authService = AuthService.Instance;
                if (authService.CurrentUser == null)
                    return (false, "Bạn chưa đăng nhập!");

                var filter = Builders<Company>.Filter.And(
                    Builders<Company>.Filter.Eq(c => c.Id, companyId),
                    Builders<Company>.Filter.Eq(c => c.UserId, authService.CurrentUser.Id)
                );

                var result = await _companiesCollection.DeleteOneAsync(filter);
                
                if (result.DeletedCount == 0)
                    return (false, "Không tìm thấy công ty hoặc bạn không có quyền xóa!");

                return (true, "Xóa công ty thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Lọc công ty theo trạng thái
        /// </summary>
        public async Task<List<Models.CompanyInfo>> GetCompaniesByStatusAsync(string status)
        {
            var authService = AuthService.Instance;
            if (authService.CurrentUser == null)
                return new List<Models.CompanyInfo>();

            FilterDefinition<Company> filter;
            
            if (status == "Appointment")
            {
                filter = Builders<Company>.Filter.And(
                    Builders<Company>.Filter.Eq(c => c.UserId, authService.CurrentUser.Id),
                    Builders<Company>.Filter.Eq(c => c.HasAppointment, true)
                );
            }
            else if (status == "Considering")
            {
                filter = Builders<Company>.Filter.And(
                    Builders<Company>.Filter.Eq(c => c.UserId, authService.CurrentUser.Id),
                    Builders<Company>.Filter.Eq(c => c.IsConsidering, true)
                );
            }
            else if (status == "NoNeed")
            {
                filter = Builders<Company>.Filter.And(
                    Builders<Company>.Filter.Eq(c => c.UserId, authService.CurrentUser.Id),
                    Builders<Company>.Filter.Eq(c => c.NoNeed, true)
                );
            }
            else
            {
                filter = Builders<Company>.Filter.Eq(c => c.UserId, authService.CurrentUser.Id);
            }

            var companies = await _companiesCollection
                .Find(filter)
                .SortByDescending(c => c.UpdatedAt)
                .ToListAsync();

            return companies.Select(c => ConvertToCompanyInfo(c)).ToList();
        }

        private Company ConvertToCompany(Models.CompanyInfo info)
        {
            return new Company
            {
                CompanyName = info.CompanyName,
                RepresentativeName = info.RepresentativeName,
                PhoneNumber = info.PhoneNumber,
                Address = info.Address,
                Status = info.Status,
                BusinessSector = info.BusinessSector,
                ActiveDate = info.ActiveDate,
                HasAppointment = info.HasAppointment,
                IsConsidering = info.IsConsidering,
                NoNeed = info.NoNeed,
                Note = info.Note,
                AppointmentDate = info.AppointmentDate?.DateTime
            };
        }

        private Models.CompanyInfo ConvertToCompanyInfo(Company company)
        {
            var info = new Models.CompanyInfo
            {
                Id = company.Id ?? string.Empty,
                CompanyName = company.CompanyName,
                RepresentativeName = company.RepresentativeName,
                PhoneNumber = company.PhoneNumber,
                Address = company.Address,
                Status = company.Status,
                BusinessSector = company.BusinessSector,
                ActiveDate = company.ActiveDate,
                HasAppointment = company.HasAppointment,
                IsConsidering = company.IsConsidering,
                NoNeed = company.NoNeed,
                Note = company.Note,
                AppointmentDate = company.AppointmentDate.HasValue ? new DateTimeOffset(company.AppointmentDate.Value) : null
            };

            // Đăng ký sự kiện để tự động lưu khi có thay đổi
            info.PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(Models.CompanyInfo.HasAppointment) ||
                    e.PropertyName == nameof(Models.CompanyInfo.IsConsidering) ||
                    e.PropertyName == nameof(Models.CompanyInfo.NoNeed) ||
                    e.PropertyName == nameof(Models.CompanyInfo.Note) ||
                    e.PropertyName == nameof(Models.CompanyInfo.AppointmentDate))
                {
                    if (!string.IsNullOrEmpty(info.Id))
                    {
                        await UpdateCompanyAsync(info.Id, info);
                    }
                }
            };

            return info;
        }

        /// <summary>
        /// Xóa tất cả công ty của user hiện tại
        /// </summary>
        public async Task<(bool success, string message)> DeleteAllCompaniesAsync()
        {
            try
            {
                var authService = AuthService.Instance;
                if (authService.CurrentUser == null)
                    return (false, "Bạn chưa đăng nhập!");

                var filter = Builders<Company>.Filter.Eq(c => c.UserId, authService.CurrentUser.Id);
                var result = await _companiesCollection.DeleteManyAsync(filter);
                
                // Xóa dữ liệu local
                Companies.Clear();

                return (true, $"Đã xóa {result.DeletedCount} công ty!");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa tất cả dữ liệu local khi logout
        /// </summary>
        public void ClearLocalData()
        {
            Companies.Clear();
        }
    }
}
