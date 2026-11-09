using DocumentManagement_Enriquez_SeanMcHalle.Business.Generics;
using DocumentManagement_Enriquez_SeanMcHalle.Business.Model;
using DocumentManagement_Enriquez_SeanMcHalle.Data.Repository;
using DocumentManagement_Enriquez_SeanMcHalle.Data.Repository.Model;
using Microsoft.AspNetCore.StaticFiles;

namespace DocumentManagement_Enriquez_SeanMcHalle.Business
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public DocumentService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;

            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task<Result<IEnumerable<DocumentDal>>> GetAllAsync()
        {
            try
            {
                var documents = await _documentRepository.GetAllAsync();

                if (documents == null || !documents.Any())
                {
                    return Result<IEnumerable<DocumentDal>>.Failure("No documents were found.");
                }

                return Result<IEnumerable<DocumentDal>>.Success(documents);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<DocumentDal>>.Failure("An error occurred while retrieving documents.");
            }
        }

        public async Task<Result<DocumentDal?>> GetByIdAsync(string id)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(id);

                if (document == null)
                {
                    return Result<DocumentDal?>.Failure("No document found.");
                }

                return Result<DocumentDal?>.Success(document);
            }
            catch (Exception ex)
            {
                return Result<DocumentDal?>.Failure("An unexpected error occurred while retrieving the document.");
            }

        }

        public async Task<Result<DocumentDal>> UploadAsync(DocumentDto request)
        {
            try
            {
                // Validation: Check if request is empty
                if (request.File == null || request.File.Length == 0)
                {
                    return Result<DocumentDal>.Failure("Document must be uploaded.");
                }

                // Validation: Check document size limit
                long sizeLimit = 10 * 1024 * 1024; // 10 MB
                if (request.File.Length > sizeLimit)
                {
                    return Result<DocumentDal>.Failure("Document size must be 10mb.");
                }

                // Validation: Security check if document is supported
                var allowedExtensions = new[] { ".pdf", ".docx", ".txt" };
                var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    return Result<DocumentDal>.Failure("Document type Not Supported: Only PDF, DOCX, and TXT are allowed.");
                }

                // Generating unique identifier for filename
                var newId = Guid.NewGuid();
                var trustedFileName = $"{newId}{extension}";
                var fullPath = Path.Combine(_storagePath, trustedFileName);


                // Mapping request
                var fileNameOnly = request.File.FileName.Split('.');
                var documentMetadata = new DocumentDal
                {
                    OriginalFileName = fileNameOnly[0],
                    StoredIdentifier = newId.ToString(),
                    ContentType = extension.TrimStart('.'),
                    FileSize = request.File.Length
                };

                // Call Repository for storing meta data
                var isUploaded = await _documentRepository.UploadAsync(documentMetadata);

                if (isUploaded)
                {
                    // Move physical file to storage path
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await request.File.CopyToAsync(stream);
                    }
                }

                return Result<DocumentDal>.Success(documentMetadata);
            }
            catch (Exception ex)
            {
                return Result<DocumentDal>.Failure("An unexpected error occurred while uploading the document.");
            }

        }

        public async Task<Result<bool>> DeleteByIdAsync(string id)
        {
            try 
            {
                // Sanitize input to prevent Path Traversal attacks (e.g., passing "../../etc/passwd")
                var safeFileName = Path.GetFileName(id);
                var fullPath = Path.Combine(_storagePath, safeFileName);

                // Fetch the metadata from the database to get the original file name
                var response = await _documentRepository.GetByIdAsync(id);
                var fileExtension = response.ContentType;
                var originalFileName = $"{response.OriginalFileName}.{fileExtension}";
                fullPath = $"{fullPath}.{fileExtension}";

                var isDeleted = await _documentRepository.DeleteByIdAsync(id);

                if (isDeleted)
                {
                    // Delete file if exist
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    else
                    {
                        return Result<bool>.Failure("Document does not exist.");
                    }
                }

                return Result<bool>.Success(isDeleted);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure("An unexpected error occurred while deleting the document.");
            }
        }

        public async Task<Result<DocumentDal>> DownloadAsync(string id)
        {
            try
            {
                // Sanitize input to prevent Path Traversal attacks (e.g., passing "../../etc/passwd")
                var safeFileName = Path.GetFileName(id);
                var fullPath = Path.Combine(_storagePath, safeFileName);

                // Fetch the metadata from the database to get the original file name

                var response = await _documentRepository.GetByIdAsync(id);
                var fileExtension = response.ContentType;
                var originalFileName = $"{response.OriginalFileName}.{fileExtension}";
                fullPath = $"{fullPath}.{fileExtension}";

                // Validation: Verify the file exists on the disk
                if (!System.IO.File.Exists(fullPath))
                {
                    return Result<DocumentDal>.Failure("The requested file does not exist.");
                }

                // Check the correct content type
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(fullPath, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                var documentMetadata = new DocumentDal
                {
                    StoredIdentifier = fullPath,
                    OriginalFileName = originalFileName,
                    ContentType = contentType
                };

                return Result<DocumentDal>.Success(documentMetadata);
            }
            catch (Exception ex)
            {
                return Result<DocumentDal>.Failure("An unexpected error occurred while downloading the document.");
            }
        }
    }
}
