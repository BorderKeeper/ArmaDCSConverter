using ArmaDCSConverter.ArmaDtos;
using System.Text;

namespace ArmaDCSConverter;

public class SqfBuilder
{
    private const int XOffset = -7000;
    private const int YOffset = 10000;
    private const int ZOffset = 2500;

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
            $"{trackedEntity.Value.ArmaVariableName} = \"{vehicle}\" createVehicle [{trackedEntity.Value.Position.X},{trackedEntity.Value.Position.Y},{trackedEntity.Value.Position.Z}];");

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
            $"[{t.Time - trackedEntity.Time},[{t.Position.X + XOffset:F},{t.Position.Y + YOffset:F},{t.Position.Z + ZOffset:F}],[{t.DirectionVector.X:F},{t.DirectionVector.Z:F},{t.DirectionVector.Y:F}],[{t.UpVector.X:F},{t.UpVector.Y:F},{t.UpVector.Z:F}],[{t.Speed.X:F},{t.Speed.Y:F},{t.Speed.Z:F}]]"); //TODO: Offset height for debugging
        //TODO: I offset this time here as well as this is delta between mission start (original 0) and when the thing got created

        var trackData = $"[{string.Join(",\n", armaValidTrack)}]";

        result.AppendLine($"[{trackedEntity.ArmaVariableName}, {trackData}] spawn BIS_fnc_unitPlay; sleep {objectDeletion - trackedEntity.Time}; {{ {trackedEntity.ArmaVariableName} deleteVehicleCrew _x }} forEach crew {trackedEntity.ArmaVariableName}; deleteVehicle {trackedEntity.ArmaVariableName};");
    }
}