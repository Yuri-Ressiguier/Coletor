namespace Coletor.Models
{
    public class FileType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public FileType()
        {
        }

        public FileType(string name)
        {
            Name = name;
        }
    }
}
