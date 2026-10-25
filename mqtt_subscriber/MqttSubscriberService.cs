
using MQTTnet;
using System.ComponentModel;
using System.Text;

public class MqttSubscriberService :  IMqttSubscriberService
{
    private readonly IMqttClient _client;
    private readonly string _brokerAddress;
    private readonly int _brokerPort;
    private readonly string _topic;

    public event Action<string, string>? MessageReceived;

    public MqttSubscriberService(
        string brokerAddress,
        int brokerPort,
        string topic)
    {
        _brokerAddress = brokerAddress;
        _brokerPort = brokerPort;
        _topic = topic;

        var factory = new MqttClientFactory();
        _client = factory.CreateMqttClient();

        _client.ApplicationMessageReceivedAsync += e =>
        {
            string topic = e.ApplicationMessage.Topic;
            string payload = Encoding.UTF8.GetString(
               e.ApplicationMessage.Payload);
            
            
            MessageReceived?.Invoke(payload, topic);

            return Task.CompletedTask;
        };
    }

    public async Task ConnectAsync(CancellationToken ct)
    {
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(_brokerAddress, _brokerPort)
            .WithClientId($"Client-{Guid.NewGuid():N}")
            .Build();

        await _client.ConnectAsync(
            options, CancellationToken.None);

        var factory = new MqttClientFactory();

        var subscription = factory
            .CreateSubscribeOptionsBuilder()
            .WithTopicFilter(f => f.WithTopic(_topic))
            .Build();

        await _client.SubscribeAsync(
            subscription, CancellationToken.None);
    }

    public void Dispose()
    {
        _client.DisconnectAsync();
    }
}
