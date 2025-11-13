using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace cowl.Views
{
    /// <summary>
    /// Trang hiển thị các công ty không có nhu cầu
    /// </summary>
    public sealed partial class NoNeedPage : Page
    {
        private Services.CompanyDataService DataService => Services.CompanyDataService.Instance;

        public ObservableCollection<Models.CompanyInfo> NoNeedCompanies { get; } = new ObservableCollection<Models.CompanyInfo>();

        public NoNeedPage()
        {
            this.InitializeComponent();
            LoadNoNeedCompanies();
            
            // Đăng ký sự kiện để cập nhật khi có thay đổi
            DataService.Companies.CollectionChanged += (s, e) => LoadNoNeedCompanies();
            
            // Lắng nghe thay đổi của từng công ty
            foreach (var company in DataService.Companies)
            {
                company.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Models.CompanyInfo.NoNeed))
                    {
                        LoadNoNeedCompanies();
                    }
                };
            }
        }

        private void LoadNoNeedCompanies()
        {
            NoNeedCompanies.Clear();
            foreach (var company in DataService.Companies.Where(c => c.NoNeed))
            {
                NoNeedCompanies.Add(company);
            }
        }
    }
}
