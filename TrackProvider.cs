namespace ArmaDCSConverter;

public class TrackProvider
{
    public IEnumerable<Track> GetTracks(string fileName)
    {
        FileStream fileStream = new FileStream(fileName, FileMode.Open);

        using StreamReader reader = new StreamReader(fileStream);

        var tracks = new List<Track>();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            var track = line == null ? Track.InvalidTrack : Track.GetTrack(line);

            tracks.Add(track);
        }

        return tracks;
    }
}