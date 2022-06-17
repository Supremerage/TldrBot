using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TextGenerationBot.App.Handlers
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _services;
        private readonly ILogger<InteractionHandler> _logger;
        private readonly ModalHandler _modalHandler;

        public InteractionHandler(DiscordSocketClient discordClient, InteractionService interactionService, IServiceProvider services, ILogger<InteractionHandler> logger, ModalHandler modalHandler)
        {
            _discordClient = discordClient;
            _interactionService = interactionService;
            _services = services;
            _logger = logger;
            _modalHandler = modalHandler;
        }

        public async Task InitializeAsync()
        {
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _discordClient.InteractionCreated += HandleInteraction;

            _discordClient.ModalSubmitted += _modalHandler.HandleModalAsync;
        }

        public async Task RegisterCommandsAsync()
        {
            await _interactionService.RegisterCommandsToGuildAsync(651272290385788929, true);
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                var ctx = new SocketInteractionContext(_discordClient, interaction);
                await _interactionService.ExecuteCommandAsync(ctx, _services);
            }
             catch (Exception ex)
            {
                _logger.LogError(ex, "An error ocurred while executing interaction");
            }
        }
    }
}
