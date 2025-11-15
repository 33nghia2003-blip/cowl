using Microsoft.UI.Xaml.Controls;
using System;
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

        public ObservableCollection<Models.CompanyInfo> FilteredCompanies { get; } = new ObservableCollection<Models.CompanyInfo>();

        public int TotalCompanies => DataService.Companies.Count;
        
        public int ClassifiedCompanies => DataService.Companies.Count(c => 
            c.HasAppointment || c.IsConsidering || c.NoNeed);
        
        public int UnclassifiedCompanies => DataService.Companies.Count(c => 
            !c.HasAppointment && !c.IsConsidering && !c.NoNeed);

        private string _searchText = string.Empty;

        public DisplayPage()
        {
            this.InitializeComponent();
            
            // Load initial data
            LoadFilteredCompanies();
            
            // Lắng nghe thay đổi để cập nhật thống kê
            DataService.Companies.CollectionChanged += (s, e) =>
            {
                UpdateStatistics();
                LoadFilteredCompanies();
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

        private void OnSearchTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                _searchText = sender.Text;
                LoadFilteredCompanies();
            }
        }

        private void LoadFilteredCompanies()
        {
            FilteredCompanies.Clear();

            var query = DataService.Companies.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                var searchLower = _searchText.ToLower();
                query = query.Where(c =>
                    c.CompanyName.ToLower().Contains(searchLower) ||
                    c.RepresentativeName.ToLower().Contains(searchLower) ||
                    c.PhoneNumber.Contains(searchLower));
            }

            foreach (var company in query)
            {
                FilteredCompanies.Add(company);
            }
        }

        private void UpdateStatistics()
        {
            Bindings.Update();
        }
    }
}
