using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextGenerationBot.App.Services;

namespace TextGenerationBot.App.Handlers
{
    public class ModalHandler
    {
        private readonly ILogger<ModalHandler> _logger;
        private readonly TextGenerationService _textGenerationService;

        public ModalHandler(ILogger<ModalHandler> logger, TextGenerationService textGenerationService)
        {
            _logger = logger;
            _textGenerationService = textGenerationService;
        }

        public async Task HandleModalAsync(SocketModal modal)
        {
            try
            {
                var components = modal.Data.Components.ToList();
                switch (modal.Data.CustomId)
                {
                    case "text_generator":
                        var textPrompt = components.Single(x => x.CustomId == "prompt");
                        await modal.RespondAsync($"Your prompt was: {textPrompt.Value}");
                        var textGenerationResult = await _textGenerationService.GenerateTextAsync(textPrompt.Value, 100, 500);
                        await modal.FollowupAsync(textGenerationResult.GeneratedText);
                        break;
                    case "fake_news":
                        var newsPrompt = components.Single(x => x.CustomId == "prompt");
                        await modal.RespondAsync($"Your prompt was: {newsPrompt.Value}");
                        var fakeNewsResult = await _textGenerationService.GenerateSummaryAsync(newsPrompt.Value, 100,200);
                        await modal.FollowupAsync(fakeNewsResult.SummaryText);
                        break;
                    default:
                        await modal.RespondAsync();
                        break;
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while responding to modal with id: {modalId}", modal.Data.CustomId);
            }
        }
    }
}
