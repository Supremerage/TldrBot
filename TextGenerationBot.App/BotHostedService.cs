using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TextGenerationBot.App.Handlers;

namespace TextGenerationBot.App;
internal class BotHostedService : IHostedService
{
    private readonly ILogger<BotHostedService> _logger;
    private readonly IConfiguration _configuration;
    private readonly DiscordSocketClient _discordClient;
    private readonly CommandHandler _commandHandler;
    private readonly InteractionHandler _interactionHandler;

    public BotHostedService(ILogger<BotHostedService> logger, IConfiguration configuration, IHostApplicationLifetime appLifetime, DiscordSocketClient discordClient, CommandHandler commandHandler, InteractionHandler interactionHandler)
    {
        _logger = logger;
        _configuration = configuration;
        _discordClient = discordClient;
        _commandHandler = commandHandler;
        _interactionHandler = interactionHandler;
        appLifetime.ApplicationStarted.Register(OnStarted);
        appLifetime.ApplicationStopping.Register(OnStopping);
        appLifetime.ApplicationStopped.Register(async () => await OnStopped());
    }

    private async Task OnStopped()
    {
        await _discordClient.DisposeAsync();
        _logger.LogInformation("Bot Stopped");
    }

    private void OnStopping()
    {
        _logger.LogInformation("Bot Stopping...");
    }

    private void OnStarted()
    {
        _logger.LogInformation("Bot Started");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Bot Starting...");
        var token = _configuration["Discord:Token"];
        if (string.IsNullOrWhiteSpace(token)) throw new Exception("Discord Token must be configured");

        await _commandHandler.InstallCommandsAsync();
        await _interactionHandler.InitializeAsync();

        _discordClient.Ready += async () =>
        {
            _logger.LogInformation("Installing commands...");
            await _interactionHandler.RegisterCommandsAsync();
            _logger.LogInformation($"Client ready and logged in as: {_discordClient.CurrentUser.Username}#{_discordClient.CurrentUser.Discriminator}");
        };

        await _discordClient.LoginAsync(Discord.TokenType.Bot, token);
        await _discordClient.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discordClient.LogoutAsync();
    }
}

