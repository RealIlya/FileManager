using System.Windows.Controls;

namespace FileManager.Views
{
    public partial class PictureUserControl : UserControl
    {
        private static PictureUserControl? _instance;

        public static PictureUserControl GetInstance()
        {
            return _instance ??= new PictureUserControl();
        }
        
        public PictureUserControl()
        {
            InitializeComponent();
        }
    }
}