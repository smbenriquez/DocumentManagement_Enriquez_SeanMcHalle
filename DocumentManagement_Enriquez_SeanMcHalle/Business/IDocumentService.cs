using DocumentManagement_Enriquez_SeanMcHalle.Business.Generics;
using DocumentManagement_Enriquez_SeanMcHalle.Business.Model;
using DocumentManagement_Enriquez_SeanMcHalle.Data.Repository.Model;

namespace DocumentManagement_Enriquez_SeanMcHalle.Business
{
    public interface IDocumentService
    {
        Task<Result<IEnumerable<DocumentDal>>> GetAllAsync();
        Task<Result<DocumentDal?>> GetByIdAsync(string id);
        Task<Result<DocumentDal>> DownloadAsync(string id);
        Task<Result<DocumentDal>> UploadAsync(DocumentDto document);
        Task<Result<bool>> DeleteByIdAsync(string id);
    }
}
