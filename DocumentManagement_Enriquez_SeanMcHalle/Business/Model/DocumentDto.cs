using System.ComponentModel.DataAnnotations;

namespace DocumentManagement_Enriquez_SeanMcHalle.Business.Model
{
    public class DocumentDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
