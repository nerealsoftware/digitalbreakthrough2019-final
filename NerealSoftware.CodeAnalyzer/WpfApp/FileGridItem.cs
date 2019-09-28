namespace WpfApp
{
    public class FileGridItem
    { 
        public int Id { get; set; }
        public string FileName { get; set; }
        
        public string Description { get; set; }

        public FileGridItem(int id, string name, string description)
        {
            Id = id;
            FileName = name;
            Description = description;
        }
    }
}
