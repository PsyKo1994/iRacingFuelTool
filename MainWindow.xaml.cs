using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace iRacingFuelTool
{
    public partial class MainWindow : Window
    {
        private readonly Random _rand = new Random();
        private readonly DispatcherTimer _timer;
        private readonly TelemetryReader _telemetryReader = new();
        private readonly CancellationTokenSource _cts = new();



        public MainWindow()
        {
            //Start UI
            InitializeComponent();

            //Start Telemetry
            StartTelemetry();

            //Create a timer that ticks 2 times a second
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //Get telemetry values from TelemetryReader
            double _TrackTemp = _telemetryReader.TrackTemp;
            double _AirTemp = _telemetryReader.AirTemp;

            //Pretend values
            double avgFuelPerLap = Math.Round(2.5 + _rand.NextDouble() * 0.3, 2); // e.g. 2.5 L/lap
            int lapsRemaining = _rand.Next(5, 20); // e.g. 12 laps left
            double currentFuel = Math.Round(15 + _rand.NextDouble() * 5, 1); // e.g. 17.2 L in tank

            //Calc fuel needed to finish
            double fuelNeeded = Math.Round(avgFuelPerLap * lapsRemaining, 1);

            //We'll give a fuel safety buffer of 2L
            double safetyBuffer = 2.0;

            //How much more we need compared to what's in the tank
            double fuelToAdd = Math.Round(fuelNeeded + safetyBuffer - currentFuel, 1);
            if (fuelToAdd < 0) fuelToAdd = 0;

            //Push to UI
            AvgFuelPerLapText.Text = avgFuelPerLap.ToString("0.00");
            LapsRemainingText.Text = lapsRemaining.ToString();
            FuelNeededText.Text = fuelNeeded.ToString("0.0");
            FuelToAddText.Text = fuelToAdd.ToString("0.0");
            TrackTemp.Text = _TrackTemp.ToString("00.00 °C");
            AirTemp.Text = _AirTemp.ToString("00.00 °C");

            //Status logic
            if (fuelToAdd > 0)
            {
                StatusText.Text = $"BOX THIS LAP: Add {fuelToAdd:0.0} L";
                StatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
            else
            {
                StatusText.Text = "You're good to the end ✅";
                StatusText.Foreground = System.Windows.Media.Brushes.Green;
            }
        }

        private async void StartTelemetry()
        {
            try
            {
                await _telemetryReader.StartAsync(_cts.Token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Telemetry failed: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _cts.Cancel();  // stop gracefully when window closes
            base.OnClosed(e);
        }
    }
}
