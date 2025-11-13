using System.Collections.ObjectModel;

namespace cowl.Services
{
    /// <summary>
    /// Service để chia sẻ dữ liệu giữa các trang
    /// </summary>
    public class CompanyDataService
    {
        private static CompanyDataService? _instance;
        public static CompanyDataService Instance => _instance ??= new CompanyDataService();

        private CompanyDataService() { }

        public ObservableCollection<Models.CompanyInfo> Companies { get; } = new ObservableCollection<Models.CompanyInfo>();
    }
}
