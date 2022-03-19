namespace MarkPad.FileSystem.Data
{
    public class DataItem
    {
        public DataType Type { get; set; }

        public string FullPath { get; set; }

        public string Name
        {
            get
            {
                return Type == DataType.Drive ? FullPath : DirectoryStructure.GetFileOrFolderName(FullPath);
            }
        }
    }
}
