using System.Windows.Controls;

namespace FileManager.Views
{
    public partial class TextPage : Page
    {
        private static TextPage? _instance;

        public static TextPage GetInstance()
        {
            return _instance ??= new TextPage();
        }

        public TextPage()
        {
            InitializeComponent();
        }
    }
}