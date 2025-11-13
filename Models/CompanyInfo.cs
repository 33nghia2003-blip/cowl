using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace cowl.Models
{
    /// <summary>
    /// Model để lưu thông tin công ty
    /// </summary>
    public class CompanyInfo : INotifyPropertyChanged
    {
        private string _companyName = string.Empty;
        private string _representativeName = string.Empty;
        private string _phoneNumber = string.Empty;
        private string _address = string.Empty;
        private string _status = string.Empty;
        private string _businessSector = string.Empty;
        private string _activeDate = string.Empty;
        private bool _hasAppointment = false;
        private bool _isConsidering = false;
        private bool _noNeed = false;

        public string CompanyName
        {
            get => _companyName;
            set
            {
                if (_companyName != value)
                {
                    _companyName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string RepresentativeName
        {
            get => _representativeName;
            set
            {
                if (_representativeName != value)
                {
                    _representativeName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public string BusinessSector
        {
            get => _businessSector;
            set
            {
                if (_businessSector != value)
                {
                    _businessSector = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ActiveDate
        {
            get => _activeDate;
            set
            {
                if (_activeDate != value)
                {
                    _activeDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasAppointment
        {
            get => _hasAppointment;
            set
            {
                if (_hasAppointment != value)
                {
                    _hasAppointment = value;
                    OnPropertyChanged();
                    // Khi chọn một option, bỏ chọn các option khác
                    if (value)
                    {
                        IsConsidering = false;
                        NoNeed = false;
                    }
                }
            }
        }

        public bool IsConsidering
        {
            get => _isConsidering;
            set
            {
                if (_isConsidering != value)
                {
                    _isConsidering = value;
                    OnPropertyChanged();
                    // Khi chọn một option, bỏ chọn các option khác
                    if (value)
                    {
                        HasAppointment = false;
                        NoNeed = false;
                    }
                }
            }
        }

        public bool NoNeed
        {
            get => _noNeed;
            set
            {
                if (_noNeed != value)
                {
                    _noNeed = value;
                    OnPropertyChanged();
                    // Khi chọn một option, bỏ chọn các option khác
                    if (value)
                    {
                        HasAppointment = false;
                        IsConsidering = false;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
