namespace FileManager.Abstractions
{
    public interface IFolderElement
    {
        string Name { get; set; }
        string FullName { get; set; }

    }
}