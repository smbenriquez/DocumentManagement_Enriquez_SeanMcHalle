using System.ComponentModel.DataAnnotations;

namespace DocumentManagement_Enriquez_SeanMcHalle.Data.Repository.Model
{
    public class DocumentDal
    {
        [Required]
        public string OriginalFileName { get; set; } = string.Empty;
        [Required]
        public string StoredIdentifier { get; set; } = string.Empty;
        [Required]
        public string ContentType { get; set; } = string.Empty;
        [Required]
        public long FileSize { get; set; }
    }
}
