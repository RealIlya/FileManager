using System.Windows.Controls;

namespace FileManager.Models
{
    public class TabElement
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string? Extension { get; set; }

        public UserControl Content { get; set; }
        
        public TabElement()
        {
            Name = null!;
            FullName = null!;
            Content = null!;
        }
    }
}