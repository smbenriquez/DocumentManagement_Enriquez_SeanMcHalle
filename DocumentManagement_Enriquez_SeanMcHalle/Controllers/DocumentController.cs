using DocumentManagement_Enriquez_SeanMcHalle.Business;
using DocumentManagement_Enriquez_SeanMcHalle.Business.Model;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagement_Enriquez_SeanMcHalle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _documentService.GetAllAsync();

            if (result.IsFailure)
            {
                if (result.ErrorMessage == "No documents were found.")
                {
                    return NotFound(new { message = result.ErrorMessage });
                }

                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _documentService.GetByIdAsync(id);

            if (result.IsFailure)
            {
                if (result.ErrorMessage == "No document found.")
                {
                    return NotFound(new { message = result.ErrorMessage });
                }

                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(result.Value);
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadDocument(string id)
        {
            var result = await _documentService.DownloadAsync(id);

            if (result.IsFailure)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return PhysicalFile(
                result.Value.StoredIdentifier, 
                result.Value.ContentType, 
                result.Value.OriginalFileName);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument([FromForm] DocumentDto request)
        {
            var result = await _documentService.UploadAsync(request);

            if (result.IsFailure)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(result.Value);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteDocument(string id)
        {
            var result = await _documentService.DeleteByIdAsync(id);

            if (result.IsFailure)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Document deleted successfully!", StoredIdentifier = id });
        }
    }
}
