using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coletor.Models
{
    public class Collector
    {
        public int Id { get; set; }
        [Display(Name = "Data da Consulta")]
        public DateTime Date { get; set; } = DateTime.Now;
        [Display(Name = "Número de Documentos Encontrados")]
        public int NumDocFound { get; set; }
        [Display(Name = "Caminho de Entrada")]
        [Required(ErrorMessage = "É inserir um caminho de entrada")]
        public string InputPath { get; set; }
        [Display(Name = "Caminho de Saída")]
        [Required(ErrorMessage = "É inserir um caminho de saída")]
        public string OutputPath { get; set; }
        [NotMapped]
        [Display(Name = "Lista dos Arquivos (Opcional)")]
        public IFormFile? InternFile { get; set; }
        [Display(Name = "Caminho do Arquivo .txt")]
        public string? InternFilePath { get; set; }
        [Display(Name = "Nome do Arquivo .txt")]
        public string? InternFileName { get; set; }
        public List<Document> Documents { get; set; } = new List<Document>();


        public Collector()
        {
        }

        public void AddDocs(Document doc)
        {
            Documents.Add(doc);
        }




    }
}
