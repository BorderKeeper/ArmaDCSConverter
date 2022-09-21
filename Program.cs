using System.Text;
using ArmaDCSConverter;
using ArmaDCSConverter.ArmaDtos;
using GeographicLib;

string fileName = "Export.log";

StringBuilder result = new StringBuilder();

FileStream fileStream = new FileStream(fileName, FileMode.Open);

using StreamReader reader = new StreamReader(fileStream);

reader.ReadLine(); //Skip header

var tracks = new List<Track>();

while (!reader.EndOfStream)
{
    var line = reader.ReadLine();

    if(line == null) tracks.Add(Track.InvalidTrack);

    var track = Track.GetTrack(line);

    if(EntityShouldBeTracked(track))
        tracks.Add(Track.GetTrack(line));
}

//Done processing got tracks list as outcome

var trackedEntities = new Dictionary<int, ArmaTrack>();
var armaTracks = new List<ArmaTrack>();

tracks = tracks.Where(t => t.Time > 300).ToList(); //TODO: Remove

var firstTrack = tracks.First();
var origin = new LocalCartesian(firstTrack.Position.Item1, firstTrack.Position.Item2, firstTrack.Position.Item3);

double scale = 0.25; //TODO: Scale should be 1

foreach (Track track in tracks)
{
    var cartesianPosition = origin.Forward(track.Position.Item1, track.Position.Item2, track.Position.Item3);

    if (!trackedEntities.ContainsKey(track.Id))
    {
        trackedEntities.Add(track.Id, new ArmaTrack
        {
            Action = TrackAction.Spawn, 
            Id = track.Id, 
            Name = track.Name, 
            Position = (cartesianPosition.x * scale, cartesianPosition.y * scale, cartesianPosition.z * scale), 
            Time = track.Time - firstTrack.Time, //TODO: Remove is used to shift time origin to 0
        });
    }

    var armaTrack = new ArmaTrack
    {
        Action = TrackAction.Move, 
        Id = track.Id, 
        Name = track.Name, 
        Rotation = (Math.Sin(track.Heading.Value), Math.Cos(track.Heading.Value), 0), //TODO: I only have heading so I will have to calculate yaw by height vector diff
        Position = (cartesianPosition.x * scale, cartesianPosition.y * scale, cartesianPosition.z * scale), 
        Time = track.Time - firstTrack.Time //TODO: Remove is used to shift time origin to 0
    };

    armaTracks.Add(armaTrack);
}

//Got location tracks and list of things to spawn

foreach (KeyValuePair<int, ArmaTrack> trackedEntity in trackedEntities)
{
    var vehicle = "B_Plane_Fighter_01_F";
    if (trackedEntity.Value.Name.Contains("AIM_120")) vehicle = "O_HMG_01_high_F";
    if (trackedEntity.Value.Name.Contains("AIM_9")) vehicle = "O_HMG_01_F";
    if (trackedEntity.Value.Name.Contains("MiG-21Bis")) vehicle = "O_Plane_CAS_02_F";
    if (trackedEntity.Value.Name.Contains("MIG-21_PILOT")) vehicle = "Land_GarbageBarrel_01_english_F";

    result.Append($" [] spawn {{ sleep {trackedEntity.Value.Time}; ");

    result.AppendLine($"{trackedEntity.Value.ArmaVariableName} = \"{vehicle}\" createVehicle [{trackedEntity.Value.Position.Item1},{trackedEntity.Value.Position.Item2},{trackedEntity.Value.Position.Item3}];");
    //result.AppendLine($"{trackedEntity.Value.ArmaVariableName} enableSimulationGlobal false;");

    ConstructRecording(trackedEntity.Value);

    result.Append(" }; ");
}

Console.WriteLine(result.ToString());

//DONE

void ConstructRecording(ArmaTrack trackedEntity)
{
    var objectTracks = armaTracks.Where(track => track.Id == trackedEntity.Id);

    var objectDeletion = objectTracks.Last().Time;

    var armaValidTrack = objectTracks.Select(t =>
        $"[{t.Time - trackedEntity.Time},[{t.Position.Item1 + 1000},{t.Position.Item2 + 1000},{t.Position.Item3 + 1000}],[{t.Rotation.Item1},{t.Rotation.Item2},{t.Rotation.Item3}],[0,0,1],[0,0,0]]"); //TODO: Offset height for debugging
        //TODO: I offset this time here as well as this is delta between mission start (original 0) and when the thing got created

    var trackData = $"[{string.Join(",\n", armaValidTrack)}]";

    result.AppendLine($"[{trackedEntity.ArmaVariableName}, {trackData}] spawn BIS_fnc_unitPlay; sleep {objectDeletion - trackedEntity.Time}; deleteVehicle {trackedEntity.ArmaVariableName};");
}

bool EntityShouldBeTracked(Track track)
{
    //TODO: Testing
    return track.Name.Contains("FA-18C_hornet") ||
           track.Name.Contains("AIM_120") ||
           track.Name.Contains("AIM_9") ||
           track.Name.Contains("MIG-21_PILOT") ||
           track.Name.Contains("MiG-21Bis");
}