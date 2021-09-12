using FileManager.Abstractions;

namespace FileManager.Models
{
    public class MyFile : IFolderElement
    {

        public string Name { get; set; }
        public string FullName { get; set; }
    }
}