﻿using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using MetaculusDiscord.Data;
using MetaculusDiscord.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MetaculusDiscord;

public class Program
{
    public static Task Main(string[] args) => MainAsync();

    /// <summary>
    /// Start the bot using Discord.Net library
    /// </summary>
    private static async Task MainAsync()
    {
        var builder = new HostBuilder().ConfigureAppConfiguration(a =>
        {
            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true).Build();
            a.AddConfiguration(config);
        }).ConfigureLogging(x =>
        {
            x.AddConsole();
            x.SetMinimumLevel(LogLevel.Debug);
        }).ConfigureDiscordHost((context, config) =>
        {
            config.SocketConfig = new DiscordSocketConfig
                {LogLevel = LogSeverity.Debug, AlwaysDownloadUsers = false, MessageCacheSize = 200};
            config.Token = context.Configuration["Token"];
        }).UseCommandService((context, config) =>
        {
            config.CaseSensitiveCommands = false;
            config.LogLevel = LogSeverity.Debug;
            config.DefaultRunMode = RunMode.Async;
        }).UseInteractionService((context, config) =>
        {
            config.LogLevel = LogSeverity.Debug;
        }
    )
    .ConfigureServices((context, services) =>
            {
                services.AddHostedService<CommandHandler>()
                    .AddSingleton<Data.Data>(); // injecting class Data into commandHandler
                
            })
            .UseConsoleLifetime();
        var host = builder.Build();
        using (host)
        {
            await host.RunAsync();
        }

    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return null;
    }
}