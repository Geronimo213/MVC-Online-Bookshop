using Microsoft.AspNetCore.Http;

namespace Bookshop.Utility
{
    public static class HttpRequestExtensions
    {
        public static Task<string?> GetBaseUrl(this HttpRequest? request)
        {
            if (request == null) return Task.FromResult<string?>(null);

            var uriBuilder = new UriBuilder(request.Scheme, request.Host.Host, request.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }

            return Task.FromResult<string?>(uriBuilder.Uri.AbsoluteUri);

        }
    }
}
