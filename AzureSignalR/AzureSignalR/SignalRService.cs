using AzureSignalR;
using Microsoft.Azure.SignalR.Management;

public class SignalRService(AppSettings configuration, ILoggerFactory loggerFactory) : IHostedService, IHubContextStore
{
    private const string ChatHub = "Chat";
    public ServiceHubContext ChatHubContext { get; private set; }

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        using var serviceManager = new ServiceManagerBuilder()
            .WithOptions(o=>o.ConnectionString = configuration.AzureSignalREndpoint)
            .WithLoggerFactory(loggerFactory)
            .BuildServiceManager();
        ChatHubContext = await serviceManager.CreateHubContextAsync(ChatHub, cancellationToken);
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAll(Dispose(ChatHubContext));
    }

    private static Task Dispose(ServiceHubContext hubContext)
    {
        if (hubContext == null)
        {
            return Task.CompletedTask;
        }
        return hubContext.DisposeAsync();
    }
}