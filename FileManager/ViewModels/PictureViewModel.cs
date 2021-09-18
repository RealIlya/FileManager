namespace FileManager.ViewModels
{
    public class PictureViewModel : BaseVM
    {
        private string _currentPicture;

        public string CurrentPicture
        {
            get => _currentPicture;
            set
            {
                if (_currentPicture == value) return;

                _currentPicture = value;
                OnPropertyChanged("CurrentPicture");
            }
        }
    }
}