using MarkPad.FileSystem;
using MarkPad.FileSystem.Data;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MarkPad.ViewModels
{
    public class DataItemViewModel : BaseViewModel
    {
        public DataType Type { get; set; }

        public string FullPath { get; set; }

        public string Name { get { return Type == DataType.Drive ? FullPath : DirectoryStructure.GetFileOrFolderName(FullPath); } }

        public ObservableCollection<DataItemViewModel> Children { get; set; }

        public ICommand ExpandCommand { get; set; }

        public bool CanExpand { get
            {
                return Type != DataType.File;
            }
        }

        public bool IsExpanded {
            get
            {
                return Children?.Count(f => f != null) > 0;
            }
            set
            {
                if (value == true)
                {
                    Expand();

                    if (Type == DataType.FolderClosed)
                    {
                        Type = DataType.FolderOpened;
                    }
                }
                else
                {
                    ClearChildren();

                    if (Type != DataType.Drive)
                    {
                        Type = DataType.FolderClosed;
                    }
                }
            }
        }

        public DataItemViewModel(string fullPath, DataType type)
        {
            Type = type;
            FullPath = fullPath;
            ExpandCommand = new RelayCommand(Expand);

            ClearChildren();
        }

        private void ClearChildren()
        {
            Children = new ObservableCollection<DataItemViewModel>();

            if (Type != DataType.File)
            {
                Children.Add(null);
            }
            else
            {
                Children = new ObservableCollection<DataItemViewModel>();
            }
        }

        private void Expand()
        {
            if (Type == DataType.File)
            {
                return;
            }

            // I want to display the drives VolumeLabel, due to the way the Model/ViewModels are setup and this is just practice...
            var children = DirectoryStructure.GetDirectoryContents(Type == DataType.Drive ? FullPath.Substring(FullPath.Count() - 4, 3) : FullPath);

            Children = new ObservableCollection<DataItemViewModel>(children.Select(content => new DataItemViewModel(content.FullPath, content.Type)));
        }
    }
}
