using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;

namespace cowl.Views
{
    /// <summary>
    /// Trang hiển thị - nơi thể hiện các thông tin đã dán
    /// </summary>
    public sealed partial class DisplayPage : Page
    {
        private Services.CompanyDataService DataService => Services.CompanyDataService.Instance;

        public ObservableCollection<Models.CompanyInfo> Companies => DataService.Companies;

        public int TotalCompanies => DataService.Companies.Count;
        
        public int ClassifiedCompanies => DataService.Companies.Count(c => 
            c.HasAppointment || c.IsConsidering || c.NoNeed);
        
        public int UnclassifiedCompanies => DataService.Companies.Count(c => 
            !c.HasAppointment && !c.IsConsidering && !c.NoNeed);

        public DisplayPage()
        {
            this.InitializeComponent();
            
            // Lắng nghe thay đổi để cập nhật thống kê
            DataService.Companies.CollectionChanged += (s, e) =>
            {
                UpdateStatistics();
            };
            
            // Lắng nghe thay đổi phân loại
            foreach (var company in DataService.Companies)
            {
                company.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Models.CompanyInfo.HasAppointment) ||
                        e.PropertyName == nameof(Models.CompanyInfo.IsConsidering) ||
                        e.PropertyName == nameof(Models.CompanyInfo.NoNeed))
                    {
                        UpdateStatistics();
                    }
                };
            }
        }

        private void UpdateStatistics()
        {
            Bindings.Update();
        }
    }
}
