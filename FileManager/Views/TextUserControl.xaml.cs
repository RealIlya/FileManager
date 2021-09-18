using System.Windows.Controls;

namespace FileManager.Views
{
    public partial class TextUserControl : UserControl
    {
        private static TextUserControl? _instance;

        public static TextUserControl GetInstance()
        {
            return _instance ??= new TextUserControl();
        }

        public TextUserControl()
        {
            InitializeComponent();
        }
    }
}