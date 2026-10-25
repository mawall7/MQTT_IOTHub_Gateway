using IoTHubHandler;
using MqttHubGateway;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;



namespace MessagingApplication { 
    public class Program {

         
        public static async Task Main(string[] args){

            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddSingleton<IMqttSubscriberService>(provider =>
            new MqttSubscriberService("mqtt_admin@mqtt-broker-mw.local", 1883, "telemetry/#"));

            var connectionString = builder.Configuration.GetConnectionString("AzIoTHubConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string för IoTHub hittades inte!");
            }

            builder.Services.AddSingleton<IoT_HubHandler>(provider =>
        
            new IoT_HubHandler(connectionString));

            // När Gateway skapas, injiceras automatiskt de två tjänsterna ovan i dess konstruktor!
            builder.Services.AddSingleton<Gateway>();

            var host = builder.Build();
            var mqttService = host.Services.GetRequiredService<IMqttSubscriberService>();
            var gatevay = host.Services.GetRequiredService<Gateway>();

           
                
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                try
                {
                    Console.WriteLine("Ansluter till Mosquitto...");

                    await mqttService.ConnectAsync(cts.Token);
                    
                    Console.WriteLine("MQTT-klienten är igång!");
                }
                catch(OperationCanceledException)
                {
                    Console.WriteLine($"Kunde inte ansluta till MQtt-brokern innan tidsgränsen");

                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Fel vid Mqtt anslutning{ex.Message}");
                }
            

            await host.RunAsync();



        }
        
    }

}