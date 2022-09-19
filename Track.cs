using GeographicLib;

namespace ArmaDCSConverter;

public class Track
{
    public static readonly Track InvalidTrack = new Track { Id = -1, Name = "Invalid Track" };

    private Track() {}

    public static Track GetTrack(string rawLine)
    {
        var data = rawLine.Split('|');

        if (data.Length != 9) return InvalidTrack;

        var track = new Track();

        try
        {
            track.Time = int.Parse(data[0]);
            track.Id = int.Parse(data[1]);

            track.Position = (double.Parse(data[5]), double.Parse(data[6]), double.Parse(data[7])); //TODO: Height might be wrong

            track.Heading = new Degree { Value = float.Parse(data[8]) };
        }
        catch(Exception)
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

    public (double, double, double) Position { get; set; }

    public Degree Heading { get; set; }

}

public class Degree
{
    public double Value { get; set; }

    public override string ToString()
    {
        return Value.ToString();
    }
}