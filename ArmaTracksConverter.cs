using ArmaDCSConverter.ArmaDtos;
using GeographicLib;

namespace ArmaDCSConverter;

public class ArmaTracksConverter
{
    private const double Scale = 1;
    private readonly TimeSpan _startCutoff = TimeSpan.FromSeconds(300);

    public ProcessedTracks Process(IEnumerable<Track> rawTracks)
    {
        var limitedTracks = rawTracks.Where(t => t.Time > _startCutoff.Seconds).ToList();

        var trackedEntities = new Dictionary<int, ArmaTrack>();
        var armaTracks = new List<ArmaTrack>();

        var firstTrack = limitedTracks.First();

        var origin = new LocalCartesian(firstTrack.Position.Item1, firstTrack.Position.Item2, firstTrack.Position.Item3);

        foreach (Track track in limitedTracks)
        {
            var cartesianPosition = origin.Forward(track.Position.Item1, track.Position.Item2, track.Position.Item3);

            if (!trackedEntities.ContainsKey(track.Id))
            {
                trackedEntities.Add(track.Id, new ArmaTrack
                {
                    Action = TrackAction.Spawn,
                    Id = track.Id,
                    Name = track.Name,
                    Position = new Position(cartesianPosition).Multiply(Scale),
                    Time = track.Time - firstTrack.Time, //TODO: Remove is used to shift time origin to 0
                });
            }

            var armaTrack = new ArmaTrack
            {
                Action = TrackAction.Move,
                Id = track.Id,
                Name = track.Name,
                Rotation = new Rotation { X = Math.Sin(track.Heading.Value), Y = Math.Cos(track.Heading.Value), Z = 0 }, //TODO: I only have heading so I will have to calculate yaw by height vector diff
                Position = new Position(cartesianPosition).Multiply(Scale),
                Time = track.Time - firstTrack.Time //TODO: Remove is used to shift time origin to 0
            };

            armaTracks.Add(armaTrack);
        }

        return new ProcessedTracks
        {
            MovementTracks = armaTracks,
            TrackedEntities = trackedEntities
        };
    }
}