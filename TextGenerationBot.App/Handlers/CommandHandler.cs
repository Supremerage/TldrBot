using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextGenerationBot.App.Modules;
using TextGenerationBot.App.Services;
using RunMode = Discord.Interactions.RunMode;

namespace TextGenerationBot.App.Handlers
{
    internal class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly Regex _urlRegex;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly ILogger<CommandHandler> _logger;

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services, ILogger<CommandHandler> logger)
        {
            _commands = commands;
            _services = services;
            _logger = logger;
            _client = client;
            _urlRegex = new Regex(@"(?<scheme>https?)://(?<baseUrl>\w+(\.\w+)*)(?<fragment>(/[\w-\.]*)*(?<query>(\?[^\s^\?^\=^&]+=[^\s^\?^\=^&]+)(&[^\s^\?^\=^&]+=[^\s^\?^\=^&]+)*)?)?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;

            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            if (_urlRegex.IsMatch(message.Content))
            {
                await using var cmdScope = _services.CreateAsyncScope();
                var urlMatch = _urlRegex.Match(message.Content);
                var tldrService = cmdScope.ServiceProvider.GetService<TldrService>();
                if (tldrService is { })
                {
                    var tldr = await tldrService.GenerateTldr(urlMatch.Value);
                    await message.ReplyAsync(tldr);
                }
            }

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix('!', ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _services);
        }
    }
}
