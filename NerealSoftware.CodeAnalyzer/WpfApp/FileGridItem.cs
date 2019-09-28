using System.Collections.Generic;

namespace WpfApp
{
    public class FileGridItem
    { 
        public int Id { get; set; }
        public string FileName { get; set; }
        
        public string Report { get; set; }
        public List<string> FileNames { get; set; }

        public FileGridItem(int id, string name, string report, List<string> fileNames)
        {
            Id = id;
            FileName = name;
            Report = report;
            FileNames = fileNames;
        }
    }
}
