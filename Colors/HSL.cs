﻿using System;

namespace GraphAlgorithmTester.Colors;

/// <summary>
/// Structure to define HSL.
/// </summary>
public struct HSL
{
    /// <summary>
    /// Gets an empty HSL structure;
    /// </summary>
    public static readonly HSL Empty = new HSL();

    private double hue;
    private double saturation;
    private double luminance;

    public static bool operator ==(HSL item1, HSL item2)
    {
        return (
            item1.Hue == item2.Hue
            && item1.Saturation == item2.Saturation
            && item1.Luminance == item2.Luminance
            );
    }

    public static bool operator !=(HSL item1, HSL item2)
    {
        return (
            item1.Hue != item2.Hue
            || item1.Saturation != item2.Saturation
            || item1.Luminance != item2.Luminance
            );
    }

    /// <summary>
    /// Gets or sets the hue component.
    /// </summary>
    public double Hue
    {
        get
        {
            return hue;
        }
        set
        {
            hue = (value > 360) ? 360 : ((value < 0) ? 0 : value);
        }
    }

    /// <summary>
    /// Gets or sets saturation component.
    /// </summary>
    public double Saturation
    {
        get
        {
            return saturation;
        }
        set
        {
            saturation = (value > 1) ? 1 : ((value < 0) ? 0 : value);
        }
    }

    /// <summary>
    /// Gets or sets the luminance component.
    /// </summary>
    public double Luminance
    {
        get
        {
            return luminance;
        }
        set
        {
            luminance = (value > 1) ? 1 : ((value < 0) ? 0 : value);
        }
    }

    /// <summary>
    /// Creates an instance of a HSL structure.
    /// </summary>
    /// <param name="h">Hue value.</param>
    /// <param name="s">Saturation value.</param>
    /// <param name="l">Lightness value.</param>
    public HSL(double h, double s, double l)
    {
        this.hue = (h > 360) ? 360 : ((h < 0) ? 0 : h);
        this.saturation = (s > 1) ? 1 : ((s < 0) ? 0 : s);
        this.luminance = (l > 1) ? 1 : ((l < 0) ? 0 : l);
    }

    public override bool Equals(Object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;

        return (this == (HSL)obj);
    }

    public override int GetHashCode()
    {
        return Hue.GetHashCode() ^ Saturation.GetHashCode() ^
            Luminance.GetHashCode();
    }
}