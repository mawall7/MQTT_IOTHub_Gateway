
namespace IoTHubHandler
{
    public interface IIoT_HubHandler
    {
        Task ConnectAsync();
        Task SendMessage(string jsonMessage);
    }
}