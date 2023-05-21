using Moq;
using Xunit;
using Url.Shortener.Repositories;
using Url.Shortener.Services;

namespace Url.Shortener.Tests
{
    public class UrlShortenerServiceTests
    {      
        private readonly Mock<IUrlRepository> _repositoryMock;
        private readonly IUrlShortenerService _service;

        private const string _testOriginalUrl = "https://test.com";
        private const string _testEmptyUrl = "";
        private const string _testNonExistentShortUrl = "YHqCyIBHN0a7L8jYJif3Bw";

        public UrlShortenerServiceTests()
        {            
            _repositoryMock = new Mock<IUrlRepository>();
            _service = new UrlShortenerService(_repositoryMock.Object);
        }

        [Theory]
        [InlineData("http://test.com")]
        [InlineData(_testOriginalUrl)]
        public void Shorten_ValidUrl_ShouldSaveUrlInRepository(string originalUrl)
        {
            // Arrange          
            _repositoryMock.Setup(r => r.SaveUrl(It.IsAny<string>(), originalUrl)).Verifiable();

            // Act
            string result = _service.Shorten(originalUrl);

            // Assert
            Assert.NotNull(result);
            _repositoryMock.Verify(r => r.SaveUrl(result, originalUrl), Times.Once);
        }

        [Fact]
        public void Shorten_ValidUrl_ShouldReturnValidShortenedUrl()
        {
            // Arrange
            string originalUrl = "https://testTest.com";

            // Act
            string result = _service.Shorten(originalUrl);

            // Assert
            Assert.NotNull(result);            
            Assert.True(IsValidHash(result));
        }

        [Fact]
        public void Shorten_InvalidUrl_ShouldThrowArgumentException()
        {
            // Arrange
            string invalidUrl = "invalid-url";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Shorten(invalidUrl));
        }


        [Fact]
        public void Shorten_DuplicateOriginalUrl_ShouldReturnDifferentShortUrls()
        {          
            // Act
            string shortUrl1 = _service.Shorten(_testOriginalUrl);
            string shortUrl2 = _service.Shorten(_testOriginalUrl);

            // Assert
            Assert.NotEqual(shortUrl1, shortUrl2);
        }

        [Fact]
        public void Shorten_EmptyUrl_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.Shorten(_testEmptyUrl));
        }        

        [Fact]
        public void Shorten_ValidUrl_ShouldSaveUniqueShortUrls()
        {
            // Act
            string shortUrl1 = _service.Shorten(_testOriginalUrl);
            string shortUrl2 = _service.Shorten(_testOriginalUrl);

            // Assert
            Assert.NotEqual(shortUrl1, shortUrl2);
            _repositoryMock.Verify(r => r.SaveUrl(It.IsAny<string>(), _testOriginalUrl), Times.Exactly(2));
        }

        [Fact]
        public void Shorten_RepositoryThrowsException_ShouldThrowException()
        {
            // Arrange            
            _repositoryMock.Setup(r => r.SaveUrl(It.IsAny<string>(), _testOriginalUrl)).Throws<Exception>();

            // Act & Assert
            Assert.Throws<Exception>(() => _service.Shorten(_testOriginalUrl));
        }

        [Fact]
        public void GetOriginalUrl_ValidShortUrlWithPadding_ShouldReturnOriginalUrl()
        {
            // Arrange            
            string shortUrl = _service.Shorten(_testOriginalUrl);
            string paddedShortUrl = shortUrl.PadRight(22, '=');
            _repositoryMock.Setup(r => r.GetUrl(paddedShortUrl)).Returns(_testOriginalUrl);

            // Act
            string result = _service.GetOriginalUrl(paddedShortUrl);

            // Assert
            Assert.Equal(_testOriginalUrl, result);
        }

        [Fact]
        public void GetOriginalUrl_NonExistentShortUrl_ShouldReturnNull()
        {
            // Arrange            
            _repositoryMock.Setup(r => r.GetUrl(_testNonExistentShortUrl)).Returns((string)null);

            // Act
            string result = _service.GetOriginalUrl(_testNonExistentShortUrl);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetOriginalUrl_ValidShortUrl_ShouldReturnOriginalUrl()
        {
            // Arrange            
            string shortUrl = _service.Shorten(_testOriginalUrl);
            _repositoryMock.Setup(r => r.GetUrl(shortUrl)).Returns(_testOriginalUrl);

            // Act
            string result = _service.GetOriginalUrl(shortUrl);

            // Assert
            Assert.Equal(_testOriginalUrl, result);
        }

        [Fact]
        public void GetOriginalUrl_InvalidShortUrl_ShouldThrowArgumentException()
        {
            // Arrange
            string invalidShortUrl = "invalid-short-url";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.GetOriginalUrl(invalidShortUrl));
        }

        [Fact]
        public void GetOriginalUrl_EmptyShortUrl_ShouldThrowArgumentException()
        {            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.GetOriginalUrl(_testEmptyUrl));
        }

        [Fact]
        public void GetOriginalUrl_NullShortUrl_ShouldThrowArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.GetOriginalUrl(null));
        }       

        [Fact]
        public void GetOriginalUrl_RepositoryThrowsException_ShouldThrowException()
        {
            // Arrange            
            _repositoryMock.Setup(r => r.GetUrl(_testNonExistentShortUrl)).Throws<Exception>();

            // Act & Assert
            Assert.Throws<Exception>(() => _service.GetOriginalUrl(_testNonExistentShortUrl));
        }        

        private bool IsValidHash(string hash)
        {                                  
            // Check if the hash only contains alphanumeric characters and dashes
            foreach (char c in hash)
            {
                if (!char.IsLetterOrDigit(c) && c != '-')
                {
                    return false;
                }
            }

            // Check if the hash has the required length
            if (hash.Length != 22)
            {
                return false;
            }            

            return true;
        }
    }
}
