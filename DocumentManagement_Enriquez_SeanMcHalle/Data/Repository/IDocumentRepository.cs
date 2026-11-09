using DocumentManagement_Enriquez_SeanMcHalle.Data.Repository.Model;

namespace DocumentManagement_Enriquez_SeanMcHalle.Data.Repository
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<DocumentDal>> GetAllAsync();
        Task<DocumentDal?> GetByIdAsync(string id);
        Task<bool> UploadAsync(DocumentDal document);
        Task<bool> DeleteByIdAsync(string id);
    }
}
