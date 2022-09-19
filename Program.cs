using ArmaDCSConverter;
using ArmaDCSConverter.ArmaDtos;
using GeographicLib;
using System.Diagnostics.Metrics;

string fileName = "Export.log";
TimeSpan offset = TimeSpan.Zero;

FileStream fileStream = new FileStream(fileName, FileMode.Open);

using StreamReader reader = new StreamReader(fileStream);

reader.ReadLine(); //Skip header

var tracks = new List<Track>();

while (!reader.EndOfStream)
{

    var line = reader.ReadLine();

    if(line == null) tracks.Add(Track.InvalidTrack);

    tracks.Add(Track.GetTrack(line));
}

//Done processing got tracks list as outcome

var trackedEntities = new Dictionary<int, Entity>();

var armaTracks = new List<ArmaTrack>();

var referencePoint = tracks.Where(EntityShouldBeTracked).First().Position;
var origin = new LocalCartesian(referencePoint.Item1, referencePoint.Item2, FeetToMeters(referencePoint.Item3));

foreach (Track track in tracks.Where(EntityShouldBeTracked))
{
    var cartesianPosition = origin.Forward(track.Position.Item1, track.Position.Item2, FeetToMeters(track.Position.Item3));

    if (!trackedEntities.ContainsKey(track.Id))
    {
        armaTracks.Add(new ArmaTrack
        {
            Action = TrackAction.Spawn, 
            Id = track.Id, 
            Name = track.Name, 
            Position = (cartesianPosition.x, cartesianPosition.y, cartesianPosition.z), 
            Time = track.Time + offset.Seconds
        });

        trackedEntities.Add(track.Id, new Entity { Id = track.Id, Name = track.Name });
    }

    var armaTrack = new ArmaTrack
    {
        Action = TrackAction.Move, 
        Id = track.Id, 
        Name = track.Name, 
        Position = (cartesianPosition.x, cartesianPosition.y, cartesianPosition.z), 
        Time = track.Time + offset.Seconds
    };

    armaTracks.Add(armaTrack);

    Console.WriteLine($"{armaTrack.Name} - ({armaTrack.Position.Item1},{armaTrack.Position.Item2},{armaTrack.Position.Item3})");
}

Console.WriteLine("Done");

bool EntityShouldBeTracked(Track track)
{
    //TODO: Testing
    return track.Name.Contains("FA-18C_hornet")/* ||
           track.Name.Contains("AIM_120") ||
           track.Name.Contains("AIM_9") ||
           track.Name.Contains("MIG-21_PILOT") ||
           track.Name.Contains("MiG-21Bis")*/;
}

double FeetToMeters(double feet)
{
    return feet / 3.2808399;
}