using System.Windows.Documents;

namespace FileManager.ViewModels
{
    public class TextViewModel : BaseVM
    {
        private string _currentText;

        public string CurrentText
        {
            get => _currentText;
            set
            {
                if (_currentText == value) return;

                _currentText = value;
                OnPropertyChanged("CurrentText");
            }
        }
    }
}