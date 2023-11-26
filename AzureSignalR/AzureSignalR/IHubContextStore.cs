using Microsoft.Azure.SignalR.Management;

public interface IHubContextStore
{
    public ServiceHubContext ChatHubContext { get; }
}