using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace cowl.Views
{
    /// <summary>
    /// Trang hiển thị các công ty có hẹn
    /// </summary>
    public sealed partial class AppointmentPage : Page
    {
        private Services.CompanyDataService DataService => Services.CompanyDataService.Instance;

        public ObservableCollection<Models.CompanyInfo> AppointmentCompanies { get; } = new ObservableCollection<Models.CompanyInfo>();

        public AppointmentPage()
        {
            this.InitializeComponent();
            LoadAppointmentCompanies();
            
            // Đăng ký sự kiện để cập nhật khi có thay đổi
            DataService.Companies.CollectionChanged += (s, e) => LoadAppointmentCompanies();
            
            // Lắng nghe thay đổi của từng công ty
            foreach (var company in DataService.Companies)
            {
                company.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Models.CompanyInfo.HasAppointment))
                    {
                        LoadAppointmentCompanies();
                    }
                };
            }
        }

        private void LoadAppointmentCompanies()
        {
            AppointmentCompanies.Clear();
            foreach (var company in DataService.Companies.Where(c => c.HasAppointment))
            {
                AppointmentCompanies.Add(company);
            }
        }
    }
}
