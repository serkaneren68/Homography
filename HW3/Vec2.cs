namespace ArHw3;

public readonly struct Vec2
{
    public double X { get; }
    public double Y { get; }

    public Vec2(double x, double y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X:F3}, {Y:F3})";
}
