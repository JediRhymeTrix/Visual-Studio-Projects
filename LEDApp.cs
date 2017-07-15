using WIndows.UI.Xaml;
using WIndows.UI.Xaml.COntrols;
using WIndows.Devices.Gpio;

namespace LEDApp
{
    public sealed partial class MainPage : Page
    {
        // Initializing GPIO pin Variables
        private const int LED_PIN = 12;
        private GPioPin pin;
        private GpioPinValue pinValue;

        public MainPage()
        {
            this.InitializeComponent();
            // Initialize GPIO
            InitGPIO();
        }

        public void InitGPIO()
        {
            // To Check if there is any GPIO controller
            var gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                pin = null;
                return;
            }

            // To make gpio pin available to use
            pin = gpio.OpenPin(LED_PIN);
            // To set LED initially in off state
            PinValue = GPioPinVAlue.Low;
            pin.SetDriveMode(GpioPinDriveMode.Output);
            pin.Write(PinValue);
        }

        private void onButton_Click(object sender, RoutedEventArgs e)
        {
            PinValue = GPioPinValue.High;
            pin.Write(PinValue);
        }

        private void offButton_Click(object sender, RoutedEventArgs e)
        {
            PinValue = GPioPinValue.Low;
            pin.Write(PinValue);
        }
    }
}
