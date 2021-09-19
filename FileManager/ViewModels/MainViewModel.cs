using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using DevExpress.Mvvm;
using FileManager.Models;
using FileManager.Views;
using Microsoft.WindowsAPICodePack.Dialogs;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

// TODO доделать отображение всех страниц, оптимизировать код

namespace FileManager.ViewModels
{
    public class MainViewModel : BaseVM
    {
        private Element _selectedFile;
        private TabElement _selectedTabElement;

        public ObservableCollection<Element> Elements { get; set; }
        public ObservableCollection<TabElement> Pages { get; set; }

        public Element SelectedFile
        {
            get => _selectedFile;
            set
            {
                if (_selectedFile == value) return;

                _selectedFile = value;
                // if (_selectedFile.Extension is null) _selectedFile = null;

                OnPropertyChanged("SelectedFile");
            }
        }

        public TabElement SelectedTabElement
        {
            get => _selectedTabElement;
            set
            {
                if (_selectedTabElement == value) return;

                _selectedTabElement = value;
                OnPropertyChanged("SelectedTabElement");
            }
        }

        public MainViewModel()
        {
            _selectedFile = null!;
            _selectedTabElement = null!;
            Elements = new ObservableCollection<Element>();
            Pages = new ObservableCollection<TabElement>();
        }

        public ICommand OpenTree =>
            new DelegateCommand(async () =>
            {
                void LoadChildElements(IEnumerable<Element> currentDirectory)
                {
                    var currentElement = currentDirectory.LastOrDefault();
                    var startPath = currentElement.FullName;
                    var currentFolder = currentElement.Children;
                    var searchDirectory = new DirectoryInfo(startPath).GetDirectories();

                    if (searchDirectory.Length > 0)
                    {
                        foreach (var directory in searchDirectory)
                        {
                            currentFolder.Add(new Element() { Name = directory.Name, FullName = directory.FullName });
                            LoadChildElements(currentFolder);
                        }
                    }

                    var files = new DirectoryInfo(startPath).GetFiles();

                    foreach (var file in files)
                    {
                        currentFolder.Add(new Element()
                            { Name = file.Name, FullName = file.FullName, Extension = file.Extension });
                    }
                }

                // var fbd = new FolderBrowserDialog();
                var fbd = new CommonOpenFileDialog();

                fbd.Title = "Select the Path";
                fbd.IsFolderPicker = true;

                if (fbd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    await Task.Run(() =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Elements.Add(new Element()
                                { Name = Path.GetFileName(fbd.FileName), FullName = fbd.FileName });
                            LoadChildElements(Elements);

                            Task.Delay(500).Wait();
                        });
                    });
                }
            });

        public ICommand ClearTree =>
            new DelegateCommand(() =>
            {
                var messageBox = MessageBox.Show("Are you sure about this?", "Warning",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (messageBox == DialogResult.Yes) Elements.Clear();
            });

        public ICommand Settings =>
            new DelegateCommand(() =>
            {
                throw new Exception("Не реализовано");
                ;
            });

        public ICommand Exit => new DelegateCommand(() => Environment.Exit(0));

        public ICommand ShowFile =>
            new DelegateCommand<Element>(file =>
            {
                if (file is null) return;

                var fileExtension = Path.GetExtension(file.FullName);

                var fileExtensions = new Dictionary<string, List<string>>()
                {
                    { "Txt", new List<string>() { ".txt", ".rtf", ".docx", ".py" } },
                    { "Img", new List<string>() { ".jpeg", ".jpg", ".png" } }
                };

                if (fileExtensions["Txt"].Exists(ext => ext == fileExtension))
                {
                    using var sr = new StreamReader(file.FullName, Encoding.Default);
                    var viewModel = new TextViewModel() { CurrentText = sr.ReadToEnd() };
                    var view = new TextUserControl() { DataContext = viewModel };
                    var elem = new TabElement() { Name = file.Name, Content = view };
                    Pages.Add(elem);
                    SelectedTabElement = elem;
                    sr.Dispose();
                    sr.Close();
                }
                else if (fileExtensions["Img"].Exists(ext => ext == fileExtension))
                {
                    var viewModel = new PictureViewModel() { CurrentPicture = file.FullName };
                    var view = new PictureUserControl() { DataContext = viewModel };
                    var elem = new TabElement() { Name = file.Name, Content = view };
                    Pages.Add(elem);
                    SelectedTabElement = elem;
                }
            });

        public ICommand GoToPath =>
            new DelegateCommand<Element>(
                file => Process.Start(new ProcessStartInfo()
                    { FileName = "explorer.exe", Arguments = "/select, " + file.FullName }),
                file => Elements.Count > 0 && file is not null);

        public ICommand DeleteElemFromTree =>
            new DelegateCommand<Element>(file =>
            {
                void SearchFile(ObservableCollection<Element> elements)
                {
                    foreach (var elem in elements)
                    {
                        if (elem.Equals(file))
                        {
                            elements.Remove(file);
                            return;
                        }

                        SearchFile(elem.Children);
                    }
                }

                SearchFile(Elements);
            }, file => Elements.Count > 0 && file is not null);
    }
}