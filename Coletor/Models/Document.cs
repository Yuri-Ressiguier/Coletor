using System.ComponentModel.DataAnnotations;

namespace Coletor.Models
{
    public class Document
    {
        public int Id { get; set; }
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Display(Name = "Tamanho (Em Bytes)")]
        public long Size { get; set; }
        [Display(Name = "Caminho do Documento Original")]
        public string Path { get; set; }
        [Display(Name = "Encontrado?")]
        public bool IsHere { get; set; }

        public Document()
        {
        }

        public Document(string name)
        {
            Name = name;
            IsHere = false;
        }
    }
}
