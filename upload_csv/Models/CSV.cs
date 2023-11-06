namespace upload_csv.Models
{
    public class CSV
    {

        public int Id { get; set; } 
        public string Name { get; set; }
        public string Type { get; set; }
        public string Searchable { get; set; }
        public string LibraryFilter { get; set; }
        public string Visible { get; set; }
    }
}
