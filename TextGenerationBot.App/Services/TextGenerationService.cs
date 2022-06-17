using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TextGenerator.Core.Models;

namespace TextGenerationBot.App.Services
{
    public class TextGenerationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ApiOptions _apiOptions;

        public TextGenerationService(HttpClient httpClient, IConfiguration configuration, IOptions<ApiOptions> options)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiOptions = options.Value;
        }

        public async Task<TextGeneratorResponse> GenerateTextAsync(string prompt, int minLength, int maxLength, float temperature = 1f, bool doSample = true)
        {
            if (_apiOptions.TextGenerationApiUrl is null) throw new Exception("Text Generation API is not configured");

            var body = new
            {
                text = prompt,
                temperature = temperature,
                min_length = minLength,
                max_length = maxLength,
                do_sample = doSample
            };

            using var requestContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(_apiOptions.TextGenerationApiUrl, requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await JsonSerializer.DeserializeAsync<IEnumerable<TextGeneratorResponse>>(await response.Content.ReadAsStreamAsync());

            return responseContent.First();
        }

        public async Task<AutoSummaryResponse> GenerateSummaryAsync(string prompt, int minLength, int maxLength)
        {
            if (_apiOptions.SummaryApiUrl is null) throw new Exception("Summary API is not configured");

            var body = new
            {
                text = prompt,
            };

            var uri = new Uri(_apiOptions.SummaryApiUrl, $"?minLength={minLength}&maxLength={maxLength}");
            using var requestContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(uri, requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await JsonSerializer.DeserializeAsync<IEnumerable<AutoSummaryResponse>>(await response.Content.ReadAsStreamAsync());

            return responseContent.First();
        }
    }
}
