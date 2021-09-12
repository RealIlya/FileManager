using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using DevExpress.Mvvm;
using FileManager.Models;
using FileManager.Views;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

// TODO доделать отображение всех страниц, оптимизировать код

namespace FileManager.ViewModels
{
    public class MainViewModel : BaseVM
    {
        private MyFile _selectedFile;
        private Page _currentPage;

        public ObservableCollection<MyFile> AvailableFiles { get; set; }
        public ObservableCollection<MyFolder> Elements { get; set; }

        public MyFile SelectedFile
        {
            get => _selectedFile;
            set
            {
                if (_selectedFile == value) return;

                _selectedFile = value;
                OnPropertyChanged("SelectedFile");
            }
        }

        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage == value) return;

                _currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        public MainViewModel()
        {
            AvailableFiles = new ObservableCollection<MyFile>();
            Elements = new ObservableCollection<MyFolder>();
        }

        public ICommand OpenFiles
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    var fbd = new FolderBrowserDialog();

                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        await Task.Run(() =>
                        {
                            Application.Current.Dispatcher.Invoke(delegate
                            {
                                var rootPath = fbd.SelectedPath;
                                // GetAllFiles(rootPath, AvailableFiles);
                                LoadRootElements("String");

                                Task.Delay(500).Wait();
                            });
                        });
                    }
                });
            }
        }

        public ICommand ClearList
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var messageBox = MessageBox.Show("Are you sure about this?", "Warning",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    switch (messageBox)
                    {
                        case DialogResult.Yes:
                            AvailableFiles.Clear();
                            break;
                        case DialogResult.No:
                            break;
                    }
                });
            }
        }

        public ICommand Settings
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    throw new Exception("Не реализовано");
                    ;
                });
            }
        }

        public ICommand Exit
        {
            get { return new DelegateCommand(() => { Environment.Exit(0); }); }
        }

        public ICommand ShowFile
        {
            get
            {
                return new DelegateCommand<MyFile>((file) =>
                {
                    if (file == null) return;

                    var fileExtension = Path.GetExtension(file.FullName);

                    var txtFileExtensions = new List<string>() { ".txt", ".rtf", ".docx" };

                    if (txtFileExtensions.Exists(item => item == fileExtension))
                    {
                        using var sr = new StreamReader(file.FullName, Encoding.Default);
                        TextPage.GetInstance().DataContext = new TextViewModel() { TextFile = sr.ReadToEnd() };
                        CurrentPage = TextPage.GetInstance();
                        sr.Dispose();
                        sr.Close();
                    }
                });
            }
        }

        public ICommand GoToPath
        {
            get
            {
                return new DelegateCommand<MyFile>((file) =>
                {
                    var path = file.FullName;
                    var pathList = path.Split('\\').ToList();
                    pathList.Remove(file.Name);

                    var pathStr = pathList.Aggregate("", (current, str) => current + str + "\\");

                    Process.Start(pathStr);
                }, _ => AvailableFiles.Count > 0);
            }
        }

        public ICommand DeleteFileFromList
        {
            get
            {
                return new DelegateCommand<MyFile>((file) => { AvailableFiles.Remove(file); },
                    _ => AvailableFiles.Count > 0);
            }
        }

        // private void GetAllFiles(string startDirectory, ObservableCollection<MyFile> listOfFiles)
        // {
        //     var searchDirectory = new DirectoryInfo(startDirectory).GetDirectories();
        //
        //     if (searchDirectory.Length > 0)
        //     {
        //         for (int i = 0; i < searchDirectory.Length; i++)
        //         {
        //             GetAllFiles(searchDirectory[i].FullName + @"\", ref listOfFiles);
        //         }
        //     }
        //
        //     var files = new DirectoryInfo(startDirectory).GetFiles();
        //
        //     foreach (FileInfo fileInfo in files)
        //     {
        //         if (fileInfo.Name.Split('.').Length > 1)
        //         {
        //             listOfFiles.Add(new MyFile() { Name = fileInfo.Name, FullName = fileInfo.FullName });
        //         }
        //     }
        // }

        private void LoadRootElements(string rootPath)
        {
            Elements.Add(new MyFolder()
                { Name = "RimWorld", FullName = @"C:\Program Files (x86)\Steam\steamapps\common\RimWorld" });
            
            LoadChildElements(Elements.FirstOrDefault().FullName, Elements);
            
        }

        private void LoadChildElements(string startPath, ObservableCollection<MyFolder> currentDirectory)
        {
            var currentFolder = currentDirectory.LastOrDefault().ChildFolders;

            var searchDirectory = new DirectoryInfo(startPath).GetDirectories();

            if (searchDirectory.Length > 0)
            {
                foreach (var directory in searchDirectory)
                {
                    currentFolder.Add(new MyFolder() { Name = directory.Name, FullName = directory.FullName });
                    LoadChildElements(directory.FullName + @"\", currentFolder);
                }
            }

            var files = new DirectoryInfo(startPath).GetFiles();

            foreach (var file in files)
            {
                currentFolder.Add(new MyFolder() { Name = file.Name, FullName = file.FullName });
            }
        }
    }
}