using ArmaDCSConverter.ArmaDtos;
using System.Text;

namespace ArmaDCSConverter;

public class SqfBuilder
{
    private const int XOffset = -7000;
    private const int YOffset = 10000;
    private const int HeightOffset = 2500;

    public string ConvertToSqfCode(ProcessedTracks processedTracks)
    {
        StringBuilder result = new StringBuilder();

        foreach (KeyValuePair<int, ArmaTrack> trackedEntity in processedTracks.TrackedEntities)
        {
            result.Append($" [] spawn {{ sleep {trackedEntity.Value.Time}; ");

            GenerateSpawnVehicleData(result, trackedEntity);

            GenerateMovementData(result, trackedEntity.Value, processedTracks.MovementTracks);

            result.Append(" }; ");
        }

        return result.ToString();
    }

    private static void GenerateSpawnVehicleData(StringBuilder result, KeyValuePair<int, ArmaTrack> trackedEntity)
    {
        var vehicle = "B_Plane_Fighter_01_F";
        if (trackedEntity.Value.Name.Contains("AIM_120")) vehicle = "O_HMG_01_high_F";
        if (trackedEntity.Value.Name.Contains("AIM_9")) vehicle = "O_HMG_01_F";
        if (trackedEntity.Value.Name.Contains("MiG-21Bis")) vehicle = "O_Plane_CAS_02_F";
        if (trackedEntity.Value.Name.Contains("MIG-21_PILOT")) vehicle = "Land_GarbageBarrel_01_english_F";

        result.AppendLine(
            $"{trackedEntity.Value.ArmaVariableName} = \"{vehicle}\" createVehicle [{trackedEntity.Value.Position.X},{trackedEntity.Value.Position.Y},{trackedEntity.Value.Position.Height}];");

        if (vehicle.Equals("B_Plane_Fighter_01_F") || vehicle.Equals("O_Plane_CAS_02_F"))
        {
            result.AppendLine($"createVehicleCrew {trackedEntity.Value.ArmaVariableName};");
        }

        //result.AppendLine($"{trackedEntity.Value.ArmaVariableName} enableSimulationGlobal false;");
    }

    private void GenerateMovementData(StringBuilder result, ArmaTrack trackedEntity, IEnumerable<ArmaTrack> movementTracks)
    {
        var objectTracks = movementTracks.Where(track => track.Id == trackedEntity.Id);

        var objectDeletion = objectTracks.Last().Time;

        var armaValidTrack = objectTracks.Select(t =>
            $"[{t.Time - trackedEntity.Time},[{t.Position.X + XOffset},{t.Position.Y + YOffset},{t.Position.Height + HeightOffset}],[{t.Rotation.X},{t.Rotation.Y},{t.Rotation.Z}],[0,0,1],[0,0,0]]"); //TODO: Offset height for debugging
        //TODO: I offset this time here as well as this is delta between mission start (original 0) and when the thing got created

        var trackData = $"[{string.Join(",\n", armaValidTrack)}]";

        result.AppendLine($"[{trackedEntity.ArmaVariableName}, {trackData}] spawn BIS_fnc_unitPlay; sleep {objectDeletion - trackedEntity.Time}; deleteVehicle {trackedEntity.ArmaVariableName};");
    }
}