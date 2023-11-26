using Microsoft.AspNetCore.SignalR;

public class Chat : Hub
{
    public void Echo(string name, string message)
    {
        Clients.Client(Context.ConnectionId).SendAsync("echo", name, message + " (echo from server)");
    }
}