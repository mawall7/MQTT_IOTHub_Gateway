using IoTHubHandler;
using System.Text.Json;

namespace MqttHubGateway;

public class Gateway
{
    private readonly IMqttSubscriberService _mqttService;
    private readonly IoT_HubHandler _hubHandler;
    private const string MyGatewayId = "csharp-gateway";

    public Gateway(IMqttSubscriberService mqttService, IoT_HubHandler hub)
    {

        _hubHandler = hub;
        _mqttService = mqttService;
        _mqttService.MessageReceived += HandleMqttMessage;

    }

    public async void HandleMqttMessage(string payload, string topic)
    {
        try
        {

            var incomingData = JsonSerializer.Deserialize<TelemetryMessageDto>(payload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (incomingData != null)
            {
                TelemetryMessageDto gatewaytelemetry = incomingData with { GatewayId = MyGatewayId };

                string jsonToSend = JsonSerializer.Serialize(gatewaytelemetry);

                var result = _hubHandler.SendMessage(jsonToSend);

                Console.WriteLine("Forwarding message to IoT-Hub..");

                await result;

                Console.WriteLine("Message was sent to Hub!");


            }


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fel vid vidarebefordran av meddelande till Azure: {ex.Message}");


        }


    }

}