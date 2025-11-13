using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace cowl.Views
{
    /// <summary>
    /// Trang hiển thị các công ty đang phân vân
    /// </summary>
    public sealed partial class ConsideringPage : Page
    {
        private Services.CompanyDataService DataService => Services.CompanyDataService.Instance;

        public ObservableCollection<Models.CompanyInfo> ConsideringCompanies { get; } = new ObservableCollection<Models.CompanyInfo>();

        public ConsideringPage()
        {
            this.InitializeComponent();
            LoadConsideringCompanies();
            
            // Đăng ký sự kiện để cập nhật khi có thay đổi
            DataService.Companies.CollectionChanged += (s, e) => LoadConsideringCompanies();
            
            // Lắng nghe thay đổi của từng công ty
            foreach (var company in DataService.Companies)
            {
                company.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Models.CompanyInfo.IsConsidering))
                    {
                        LoadConsideringCompanies();
                    }
                };
            }
        }

        private void LoadConsideringCompanies()
        {
            ConsideringCompanies.Clear();
            foreach (var company in DataService.Companies.Where(c => c.IsConsidering))
            {
                ConsideringCompanies.Add(company);
            }
        }
    }
}
