using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttHubGateway
{

    public record TelemetryMessageDto(
        string DeviceId,
        double Value,
        string Unit,
        string GatewayId);

}