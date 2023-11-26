
using AzureSignalR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
builder.Services.AddSingleton(config);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(x => 
    x.AddDefaultPolicy(p=>
        p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
    )
);

builder.Services.AddSignalR().AddAzureSignalR(options =>
{
    options.ConnectionString = config.AzureSignalREndpoint;
});

builder.Services
    .AddSingleton<SignalRService>()
    .AddHostedService(sp => sp.GetService<SignalRService>())
    .AddSingleton<IHubContextStore>(sp => sp.GetService<SignalRService>());

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/login", async ([FromServices]IHubContextStore store) =>
{
    var negotiateResponse = await store.ChatHubContext.NegotiateAsync();
    return negotiateResponse;
});

app.UseAzureSignalR(routes =>
{
    routes.MapHub<Chat>("/chat");
});

app.Run();
