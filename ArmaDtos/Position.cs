namespace ArmaDCSConverter.ArmaDtos;

public class Position
{

    public double X { get; set; }

    public double Y { get; set; }

    public double Z { get; set; }
}

public static class PositionExtensions
{
    public static Position Multiply(this Position value, double amount)
    {
        return new Position { X = value.X * amount, Y = value.Y * amount, Z = value.Z * amount };
    }
}