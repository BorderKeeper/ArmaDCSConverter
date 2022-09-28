using System.Text;

namespace ArmaDCSConverter;

public class Converter
{
    public const string FileName = "Export.log";

    private readonly TrackProvider _trackProvider;
    private readonly ArmaTracksConverter _armaTracksConverter;
    private readonly SqfBuilder _sqfBuilder;

    public Converter()
    {
        _armaTracksConverter = new ArmaTracksConverter();
        _trackProvider = new TrackProvider();
        _sqfBuilder = new SqfBuilder();
    }

    public string Convert()
    {
        var rawTracks = _trackProvider.GetTracks(FileName);

        rawTracks = rawTracks.Where(EntityShouldBeTracked);

        var processedTracks = _armaTracksConverter.Process(rawTracks);

        var sqfCode = _sqfBuilder.ConvertToSqfCode(processedTracks);

        return sqfCode;
    }

    private bool EntityShouldBeTracked(Track track)
    {
        //TODO: Testing
        return track.Name.Contains("FA-18C_hornet") ||
               track.Name.Contains("AIM_120") ||
               track.Name.Contains("AIM_9") ||
               track.Name.Contains("MIG-21_PILOT") ||
               track.Name.Contains("MiG-21Bis");
    }
}