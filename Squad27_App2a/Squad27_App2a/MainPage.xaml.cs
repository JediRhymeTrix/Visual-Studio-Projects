using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.Devices.Gpio;

namespace Squad27_App2a
{
    public sealed partial class MainPage : Page
    {
        private const int LED_PIN = 12;
        private const int BUTTON_PIN = 69;
        private GpioPin ledPin;
        private GpioPin buttonPin;
        private GpioPinValue ledPinValue = GpioPinValue.High;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

        public MainPage()
        {
            this.InitializeComponent();
            InitGpio();
        }

        private void InitGpio()
        {
            var gpio = GpioController.GetDefault();

            // show an error if there is no GPIO controller
            if (gpio == null)
            {
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            buttonPin = gpio.OpenPin(BUTTON_PIN);
            ledPin = gpio.OpenPin(LED_PIN);

            ledPin.Write(GpioPinValue.Low);
            ledPin.SetDriveMode(GpioPinDriveMode.Output);

            if (buttonPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                buttonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                buttonPin.SetDriveMode(GpioPinDriveMode.Input);

            buttonPin.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            buttonPin.ValueChanged += buttonPin_ValueChanged;

            GpioStatus.Text = "GPIO pins initialized correctly.";
        }

        private void buttonPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            if (e.Edge == GpioPinEdge.RisingEdge)
            {
                ledPinValue = GpioPinValue.High;
                ledPin.Write(ledPinValue);
            }
            else
            {
                ledPinValue = GpioPinValue.Low;
                ledPin.Write(ledPinValue);
            }

            var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                if (e.Edge == GpioPinEdge.RisingEdge)
                {
                    ledEllipse.Fill = redBrush;
                    GpioStatus.Text = "Button Pressed.";
                }
                else
                {
                    ledEllipse.Fill = grayBrush;
                    GpioStatus.Text = "Button Released.";
                }
            });
        }
    }
} 