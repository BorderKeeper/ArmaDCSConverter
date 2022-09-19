using GeographicLib;

namespace ArmaDCSConverter.ArmaDtos;

public class ArmaTrack
{
    public int Id { get; set; }

    public string Name { get; set; }

    public double Time { get; set; }

    public (double, double, double) Position { get; set; }

    public (double, double, double) Rotation { get; set; }

    public TrackAction Action { get; set; }
}

public enum TrackAction
{
    Spawn,
    Move
}