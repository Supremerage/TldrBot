

using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TextGenerationBot.App;
using TextGenerationBot.App.Handlers;
using TextGenerationBot.App.Services;
using RunMode = Discord.Interactions.RunMode;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureAppConfiguration(options => ConfigureAppConfiguration(options));
builder.ConfigureLogging((ctx,logging) => ConfigureLogging(logging, ctx));
builder.ConfigureServices(async (ctx,services) => await ConfigureServices(services, ctx));

var app = builder.Build();
await app.RunAsync();

void ConfigureAppConfiguration(IConfigurationBuilder configurationBuilder)
{
    configurationBuilder.AddJsonFile("Config/appsettings.json", true, true);
    configurationBuilder.AddJsonFile("Config/appsettings.local.json", true, true);
    configurationBuilder.AddEnvironmentVariables("BOT_");
    configurationBuilder.AddKeyPerFile("/run/secrets/", true, true);
    configurationBuilder.AddCommandLine(args);
}



void ConfigureLogging(ILoggingBuilder logging, HostBuilderContext context)
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddConfiguration(context.Configuration);
}



async Task ConfigureServices(IServiceCollection services, HostBuilderContext context)
{
    services.AddOptions<ApiOptions>().Bind(context.Configuration.GetSection(ApiOptions.Api)).ValidateDataAnnotations().ValidateOnStart();

    var discordConfig = new DiscordSocketConfig()
    {
        GatewayIntents = Discord.GatewayIntents.GuildMessages | Discord.GatewayIntents.AllUnprivileged
    };
    var discordClient = new DiscordSocketClient(discordConfig);
    var interactionServiceConfig = new InteractionServiceConfig
    {
        UseCompiledLambda = true,
        DefaultRunMode = RunMode.Async
    };
    var interactionService = new InteractionService(discordClient.Rest, interactionServiceConfig);
    services.AddSingleton<DiscordSocketClient>(discordClient);
    services.AddSingleton<InteractionService>(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
    services.AddSingleton<InteractionHandler>();
    services.AddSingleton<CommandService>();
    services.AddSingleton<CommandHandler>();
    services.AddSingleton<ModalHandler>();
    services.AddHttpClient();
    services.AddScoped<TextGenerationService>();
    services.AddScoped<TldrService>();
    services.AddScoped<PageDownloadService>();

    services.AddHostedService<BotHostedService>();
}