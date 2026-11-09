using Dapper;
using DocumentManagement_Enriquez_SeanMcHalle.Data.DbConnection;
using DocumentManagement_Enriquez_SeanMcHalle.Data.Repository.Model;
using System.Data;

namespace DocumentManagement_Enriquez_SeanMcHalle.Data.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DocumentDal>> GetAllAsync()
        {
            var query = "SELECT * FROM Details";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<DocumentDal>(query);
        }

        public async Task<DocumentDal?> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();

            return await connection.QuerySingleOrDefaultAsync<DocumentDal>(
            "sp_GetDocumentById",
            new { StoredIdentifier = id },
            commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UploadAsync(DocumentDal document)
        {
            var parameters = new
            {
                OriginalFileName = document.OriginalFileName,
                StoredIdentifier = document.StoredIdentifier,
                ContentType = document.ContentType,
                FileSize = document.FileSize
            };

            using var connection = _context.CreateConnection();

            var rowAffected = await connection.ExecuteScalarAsync<int>(
            "sp_UploadDocument",
            parameters,
            commandType: CommandType.StoredProcedure);

            return rowAffected > 0;
        }

        public async Task<bool> DeleteByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();

            var rowAffected = await connection.ExecuteScalarAsync<int>(
            "sp_DeleteDocumentById",
            new { StoredIdentifier = id },
            commandType: CommandType.StoredProcedure);

            return rowAffected > 0;
        }
    }
}
