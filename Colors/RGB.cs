using System;


namespace GraphAlgorithmTester.Colors;

/// <summary>
/// RGB structure.
/// </summary>
public struct RGB
{
    /// <summary>
    /// Gets an empty RGB structure;
    /// </summary>
    public static readonly RGB Empty = new RGB();

    private int red;
    private int green;
    private int blue;

    public static bool operator ==(RGB item1, RGB item2)
    {
        return (
            item1.Red == item2.Red
            && item1.Green == item2.Green
            && item1.Blue == item2.Blue
            );
    }

    public static bool operator !=(RGB item1, RGB item2)
    {
        return (
            item1.Red != item2.Red
            || item1.Green != item2.Green
            || item1.Blue != item2.Blue
            );
    }

    /// <summary>
    /// Gets or sets red value.
    /// </summary>
    public int Red
    {
        get
        {
            return red;
        }
        set
        {
            red = (value > 255) ? 255 : ((value < 0) ? 0 : value);
        }
    }

    /// <summary>
    /// Gets or sets red value.
    /// </summary>
    public int Green
    {
        get
        {
            return green;
        }
        set
        {
            green = (value > 255) ? 255 : ((value < 0) ? 0 : value);
        }
    }

    /// <summary>
    /// Gets or sets red value.
    /// </summary>
    public int Blue
    {
        get
        {
            return blue;
        }
        set
        {
            blue = (value > 255) ? 255 : ((value < 0) ? 0 : value);
        }
    }

    public RGB(int R, int G, int B)
    {
        this.red = (R > 255) ? 255 : ((R < 0) ? 0 : R);
        this.green = (G > 255) ? 255 : ((G < 0) ? 0 : G);
        this.blue = (B > 255) ? 255 : ((B < 0) ? 0 : B);
    }

    public override bool Equals(Object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;

        return (this == (RGB)obj);
    }
    public override string ToString() => $"({this.Red},{this.Green},{this.blue})";
    public override int GetHashCode()
    {
        return Red.GetHashCode() ^ Green.GetHashCode() ^ Blue.GetHashCode();
    }
}