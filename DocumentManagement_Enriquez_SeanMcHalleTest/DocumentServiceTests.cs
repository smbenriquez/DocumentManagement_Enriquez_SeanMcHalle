using DocumentManagement_Enriquez_SeanMcHalle.Business;
using DocumentManagement_Enriquez_SeanMcHalle.Data.Repository.Model;
using DocumentManagement_Enriquez_SeanMcHalle.Data.Repository;
using Moq;
using FluentAssertions;
using DocumentManagement_Enriquez_SeanMcHalle.Business.Model;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace DocumentManagement_Enriquez_SeanMcHalleTest
{
    public class DocumentServiceTests : IDisposable
    {
        private readonly Mock<IDocumentRepository> _mockDocumentRepository;
        private readonly string _tempStoragePath;
        private readonly DocumentService _service;

        public DocumentServiceTests()
        {
            _mockDocumentRepository = new Mock<IDocumentRepository>();
            _tempStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            Directory.CreateDirectory(_tempStoragePath);
            _service = new DocumentService(_mockDocumentRepository.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSuccess_WhenDocumentExists()
        {
            // Arrange
            var targetId = "doc-123";
            var mockDocument = new DocumentDal
            {
                StoredIdentifier = targetId,
                ContentType = "pdf",
                OriginalFileName = "test.pdf",
                FileSize = 18
            };

            _mockDocumentRepository
                .Setup(repo => repo.GetByIdAsync(targetId))
                .ReturnsAsync(mockDocument);

            // Act
            var result = await _service.GetByIdAsync(targetId);

            // Assert
            result.IsFailure.Should().BeFalse();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(mockDocument);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnFailure_WhenDocumentDoesNotExist()
        {
            // Arrange
            var targetId = "non-existent-id";

            _mockDocumentRepository
                .Setup(repo => repo.GetByIdAsync(targetId))
                .ReturnsAsync((DocumentDal?)null);

            // Act
            var result = await _service.GetByIdAsync(targetId);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Value.Should().BeNull();
            result.ErrorMessage.Should().Be("No document found.");
        }

        [Fact]
        public async Task UploadAsync_ShouldReturnFailure_WhenFileSizeExceeds10MB()
        {
            // Arrange
            long invalidSize = (10 * 1024 * 1024) + 1;

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(invalidSize);
            mockFile.Setup(f => f.FileName).Returns("large_document.pdf");

            var request = new DocumentDto { File = mockFile.Object };

            // Act
            var result = await _service.UploadAsync(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.ErrorMessage.Should().Be("Document size must be 10mb.");

            _mockDocumentRepository.Verify(repo => repo.UploadAsync(It.IsAny<DocumentDal>()), Times.Never);
        }

        [Fact]
        public async Task UploadAsync_ShouldReturnSuccess_WhenFileIsValidAndSavedSuccessfully()
        {
            // Arrange
            var fileName = "my_report.pdf";
            var fileContent = "Fake PDF Content content string goes here";
            var bytes = Encoding.UTF8.GetBytes(fileContent);
            long validSize = bytes.Length;

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(validSize);
            mockFile.Setup(f => f.FileName).Returns(fileName);

            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Returns((Stream stream, CancellationToken token) => stream.WriteAsync(bytes, 0, bytes.Length, token));

            var request = new DocumentDto { File = mockFile.Object };

            _mockDocumentRepository
                .Setup(repo => repo.UploadAsync(It.IsAny<DocumentDal>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.UploadAsync(request);

            // Assert
            result.IsFailure.Should().BeFalse();
            result.Value.Should().NotBeNull();

            result.Value.OriginalFileName.Should().Be("my_report");
            result.Value.ContentType.Should().Be("pdf");
            result.Value.FileSize.Should().Be(validSize);

            _mockDocumentRepository.Verify(repo => repo.UploadAsync(It.IsAny<DocumentDal>()), Times.Once);

            var physicalFiles = Directory.GetFiles(_tempStoragePath);
            physicalFiles.Should().HaveCount(1);
            Path.GetExtension(physicalFiles[0]).Should().Be(".pdf");
        }

        [Fact]
        public async Task DeleteByIdAsync_ShouldReturnSuccess_WhenDatabaseAndPhysicalFileAreDeleted()
        {
            // Arrange
            var targetId = "doc-guid-12345";

            var mockMetadata = new DocumentDal
            {
                StoredIdentifier = targetId,
                OriginalFileName = "Invoice_2026",
                ContentType = "pdf"
            };

            _mockDocumentRepository
                .Setup(repo => repo.GetByIdAsync(targetId))
                .ReturnsAsync(mockMetadata);

            _mockDocumentRepository
                .Setup(repo => repo.DeleteByIdAsync(targetId))
                .ReturnsAsync(true);

            var expectedFileName = $"{targetId}.pdf";
            var physicalFilePath = Path.Combine(_tempStoragePath, expectedFileName);

            await File.WriteAllTextAsync(physicalFilePath, "Dummy file payload");

            // Act
            var result = await _service.DeleteByIdAsync(targetId);

            // Assert
            result.IsFailure.Should().BeFalse();
            result.Value.Should().BeTrue();

            _mockDocumentRepository.Verify(repo => repo.GetByIdAsync(targetId), Times.Once);
            _mockDocumentRepository.Verify(repo => repo.DeleteByIdAsync(targetId), Times.Once);

            File.Exists(physicalFilePath).Should().BeFalse();
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempStoragePath))
            {
                Directory.Delete(_tempStoragePath, recursive: true);
            }
        }
    }
}