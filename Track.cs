using ArmaDCSConverter.ArmaDtos;

namespace ArmaDCSConverter;

public class Track
{
    public static readonly Track InvalidTrack = new() { Id = -1, Name = "Invalid Track" };

    private Track() { }

    public static Track GetTrack(string rawLine)
    {
        var data = rawLine.Split('|');

        if (data.Length != 11) return InvalidTrack;

        var track = new Track();

        try
        {
            track.Time = int.Parse(data[0]);
            track.Id = int.Parse(data[1]);

            track.Position = new Position { X = double.Parse(data[5]), Y = double.Parse(data[6]), Z = double.Parse(data[7]) };

            track.Heading = new Degree { Value = float.Parse(data[8]) };
            track.Pitch = new Degree { Value = float.Parse(data[9]) };
            track.Bank = new Degree { Value = float.Parse(data[10]) };
        }
        catch (Exception)
        {
            return InvalidTrack;
        }

        track.Name = data[2] == string.Empty ? "Invalid" : data[2];

        int.TryParse(data[3], out var trackCountry);
        track.Country = trackCountry;

        track.Faction = data[4] == string.Empty ? "Invalid" : data[4];

        return track;
    }

    public int Time { get; set; }

    public int Id { get; set; }

    public string Name { get; set; }

    public int Country { get; set; } //TODO: Should be Enum

    public string Faction { get; set; }

    public Position Position { get; set; }

    /// <summary>
    /// Yaw
    /// </summary>
    public Degree Heading { get; set; }

    public Degree Pitch { get; set; }

    /// <summary>
    /// Roll
    /// </summary>
    public Degree Bank { get; set; }
}

public class Degree
{
    public double Value { get; set; }

    public override string ToString()
    {
        return Value.ToString();
    }
}