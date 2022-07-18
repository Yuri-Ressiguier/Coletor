using System.ComponentModel.DataAnnotations;

namespace Coletor.Models.ViewModels
{
    public class SearchFilesViewModel
    {
        public Collector Collector { get; set; }
        [Display(Name = "Extensão")]
        public string Type { get; set; }
        public bool Subfolder { get; set; }
        public ICollection<FileType> FileTypes { get; set; }
        [Display(Name = "Nova Extensão")]
        public string NewFileTypeName { get; set; }

        public SearchFilesViewModel()
        {
        }

        public SearchFilesViewModel(ICollection<FileType> fileTypes)
        {
            FileTypes = fileTypes;
        }
    }
}

