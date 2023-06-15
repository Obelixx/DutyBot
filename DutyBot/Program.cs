using Azure.Identity;

using DutyBot;
using DutyBot.BotActivityHandlers;
using DutyBot.Common;
using DutyBot.Common.Contracts;
using DutyBot.Helpers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Rest;
using Microsoft.TeamsFx.Conversation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient("WebClient", client => client.Timeout = TimeSpan.FromSeconds(600));
builder.Services.AddHttpContextAccessor();

// Prepare Configuration for ConfigurationBotFrameworkAuthentication
builder.Configuration["MicrosoftAppType"] = "MultiTenant";
builder.Configuration["MicrosoftAppId"] = builder.Configuration.GetSection(Configurations.BOT_ID)?.Value;
builder.Configuration[Configurations.BOT_ID] = builder.Configuration.GetSection(Configurations.BOT_ID)?.Value;
builder.Configuration["MicrosoftAppPassword"] = builder.Configuration.GetSection(Configurations.BOT_PASSWORD)?.Value;
builder.Configuration[Configurations.BOT_PASSWORD] = builder.Configuration.GetSection(Configurations.BOT_PASSWORD)?.Value;
builder.Configuration[Configurations.TENANT_ID] = builder.Configuration.GetSection(Configurations.TENANT_ID)?.Value;

// Create the Bot Framework Authentication to be used with the Bot Adapter.
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

builder.Services.AddSingleton<IDutyStorage, DutyStorage>();

// register message handlers
// TODO: use reflection to look in namespace DutyBot.BotActivityHandlers
builder.Services.AddScoped<IMessageActivityHelper, MessageActivityWithDuty>();
builder.Services.AddScoped<IMessageActivityHelper, MessageActivityWithMetionHelper>();
builder.Services.AddScoped<IMessageActivityHelper, MessageActivityWithWhoIs>();

// Create the Cloud Adapter with error handling enabled.
// Note: some classes expect a BotAdapter and some expect a BotFrameworkHttpAdapter, so
// register the same adapter instance for all types.
builder.Services.AddSingleton<CloudAdapter, AdapterWithErrorHandler>();
builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp => sp.GetService<CloudAdapter>());
builder.Services.AddSingleton<BotAdapter>(sp => sp.GetService<CloudAdapter>());


// Create the Conversation with notification feature enabled.
builder.Services.AddSingleton(sp =>
{
    var options = new ConversationOptions()
    {
        Adapter = sp.GetService<CloudAdapter>(),
        Notification = new NotificationOptions
        {
            BotAppId = builder.Configuration["MicrosoftAppId"],
        },
    };

    return new ConversationBot(options);
});

builder.Services.AddSingleton<ClientSecretCredential>(serviceProvider => 
    MSGraphClientHelper.GetMicrosoftGraphClientCredential(
        builder.Configuration[Configurations.TENANT_ID],
        builder.Configuration[Configurations.BOT_ID],
        builder.Configuration[Configurations.BOT_PASSWORD]));
builder.Services.AddTransient<GraphServiceClient>(serviceProvider =>
    MSGraphClientHelper.GetMicrosoftGraphClient(serviceProvider.GetService<ClientSecretCredential>()));
builder.Services.AddTransient<IMSGraphHelper, MSGraphHelper>();

// Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
builder.Services.AddTransient<IBot, TeamsBot>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
