using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using Url.Shortener.Controllers;
using Url.Shortener.Models;
using Url.Shortener.Services;
using Xunit;

namespace Url.Shortener.Tests
{    
    public class UrlShortenerControllerTests
    {
        private readonly UrlShortenerController _controller;
        private readonly Mock<IUrlShortenerService> _mockService;

        private readonly string _validShortUrl = "testShortUrl";        
        private readonly string _originalUrl = "https://test.com";
        private readonly string _invalidUrl = "invalid";


        public UrlShortenerControllerTests()
        {
            _mockService = new Mock<IUrlShortenerService>();
            _controller = new UrlShortenerController(_mockService.Object);
        }

        [Fact]
        public void Get_ValidShortUrl_ReturnsOk()
        {
            // Arrange
            _mockService.Setup(s => s.GetOriginalUrl(_validShortUrl)).Returns(_originalUrl);

            // Act
            var result = _controller.Get(_validShortUrl);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(_originalUrl, okResult.Value);
        }

        [Fact]
        public void Get_InvalidShortUrl_ReturnsBadRequest()
        {
            // Arrange
            _mockService.Setup(s => s.GetOriginalUrl(_invalidUrl)).Throws(new ArgumentException());

            // Act
            var result = _controller.Get(_invalidUrl);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid short URL format", badRequestResult.Value);
        }

        [Fact]
        public void Get_ErrorInProcessing_ReturnsInternalServerError()
        {
            // Arrange
            _mockService.Setup(s => s.GetOriginalUrl(_validShortUrl)).Throws(new Exception());

            // Act
            var result = _controller.Get(_validShortUrl);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("An error occurred while processing your request.", internalServerErrorResult.Value);
        }

        [Fact]
        public void Get_NoSuchShortUrl_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.GetOriginalUrl(_validShortUrl)).Returns(string.Empty);

            // Act
            var result = _controller.Get(_validShortUrl);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Not Found", notFoundResult.Value);
        }

        [Fact]
        public void Post_ValidOriginalUrl_ReturnsOk()
        {
            // Arrange
            _mockService.Setup(s => s.Shorten(It.IsAny<string>())).Returns(_validShortUrl);

            var mockUrlHelper = new Mock<IUrlHelper>(MockBehavior.Strict);
            mockUrlHelper
                .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns("http://localhost/UrlShortener/Get/" + _validShortUrl)
                .Verifiable();

            _controller.Url = mockUrlHelper.Object;

            // Act
            var result = _controller.Post(_originalUrl);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<ShortUrlResult>(okResult.Value);
            Assert.Equal(_validShortUrl, value.ShortUrl);
            Assert.Contains(_validShortUrl, value.GetUrlPath);

            mockUrlHelper.Verify();
        }

        [Fact]
        public void Post_InvalidOriginalUrl_ReturnsBadRequest()
        {
            // Arrange
            _mockService.Setup(s => s.Shorten(_invalidUrl)).Throws(new ArgumentException());

            // Act
            var result = _controller.Post(_invalidUrl);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid URL format", badRequestResult.Value);
        }

        [Fact]
        public void Post_ErrorInProcessing_ReturnsInternalServerError()
        {
            // Arrange
            _mockService.Setup(s => s.Shorten(_originalUrl)).Throws(new Exception());

            // Act
            var result = _controller.Post(_originalUrl);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("An error occurred while processing your request.", internalServerErrorResult.Value);
        }        

        [Fact]
        public void Post_NullOriginalUrl_ReturnsBadRequest()
        {
            // Arrange, Act
            var result = _controller.Post(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid URL format", badRequestResult.Value);
        }
        [Fact]
        public void Post_EmptyStringOriginalUrl_ReturnsBadRequest()
        {
            // Arrange, Act
            var result = _controller.Post("");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid URL format", badRequestResult.Value);
        }
    }
}
