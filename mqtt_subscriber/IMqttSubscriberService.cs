
public interface IMqttSubscriberService
{
    event Action<string, string>? MessageReceived;

    Task ConnectAsync(CancellationToken token);
    void Dispose();
}