using ArmaDCSConverter;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var converter = new Converter();

        var output = converter.Convert();

        Clipboard.SetText(output);
        Console.WriteLine($"Output with size {output.Length} copied to your clipboard");
    }
}