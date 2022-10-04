namespace ArmaDCSConverter.ArmaDtos;

public class ArmaTrack
{
    public int Id { get; set; }

    public string Name { get; set; }

    public double Time { get; set; }

    public Position Position { get; set; } = new();

    public Vector DirectionVector { get; set; } = new();

    public Vector UpVector { get; set; } = new();

    public Vector Speed { get; set; } = new();

    public TrackAction Action { get; set; }

    public string ArmaVariableName => $"_v{Id}";
}

public enum TrackAction
{
    Spawn,
    Move
}