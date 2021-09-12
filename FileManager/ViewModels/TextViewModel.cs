using System.Windows.Documents;

namespace FileManager.ViewModels
{
    public class TextViewModel : BaseVM
    {
        private string _textFile;

        public string TextFile
        {
            get => _textFile;
            set
            {
                if (_textFile == value) return;

                _textFile = value;
                OnPropertyChanged("TextFile");
            }
        }
    }
}