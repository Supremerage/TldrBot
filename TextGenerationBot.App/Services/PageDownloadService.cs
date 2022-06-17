using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextGenerationBot.App.Services
{
    public class PageDownloadService
    {
        private readonly ILogger<PageDownloadService> _logger;
        private readonly HttpClient _httpClient;

        public PageDownloadService(ILogger<PageDownloadService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<HtmlDocument> DownloadPageAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                await using var content = await response.Content.ReadAsStreamAsync();
                var htmlDoc = new HtmlDocument();
                htmlDoc.Load(content);
                return htmlDoc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download page from {url}", url);
                return null;
            }
        }
    }
}
