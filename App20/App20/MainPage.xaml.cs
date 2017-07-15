using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Windows.Devices.Gpio;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App20
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        const string deviceConnectionString = "HostName=Squad27-Hub.azure-devices.net;DeviceId=TestDevice;SharedAccessKey=k4ujtHpbIg5vDqb//6Ld4uUNaZRdxGNpL3IVOlO3XE4=";
        private const int BULB_PIN = 12;
        private const int PIR_PIN = 69;
        private GpioPin BulbPin;
        private GpioPin PirPin;
        private GpioPinValue BulbPinValue;

        public MainPage()
        {
            this.InitializeComponent();
            InitGPIO();
            Recieve_Alert();
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();
            if(gpio==null)
            {
                return;
            }
            PirPin = gpio.OpenPin(PIR_PIN);
            BulbPin = gpio.OpenPin(BULB_PIN);

            BulbPin.Write(GpioPinValue.Low);
            BulbPin.SetDriveMode(GpioPinDriveMode.Output);

            PirPin.SetDriveMode(GpioPinDriveMode.Input);
            PirPin.DebounceTimeout = TimeSpan.FromMilliseconds(500);
            PirPin.ValueChanged += buttonPin_ValueChanged;
        }
        public async Task SendDeviceToCloudMessageAsync()
        {
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);
            var DeviceId = "TestDevice";
            DateTime timeStamp = DateTime.Now;
            var str = DeviceId + "," + timeStamp;
            var message = new Message(Encoding.ASCII.GetBytes(str));
            await deviceClient.SendEventAsync(message);

        }

        private void buttonPin_ValueChanged(GpioPin sender,GpioPinValueChangedEventArgs e)
        {
            if(e.Edge == GpioPinEdge.FallingEdge)
            {
                SendDeviceToCloudMessageAsync();
            }
        }

        public async Task<string> ReceiveCloudToDeviceMessageAsync()
        {
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);

            while(true)
            {
                var receivedMessage = await deviceClient.ReceiveAsync();
                if(receivedMessage!=null)
                {
                    var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    await deviceClient.CompleteAsync(receivedMessage);
                    return messageData;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public async Task Recieve_Alert()
        {
            string Value = await ReceiveCloudToDeviceMessageAsync();
            textBlock.Text = Value;
            if(Value.Equals("Alert"))
            {
                BulbPinValue = GpioPinValue.High;
                BulbPin.Write(BulbPinValue);
                await Task.Delay(1000);
                BulbPinValue = GpioPinValue.Low;
                BulbPin.Write(BulbPinValue);
            }

            await Recieve_Alert();
        }
    }
}
