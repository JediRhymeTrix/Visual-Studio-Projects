using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.Devices.Gpio;

namespace LEDApp
{
    public sealed partial class MainPage : Page
    {
        // Initializing GPIO pin variables

        private const int LED_PIN = 12;
        private GpioPin pin;
        private GpioPinValue pinValue;

        public MainPage()
        {
            this.InitializeComponent();

            // Initialize GPIO

            InitGPIO();
        }

        private void InitGPIO()
        {
            // To check if there is any GPIO controller

            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                pin = null;
                return;
            }

            // To make gpio pin available to use

            pin = gpio.OpenPin(LED_PIN);

            // To set LED initially in OFF state

            pinValue = GpioPinValue.Low;
            pin.SetDriveMode(GpioPinDriveMode.Output);
            pin.Write(pinValue);
        }

        private void onButton_Click(object sender, RoutedEventArgs e)
        {
            pinValue = GpioPinValue.High;
            pin.Write(pinValue);

            button.Background = new SolidColorBrush(Windows.UI.Colors.Green);
        }

        private void offButton_Click(object sender, RoutedEventArgs e)
        {
            pinValue = GpioPinValue.Low;
            pin.Write(pinValue);

            button.Background = new SolidColorBrush(Windows.UI.Colors.Red);
        }
    }
}
