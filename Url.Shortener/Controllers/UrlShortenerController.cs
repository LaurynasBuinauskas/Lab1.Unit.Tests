using Microsoft.AspNetCore.Mvc;
using Url.Shortener.Models;
using Url.Shortener.Services;

namespace Url.Shortener.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlShortenerController : ControllerBase
    {
        private readonly IUrlShortenerService _urlShortenerService;
                
        public UrlShortenerController(IUrlShortenerService urlShortenerService)
        {
            _urlShortenerService = urlShortenerService;
        }

        [HttpGet("{shortUrl}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]        
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public ActionResult<string> Get(string shortUrl)
        {
            try
            {
                var originalUrl = _urlShortenerService.GetOriginalUrl(shortUrl);

                if (string.IsNullOrEmpty(originalUrl))
                {
                    return NotFound("Not Found");
                }

                return Ok(originalUrl);
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid short URL format");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShortUrlResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public ActionResult Post([FromBody] string originalUrl)
        {
            try
            {
                var shortUrl = _urlShortenerService.Shorten(originalUrl);
                var getUrlPath = Url.Action(nameof(Get), new { shortUrl = shortUrl });

                return Ok(new ShortUrlResult { ShortUrl = shortUrl, GetUrlPath = getUrlPath });
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid URL format");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
