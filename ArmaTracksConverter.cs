using ArmaDCSConverter.ArmaDtos;

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
        var previousTrack = firstTrack;

        foreach (Track track in limitedTracks)
        {
            if (track.Time == previousTrack.Time && track.Id == previousTrack.Id) continue; //TODO: There is some duplication in my current export.log

            var speed = new Vector
            {
                X = track.Position.X - previousTrack.Position.X,
                Y = track.Position.Y - previousTrack.Position.Y,
                Z = track.Position.Z - previousTrack.Position.Z
            };

            var shiftedPosition = new Position
            {
                X = track.Position.X - firstTrack.Position.X,
                Y = track.Position.Y - firstTrack.Position.Y,
                Z = track.Position.Z - firstTrack.Position.Z
            }.Multiply(Scale); //TODO: Remove is used to shift place origin to 0

            if (!trackedEntities.ContainsKey(track.Id))
            {
                trackedEntities.Add(track.Id, new ArmaTrack //TODO: Does this need rotation and speed too?
                {
                    Action = TrackAction.Spawn,
                    Id = track.Id,
                    Name = track.Name,
                    Position = shiftedPosition,
                    Time = track.Time - firstTrack.Time, //TODO: Remove is used to shift time origin to 0
                });
            }

            var directionVector = new Vector
            {
                X = Math.Cos(track.Pitch.Value) * Math.Cos(track.Heading.Value),
                Y = Math.Cos(track.Pitch.Value) * Math.Sin(track.Heading.Value),
                Z = Math.Sin(track.Pitch.Value)
            };

            var upVector = new Vector
            {
                X = Math.Cos(track.Bank.Value) * Math.Cos(track.Heading.Value),
                Y = Math.Cos(track.Bank.Value) * Math.Sin(track.Heading.Value),
                Z = Math.Sin(track.Bank.Value)
            };

            var armaTrack = new ArmaTrack
            {
                Action = TrackAction.Move,
                Id = track.Id,
                Name = track.Name,
                DirectionVector = directionVector,
                UpVector = upVector,
                Position = shiftedPosition,
                Speed = speed,
                Time = track.Time - firstTrack.Time //TODO: Remove is used to shift time origin to 0
            };

            armaTracks.Add(armaTrack);

            previousTrack = track;
        }

        return new ProcessedTracks
        {
            MovementTracks = armaTracks,
            TrackedEntities = trackedEntities
        };
    }
}