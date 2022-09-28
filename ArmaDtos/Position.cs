namespace ArmaDCSConverter.ArmaDtos;

public class Position
{
    public Position()
    {
    }

    public Position((double, double, double) cartesianPosition)
    {
        X = cartesianPosition.Item1;
        Y = cartesianPosition.Item2;
        Height = cartesianPosition.Item3;
    }

    public double X { get; set; }

    public double Y { get; set; }

    public double Height { get; set; }
}

public static class PositionExtensions
{
    public static Position Multiply(this Position value, double amount)
    {
        return new Position { X = value.X * amount, Y = value.Y * amount, Height = value.Height * amount };
    }
}