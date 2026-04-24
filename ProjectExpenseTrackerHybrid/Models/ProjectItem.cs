using System.ComponentModel;

namespace ProjectExpenseTrackerHybrid.Models
{
    public class ProjectItem : INotifyPropertyChanged
    {
        private bool _isFavourite;

        public string ProjectCode { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectDescription { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string ProjectManager { get; set; } = string.Empty;
        public string ProjectStatus { get; set; } = string.Empty;
        public double ProjectBudget { get; set; }
        public string SpecialRequirements { get; set; } = string.Empty;
        public string ClientInfo { get; set; } = string.Empty;
        public string ExactLocation { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public long LastModified { get; set; }
        public int Synced { get; set; }

        public bool IsFavourite
        {
            get => _isFavourite;
            set
            {
                if (_isFavourite == value)
                    return;

                _isFavourite = value;
                OnPropertyChanged(nameof(IsFavourite));
                OnPropertyChanged(nameof(FavouriteText));
            }
        }

        public string FavouriteText => IsFavourite ? "★ Favourite" : "☆ Add Favourite";

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}