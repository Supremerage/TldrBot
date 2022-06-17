using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextGenerationBot.App
{
    public class ApiOptions
    {
        public const string Api = "Api";
        [RegularExpression(
            @"^https?://\w+(\.?[\w-]+)*(:\d+)?(/[\w+-\.]*)*(\?[\w-\.]+(=[\w-\.]+)?(\&[\w-\.]+(=[\w-\.]+)?)*)?$", 
            ErrorMessage = $"The {nameof(SummaryApiUrl)} field is not a valid URL")]
        public Uri? SummaryApiUrl { get; set; }
        [RegularExpression(
            @"^https?://\w+(\.?[\w-]+)*(:\d+)?(/[\w+-\.]*)*(\?[\w-\.]+(=[\w-\.]+)?(\&[\w-\.]+(=[\w-\.]+)?)*)?$",
            ErrorMessage = $"The {nameof(TextGenerationApiUrl)} field is not a valid URL")]
        public Uri? TextGenerationApiUrl { get; set; }
        [RegularExpression(
            @"^https?://\w+(\.?[\w-]+)*(:\d+)?(/[\w+-\.]*)*(\?[\w-\.]+(=[\w-\.]+)?(\&[\w-\.]+(=[\w-\.]+)?)*)?$",
            ErrorMessage = $"The {nameof(SentimentApiUrl)} field is not a valid URL")]
        public Uri? SentimentApiUrl { get; set; }
    }
}
