/*Program to send message Cloud to Device(C2D) message*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
//using WindowsAzure.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Azure.Devices;

namespace c2d_console
{
    class Program
    {
        static ServiceClient serviceClient;
        static DateTime startingDateTimeUtc;
        static Dictionary<string, int> dictionary = new Dictionary<string, int>();

        //check this once!!
        static string connectionString = "HostName=Squad27-Hub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=/AO1OWNmF7P6khzjFxEJZXvXP81DHnVVIsiPNPVE74E=";
        static void Main(string[] args)
        {
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            ConnectCloud();
        }

        public static void ConnectCloud()
        {
            string iotHubD2cEndpoint = "messages/events";
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);
            startingDateTimeUtc = DateTime.Now;
            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;

            foreach (string partition in d2cPartitions)
            {
                var receiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, startingDateTimeUtc);
                ReceiveMessagesFromDeviceAsync(receiver);
            }
            Console.ReadLine();
        }

        public static async Task ReceiveMessagesFromDeviceAsync(EventHubReceiver receiver)
        {
            while (true)
            {
                EventData eventData = await receiver.ReceiveAsync();
                if (eventData == null)
                    continue;
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                string[] value = data.Split(',');
                DangerWindow(value[0], value[1]);
            }
        }

        public static void DangerWindow(string deviceId, string deviceTimeStamp)
        {
            int start, end;
            DateTime dt = Convert.ToDateTime(deviceTimeStamp);
            TimeSpan current = dt.TimeOfDay;
            TimeSpan startTime = new TimeSpan(10, 00, 0);
            TimeSpan endTime = new TimeSpan(20, 00, 00);
            start = current.CompareTo(startTime);//current>startTime -- start= +ve
            end = current.CompareTo(endTime);//current<endTime -- end= -ve
            Console.WriteLine(start + " " + end);
            if (start > 0 && end < 0)
            {
                if (dictionary.ContainsKey(deviceId))
                {
                    dictionary[deviceId]++;
                    if (dictionary[deviceId] == 10)
                    {
                        SendCloudToDeviceMessageAsync(deviceId, "Alert");
                        dictionary[deviceId] = 0;
                    }
                    Console.WriteLine("{0} , {1}", deviceId, dictionary[deviceId]);
                }
                else
                {
                    dictionary.Add(deviceId, 1);
                    Console.WriteLine("{0} , {1}", deviceId, dictionary[deviceId]);
                }
            }
        }

        public async static Task SendCloudToDeviceMessageAsync(string destination, string message)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
            await serviceClient.SendAsync(destination, commandMessage);
        }
    }
}