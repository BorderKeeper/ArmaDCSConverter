namespace ArmaDCSConverter.ArmaDtos;

public class ProcessedTracks
{
    public IDictionary<int, ArmaTrack> TrackedEntities { get; set; }

    public IEnumerable<ArmaTrack> MovementTracks { get; set; }
}