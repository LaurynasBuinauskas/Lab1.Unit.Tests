namespace Url.Shortener.Repositories
{
    public interface IUrlRepository
    {
        void SaveUrl(string shortUrl, string originalUrl);
        string GetUrl(string shortUrl);
    }

    public class UrlRepository : IUrlRepository
    {
        private readonly Dictionary<string, string> _urls = new Dictionary<string, string>();

        public void SaveUrl(string shortUrl, string originalUrl)
        {
            _urls[shortUrl] = originalUrl;
        }

        public string GetUrl(string shortUrl)
        {
            return _urls.TryGetValue(shortUrl, out var originalUrl) ? originalUrl : null;
        }
    }
}

