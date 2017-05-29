using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace mqtt_dotnet_sample
{
    class Program
    {
        static MqttClient client;

        static void Main(string[] args)
        {
            Console.WriteLine("MQTT Client Sample");

            var address = "127.0.0.1";

            client = new MqttClient(address);
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

            client.Connect(Guid.NewGuid().ToString());

            string[] topic = { "sensor/temp", "sensor/humidity" };

            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE };

            client.Subscribe(topic, qosLevels);

            // Send


            SendDeviceToMQTTMessagesAsync();

            Console.ReadLine();


        }

        private static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = Encoding.ASCII.GetString(e.Message);

            Console.WriteLine(message);
        }

        private static async void SendDeviceToMQTTMessagesAsync()
        {
            double minTemperature = 20;
            double minHumidity = 60;
            Random rand = new Random();

            while (true)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                var telemetryDataPoint = new
                {
                    deviceId = "dev-001",
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = Encoding.ASCII.GetBytes(messageString);

                client.Publish("sensor/temp", message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                await Task.Delay(1000);
            }
        }
    }
}