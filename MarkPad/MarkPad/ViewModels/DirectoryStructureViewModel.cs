using MarkPad.FileSystem;
using MarkPad.FileSystem.Data;
using System.Collections.ObjectModel;
using System.Linq;

namespace MarkPad.ViewModels
{
    class DirectoryStructureViewModel : BaseViewModel
    {
        public ObservableCollection<DataItemViewModel> Items { get; set; }

        public DirectoryStructureViewModel()
        {
            var children = DirectoryStructure.GetLogicalDrives();

            Items = new ObservableCollection<DataItemViewModel>(children.Select(drive => new DataItemViewModel(drive.FullPath, DataType.Drive)));
        }
    }
}
