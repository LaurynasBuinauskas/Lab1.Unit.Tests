using Moq;
using Url.Shortener.Repositories;
using Xunit;

namespace Url.Shortener.Tests
{    
    public class UrlRepositoryTests
    {        
        private readonly UrlRepository _repository;
        private readonly string _validShortUrl = "testShortUrl";
        private readonly string _originalUrl = "https://test.com";

        public UrlRepositoryTests()
        {            
            _repository = new UrlRepository();
        }

        [Fact]
        public void SaveUrl_ValidInput_ShouldSaveUrl()
        {
            // Arrange
            string shortUrl = _validShortUrl;
            string originalUrl = _originalUrl;

            // Act
            _repository.SaveUrl(shortUrl, originalUrl);

            // Assert
            string savedUrl = _repository.GetUrl(shortUrl);
            Assert.Equal(originalUrl, savedUrl);            
        }

        [Fact]
        public void GetUrl_ExistingShortUrl_ShouldReturnOriginalUrl()
        {
            // Arrange
            string shortUrl = _validShortUrl;
            string originalUrl = _originalUrl;
            _repository.SaveUrl(shortUrl, originalUrl);

            // Act
            string retrievedUrl = _repository.GetUrl(shortUrl);

            // Assert
            Assert.Equal(originalUrl, retrievedUrl);
        }

        [Fact]
        public void GetUrl_NonExistingShortUrl_ShouldReturnNull()
        {
            // Arrange
            string shortUrl = "xyz789";

            // Act
            string retrievedUrl = _repository.GetUrl(shortUrl);

            // Assert
            Assert.Null(retrievedUrl);
        }

        [Fact]
        public void SaveUrl_DuplicateShortUrl_ShouldOverwriteExistingUrl()
        {
            // Arrange
            string shortUrl = _validShortUrl;
            string originalUrl1 = _originalUrl;
            string originalUrl2 = "http://test.lt";

            // Act
            _repository.SaveUrl(shortUrl, originalUrl1);
            _repository.SaveUrl(shortUrl, originalUrl2);

            // Assert
            string retrievedUrl = _repository.GetUrl(shortUrl);
            Assert.Equal(originalUrl2, retrievedUrl);
        }        

        [Fact]
        public void GetUrl_EmptyShortUrl_ShouldReturnNull()
        {
            // Arrange
            string shortUrl = "";

            // Act
            string retrievedUrl = _repository.GetUrl(shortUrl);

            // Assert
            Assert.Null(retrievedUrl);
        }

        [Fact]
        public void SaveUrl_EmptyShortUrl_ShouldHandleEmptyValues()
        {
            // Arrange
            string shortUrl = "";
            string originalUrl = _originalUrl;           

            // Act
            _repository.SaveUrl(shortUrl, originalUrl);

            // Assert
            string savedUrl = _repository.GetUrl(shortUrl);
            Assert.Equal(savedUrl, originalUrl);
        }

        [Fact]
        public void SaveUrl_EmptyOriginalUrl_ShouldHandleEmptyValues()
        {
            // Arrange
            string shortUrl = _validShortUrl;
            string originalUrl = "";            

            // Act
            _repository.SaveUrl(shortUrl, originalUrl);

            // Assert
            string savedUrl = _repository.GetUrl(shortUrl);
            Assert.Empty(savedUrl);
        }

        [Fact]
        public void SaveUrl_WithSpecialCharacters_ShouldSaveAndRetrieveUrl()
        {
            // Arrange
            string shortUrl = _validShortUrl;
            string originalUrl = "http://test.com/?param=value&key=123#section";

            // Act
            _repository.SaveUrl(shortUrl, originalUrl);
            string retrievedUrl = _repository.GetUrl(shortUrl);

            // Assert
            Assert.Equal(originalUrl, retrievedUrl);
        }        
    }
}
