using System.Collections.ObjectModel;

namespace FileManager.Models
{
    public class Element
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string? Extension { get; set; }

        public ObservableCollection<Element> Children { get; set; }

        public Element()
        {
            Name = null!;
            FullName = null!;
            Children = new ObservableCollection<Element>();
        }
    }
}