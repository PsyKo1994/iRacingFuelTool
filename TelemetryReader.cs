using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SVappsLAB.iRacingTelemetrySDK;

//Define the telemetry variables you want to track
[RequiredTelemetryVars(["Speed", "RPM", "Gear", "TrackTemp", "AirTemp"])]

public class TelemetryReader
{
    //Values being captured
    public double TrackTemp { get; private set; }
    public double AirTemp { get; private set; }

    //Tassk to start telemetry monitoring
    public async Task StartAsync(CancellationToken token)
    {
        var logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }).CreateLogger("TelemetryApp");

        var ibtOptions = (IBTOptions?)null;

        using var client = TelemetryClient<TelemetryData>.Create(logger, ibtOptions);

        

    client.OnTelemetryUpdate += (sender, data) =>
        {
            System.Diagnostics.Debug.WriteLine($"Speed: {data.Speed:F0} KPH, RPM: {data.RPM:F0}, Gear: {data.Gear}, Track Temp: {data.TrackTemp:F2}, Air Temp: {data.AirTemp:F2}");
            TrackTemp = data.TrackTemp;
            AirTemp = data.AirTemp;


        };

        await client.Monitor(token);
    }
}
