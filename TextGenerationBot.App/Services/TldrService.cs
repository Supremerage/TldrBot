using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextGenerationBot.App.Services
{
    public class TldrService
    {
        private readonly ILogger<TldrService> _logger;
        private readonly PageDownloadService _pageDownloadService;
        private readonly TextGenerationService _textGenerationService;

        public TldrService(ILogger<TldrService> logger, PageDownloadService pageDownloadService, TextGenerationService textGenerationService)
        {
            _logger = logger;
            _pageDownloadService = pageDownloadService;
            _textGenerationService = textGenerationService;
        }

        public async Task<string?> GenerateTldr(string url)
        {
            try
            {
                var htmlDocument = await _pageDownloadService.DownloadPageAsync(url);
                if (htmlDocument is null) return null;
                var article = htmlDocument.DocumentNode.Descendants().FirstOrDefault(x => x.OriginalName == "article");
                if (article is null) return null;

                var articleLines = article.DescendantsAndSelf().OfType<HtmlTextNode>().Select(x => x.InnerText).ToList();
                var articleText = string.Join('\n', articleLines);

                var summary = await _textGenerationService.GenerateSummaryAsync(articleText, 30, 130);
                return summary.SummaryText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate summary for {url}", url);
                return null;
            }
        }

    }
}
