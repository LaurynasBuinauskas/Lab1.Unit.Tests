using Url.Shortener.Repositories;

namespace Url.Shortener.Services
{
    public interface IUrlShortenerService
    {
        string Shorten(string originalUrl);
        string GetOriginalUrl(string shortUrl);
    }

    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly IUrlRepository _repository;
        
        public UrlShortenerService(IUrlRepository repository)
        {
            _repository = repository;
        }

        public string Shorten(string originalUrl)
        {
            try
            {
                if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out var uriResult)
                    || !Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute)
                    || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                {
                    throw new ArgumentException("Invalid URL format", nameof(originalUrl));
                }

                var shortUrl = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                shortUrl = shortUrl.Replace("/", "_").Replace("+", "-").TrimEnd('=');

                _repository.SaveUrl(shortUrl, originalUrl);

                return shortUrl;
            }
            catch (Exception)
            {                
                throw;
            }
        }

        public string GetOriginalUrl(string shortUrl)
        {
            if (string.IsNullOrEmpty(shortUrl) || shortUrl.Length != 22)
            {
                throw new ArgumentException("Invalid short URL format", nameof(shortUrl));
            }

            try
            {
                var paddedShortUrl = shortUrl.PadRight(22, '=');
                return _repository.GetUrl(paddedShortUrl);
            }
            catch (Exception)
            {                
                throw;
            }
        }
    }
}