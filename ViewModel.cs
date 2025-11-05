using System.ComponentModel;
using System.Runtime.CompilerServices;

public class TelemetryViewModel : INotifyPropertyChanged
{
    double _speedKph, _rpm, _trackTemp, _airTemp; int _gear;

    public double SpeedKph { get => _speedKph; set => Set(ref _speedKph, value); }
    public double RPM { get => _rpm; set => Set(ref _rpm, value); }
    public int Gear { get => _gear; set => Set(ref _gear, value); }
    public double TrackTemp { get => _trackTemp; set => Set(ref _trackTemp, value); }
    public double AirTemp { get => _airTemp; set => Set(ref _airTemp, value); }

    public event PropertyChangedEventHandler? PropertyChanged;
    void Set<T>(ref T field, T value, [CallerMemberName] string? n = null)
    {
        if (!Equals(field, value)) { field = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n)); }
    }
}
