using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileManager.Abstractions;

namespace FileManager.Models
{
    public class MyFolder : IFolderElement
    {
        public string Name { get; set; }
        public string FullName { get; set; }

        public ObservableCollection<MyFolder> ChildFolders { get; set; }
        public ObservableCollection<MyFile> ChildFiles { get; set; }

        public MyFolder()
        {
            ChildFolders = new ObservableCollection<MyFolder>();
            ChildFiles = new ObservableCollection<MyFile>();
        }
    }
}