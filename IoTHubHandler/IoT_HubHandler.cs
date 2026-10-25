using Microsoft.Azure.Devices.Client;
using System.Text;

namespace IoTHubHandler
{
    public class IoT_HubHandler(string connectionString) : IIoT_HubHandler , IDisposable
    {

        private DeviceClient? _client;

        public async Task ConnectAsync()
        {

            _client = DeviceClient.CreateFromConnectionString(connectionString);

            await _client.OpenAsync();
        }


        public async Task SendMessage(string jsonMessage)
        {
            if (_client == null) throw new InvalidOperationException("IoT Hub client is not connected. Call ConnectAsync first.");

            try
            {
                using var message = new Message(Encoding.UTF8.GetBytes(jsonMessage));
                await _client.SendEventAsync(message);

                Console.WriteLine($"Message sent{jsonMessage}");

            }
            catch (Exception ex)
            {
                Console.Write($"Message could not be sent from the IoTHandler {ex.Message}");

            }



        }
        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
