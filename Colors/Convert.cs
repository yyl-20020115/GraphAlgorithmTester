using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GraphAlgorithmTester.Colors;

public static class Convert
{
    /// <summary>
    /// Converts RGB to HSB.
    /// </summary>
    public static HSB RGBtoHSB(int red, int green, int blue)
    {
        // normalize red, green and blue values
        double r = ((double)red / 255.0);
        double g = ((double)green / 255.0);
        double b = ((double)blue / 255.0);

        // conversion start
        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));

        double h = 0.0;
        if (max == r && g >= b)
        {
            h = 60 * (g - b) / (max - min);
        }
        else if (max == r && g < b)
        {
            h = 60 * (g - b) / (max - min) + 360;
        }
        else if (max == g)
        {
            h = 60 * (b - r) / (max - min) + 120;
        }
        else if (max == b)
        {
            h = 60 * (r - g) / (max - min) + 240;
        }

        double s = (max == 0) ? 0.0 : (1.0 - (min / max));

        return new HSB(h, s, (double)max);
    }
    /// <summary>
    /// Converts RGB to HSL.
    /// </summary>
    /// <param name="red">Red value, must be in [0,255].</param>
    /// <param name="green">Green value, must be in [0,255].</param>
    /// <param name="blue">Blue value, must be in [0,255].</param>
    public static HSL RGBtoHSL(int red, int green, int blue)
    {
        double h = 0, s = 0, l = 0;

        // normalize red, green, blue values
        double r = (double)red / 255.0;
        double g = (double)green / 255.0;
        double b = (double)blue / 255.0;

        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));

        // hue
        if (max == min)
        {
            h = 0; // undefined
        }
        else if (max == r && g >= b)
        {
            h = 60.0 * (g - b) / (max - min);
        }
        else if (max == r && g < b)
        {
            h = 60.0 * (g - b) / (max - min) + 360.0;
        }
        else if (max == g)
        {
            h = 60.0 * (b - r) / (max - min) + 120.0;
        }
        else if (max == b)
        {
            h = 60.0 * (r - g) / (max - min) + 240.0;
        }

        // luminance
        l = (max + min) / 2.0;

        // saturation
        if (l == 0 || max == min)
        {
            s = 0;
        }
        else if (0 < l && l <= 0.5)
        {
            s = (max - min) / (max + min);
        }
        else if (l > 0.5)
        {
            s = (max - min) / (2 - (max + min)); //(max-min > 0)?
        }

        return new HSL(Math.Round(h, 2), Math.Round(s, 2), Math.Round(l, 2));
        //        Double.Parse(String.Format("{0:0.##}", h)),
        //        Double.Parse(String.Format("{0:0.##}", s)),
        //        Double.Parse(String.Format("{0:0.##}", l)));
    }
    /// <summary>
    /// Converts RGB to CMYK.
    /// </summary>
    /// <param name="red">Red vaue must be in [0, 255]. </param>
    /// <param name="green">Green vaue must be in [0, 255].</param>
    /// <param name="blue">Blue vaue must be in [0, 255].</param>
    public static CMYK RGBtoCMYK(int red, int green, int blue)
    {
        // normalizes red, green, blue values
        double c = (double)(255 - red) / 255;
        double m = (double)(255 - green) / 255;
        double y = (double)(255 - blue) / 255;

        double k = (double)Math.Min(c, Math.Min(m, y));

        if (k == 1.0)
        {
            return new CMYK(0, 0, 0, 1);
        }
        else
        {
            return new CMYK((c - k) / (1 - k), (m - k) / (1 - k), (y - k) / (1 - k), k);
        }
    }

    /// <summary>
    /// Converts RGB to YUV.
    /// </summary>
    /// <param name="red">Red must be in [0, 255].</param>
    /// <param name="green">Green must be in [0, 255].</param>
    /// <param name="blue">Blue must be in [0, 255].</param>
    public static YUV RGBtoYUV(int red, int green, int blue)
    {
        YUV yuv = new YUV();

        // normalizes red, green, blue values
        double r = (double)red / 255.0;
        double g = (double)green / 255.0;
        double b = (double)blue / 255.0;

        yuv.Y = 0.299 * r + 0.587 * g + 0.114 * b;
        yuv.U = -0.14713 * r - 0.28886 * g + 0.436 * b;
        yuv.V = 0.615 * r - 0.51499 * g - 0.10001 * b;

        return yuv;
    }

    /// <summary>
    /// Converts a RGB color format to an hexadecimal color.
    /// </summary>
    /// <param name="r">Red value.</param>
    /// <param name="g">Green value.</param>
    /// <param name="b">Blue value.</param>
    public static string RGBToHex(int r, int g, int b)
    {
        return String.Format("#{0:x2}{1:x2}{2:x2}", r, g, b).ToUpper();
    }
    /// <summary>
    /// Converts RGB to CIE XYZ (CIE 1931 color space)
    /// </summary>
    public static CIEXYZ RGBtoXYZ(int red, int green, int blue)
    {
        // normalize red, green, blue values
        double rLinear = (double)red / 255.0;
        double gLinear = (double)green / 255.0;
        double bLinear = (double)blue / 255.0;

        // convert to a sRGB form
        double r = (rLinear > 0.04045) ? Math.Pow((rLinear + 0.055) / (
            1 + 0.055), 2.2) : (rLinear / 12.92);
        double g = (gLinear > 0.04045) ? Math.Pow((gLinear + 0.055) / (
            1 + 0.055), 2.2) : (gLinear / 12.92);
        double b = (bLinear > 0.04045) ? Math.Pow((bLinear + 0.055) / (
            1 + 0.055), 2.2) : (bLinear / 12.92);

        // converts
        return new CIEXYZ(
            (r * 0.4124 + g * 0.3576 + b * 0.1805),
            (r * 0.2126 + g * 0.7152 + b * 0.0722),
            (r * 0.0193 + g * 0.1192 + b * 0.9505)
            );
    }
    /// <summary>
    /// Converts RGB to CIELab.
    /// </summary>
    public static CIELab RGBtoLab(int red, int green, int blue)
    {
        var xyz = RGBtoXYZ(red, green, blue);
        return XYZtoLab(xyz.X, xyz.Y, xyz.Z);
    }
    /// <summary>
    /// Converts HSB to RGB.
    /// </summary>
    public static RGB HSBtoRGB(double h, double s, double _b)
    {
        double r = 0;
        double g = 0;
        double b = 0;

        if (s == 0)
        {
            r = g = b = _b;
        }
        else
        {
            // the color wheel consists of 6 sectors. Figure out which sector
            // you're in.
            double sectorPos = h / 60.0;
            int sectorNumber = (int)(Math.Floor(sectorPos));
            // get the fractional part of the sector
            double fractionalSector = sectorPos - sectorNumber;

            // calculate values for the three axes of the color.
            double p = b * (1.0 - s);
            double q = b * (1.0 - (s * fractionalSector));
            double t = b * (1.0 - (s * (1 - fractionalSector)));

            // assign the fractional colors to r, g, and b based on the sector
            // the angle is in.
            switch (sectorNumber)
            {
                case 0:
                    r = b;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = b;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = b;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = _b;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = _b;
                    break;
                case 5:
                    r = b;
                    g = p;
                    b = q;
                    break;
            }
        }

        return new RGB((int)(Math.Round(r * 255.0, 2)), (int)(Math.Round(g * 255.0, 2)), (int)(Math.Round(b * 255.0, 2)));
        //Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", r * 255.0))),
        //Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", g * 255.0))),
        //Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", b * 255.0)))
    }
    /// <summary>
    /// Converts HSL to HSB.
    /// </summary>
    public static HSB HSLtoHSB(double h, double s, double l)
    {
        RGB rgb = HSLtoRGB(h, s, l);

        return RGBtoHSB(rgb.Red, rgb.Green, rgb.Blue);
    }
    /// <summary>
    /// Converts HSB to CMYK.
    /// </summary>
    public static CMYK HSBtoCMYK(double h, double s, double b)
    {
        RGB rgb = HSBtoRGB(h, s, b);

        return RGBtoCMYK(rgb.Red, rgb.Green, rgb.Blue);
    }
    /// <summary>
    /// Converts HSB to CMYK.
    /// </summary>
    public static YUV HSBtoYUV(double h, double s, double b)
    {
        RGB rgb = HSBtoRGB(h, s, b);

        return RGBtoYUV(rgb.Red, rgb.Green, rgb.Blue);
    }
    /// <summary>
    /// Converts HSL to RGB.
    /// </summary>
    /// <param name="h">Hue, must be in [0, 360].</param>
    /// <param name="s">Saturation, must be in [0, 1].</param>
    /// <param name="l">Luminance, must be in [0, 1].</param>
    public static RGB HSLtoRGB(double h, double s, double l)
    {
        if (s == 0)
        {
            // achromatic color (gray scale)
            return new RGB((int)Math.Round(l * 255.0, 2), (int)Math.Round(l * 255.0, 2), (int)Math.Round(l * 255.0, 2));
            //Convert.ToInt32(Double.Parse(String.Format("{0:0.00}",
            //    l * 255.0))),
            //Convert.ToInt32(Double.Parse(String.Format("{0:0.00}",
            //    l * 255.0))),
            //Convert.ToInt32(Double.Parse(String.Format("{0:0.00}",
            //    l * 255.0)))
            //);
        }
        else
        {
            double q = (l < 0.5) ? (l * (1.0 + s)) : (l + s - (l * s));
            double p = (2.0 * l) - q;

            double Hk = h / 360.0;
            double[] T = new double[3];
            T[0] = Hk + (1.0 / 3.0);    // Tr
            T[1] = Hk;                // Tb
            T[2] = Hk - (1.0 / 3.0);    // Tg

            for (int i = 0; i < 3; i++)
            {
                if (T[i] < 0) T[i] += 1.0;
                if (T[i] > 1) T[i] -= 1.0;

                if ((T[i] * 6) < 1)
                {
                    T[i] = p + ((q - p) * 6.0 * T[i]);
                }
                else if ((T[i] * 2.0) < 1) //(1.0/6.0)<=T[i] && T[i]<0.5
                {
                    T[i] = q;
                }
                else if ((T[i] * 3.0) < 2) // 0.5<=T[i] && T[i]<(2.0/3.0)
                {
                    T[i] = p + (q - p) * ((2.0 / 3.0) - T[i]) * 6.0;
                }
                else T[i] = p;
            }
            return new RGB((int)Math.Round(T[0] * 255.0, 2), (int)Math.Round(T[1] * 255.0, 2), (int)Math.Round(T[2] * 255.0, 2));
        }
    }
    /// <summary>
    /// Converts HSL to CMYK.
    /// </summary>
    public static CMYK HSLtoCMYK(double h, double s, double l)
    {
        RGB rgb = HSLtoRGB(h, s, l);

        return RGBtoCMYK(rgb.Red, rgb.Green, rgb.Blue);
    }
    /// <summary>
    /// Converts HSL to YUV.
    /// </summary>
    public static YUV HSLtoYUV(double h, double s, double l)
    {
        RGB rgb = HSLtoRGB(h, s, l);

        return RGBtoYUV(rgb.Red, rgb.Green, rgb.Blue);
    }
    /// <summary>
    /// Converts CMYK to RGB.
    /// </summary>
    public static Color CMYKtoRGB(double c, double m, double y, double k)
    {
        int red = (int)((1 - c) * (1 - k) * 255.0);
        int green = (int)((1 - m) * (1 - k) * 255.0);
        int blue = (int)((1 - y) * (1 - k) * 255.0);

        return Color.FromArgb(red, green, blue);
    }
    /// <summary>
    /// Converts CMYK to HSL.
    /// </summary>
    public static HSL CMYKtoHSL(double c, double m, double y, double k)
    {
        var rgb = CMYKtoRGB(c, m, y, k);

        return RGBtoHSL(rgb.R, rgb.G, rgb.B);
    }
    /// <summary>
    /// Converts CMYK to HSB.
    /// </summary>
    public static HSB CMYKtoHSB(double c, double m, double y, double k)
    {
        var rgb = CMYKtoRGB(c, m, y, k);

        return RGBtoHSB(rgb.R, rgb.G, rgb.B);
    }
    /// <summary>
    /// Converts CMYK to YUV.
    /// </summary>
    public static YUV CMYKtoYUV(double c, double m, double y, double k)
    {
        var rgb = CMYKtoRGB(c, m, y, k);

        return RGBtoYUV(rgb.R, rgb.G, rgb.B);
    }
    /// <summary>
    /// Converts YUV to RGB.
    /// </summary>
    /// <param name="y">Y must be in [0, 1].</param>
    /// <param name="u">U must be in [-0.436, +0.436].</param>
    /// <param name="v">V must be in [-0.615, +0.615].</param>
    public static RGB YUVtoRGB(double y, double u, double v)
    {
        RGB rgb = new RGB();

        rgb.Red = (int)((y + 1.139837398373983740 * v) * 255);
        rgb.Green = (int)((
            y - 0.3946517043589703515 * u - 0.5805986066674976801 * v) * 255);
        rgb.Blue = (int)((y + 2.032110091743119266 * u) * 255);

        return rgb;
    }
    /// <summary>
    /// Converts YUV to HSL.
    /// </summary>
    /// <param name="y">Y must be in [0, 1].</param>
    /// <param name="u">U must be in [-0.436, +0.436].</param>
    /// <param name="v">V must be in [-0.615, +0.615].</param>
    public static HSL YUVtoHSL(double y, double u, double v)
    {
        RGB rgb = YUVtoRGB(y, u, v);

        return RGBtoHSL(rgb.Red, rgb.Green, rgb.Blue);
    }
    /// <summary>
    /// Converts YUV to HSB.
    /// </summary>
    /// <param name="y">Y must be in [0, 1].</param>
    /// <param name="u">U must be in [-0.436, +0.436].</param>
    /// <param name="v">V must be in [-0.615, +0.615].</param>
    public static HSB YUVtoHSB(double y, double u, double v)
    {
        RGB rgb = YUVtoRGB(y, u, v);

        return RGBtoHSB(rgb.Red, rgb.Green, rgb.Blue);
    }
    /// <summary>
    /// Converts YUV to CMYK.
    /// </summary>
    /// <param name="y">Y must be in [0, 1].</param>
    /// <param name="u">U must be in [-0.436, +0.436].</param>
    /// <param name="v">V must be in [-0.615, +0.615].</param>
    public static CMYK YUVtoCMYK(double y, double u, double v)
    {
        RGB rgb = YUVtoRGB(y, u, v);

        return RGBtoCMYK(rgb.Red, rgb.Green, rgb.Blue);
    }
    /// <summary>
    /// Converts CIEXYZ to RGB structure.
    /// </summary>
    public static RGB XYZtoRGB(double x, double y, double z)
    {
        double[] Clinear = new double[3];
        Clinear[0] = x * 3.2410 - y * 1.5374 - z * 0.4986; // red
        Clinear[1] = -x * 0.9692 + y * 1.8760 - z * 0.0416; // green
        Clinear[2] = x * 0.0556 - y * 0.2040 + z * 1.0570; // blue

        for (int i = 0; i < 3; i++)
        {
            Clinear[i] = (Clinear[i] <= 0.0031308) ? 12.92 * Clinear[i] : (
                1 + 0.055) * Math.Pow(Clinear[i], (1.0 / 2.4)) - 0.055;
        }

        return new RGB(
            (int)Math.Round(Clinear[0] * 255.0, 2),
            (int)Math.Round(Clinear[1] * 255.0, 2),
            (int)Math.Round(Clinear[2] * 255.0, 2));
    }
    /// <summary>
    /// XYZ to L*a*b* transformation function.
    /// </summary>
    private static double Fxyz(double t)
    {
        return ((t > 0.008856) ? Math.Pow(t, (1.0 / 3.0)) : (7.787 * t + 16.0 / 116.0));
    }

    /// <summary>
    /// Converts CIEXYZ to CIELab.
    /// </summary>
    public static CIELab XYZtoLab(double x, double y, double z)
    {
        CIELab lab = CIELab.Empty;

        lab.L = 116.0 * Fxyz(y / CIEXYZ.D65.Y) - 16;
        lab.A = 500.0 * (Fxyz(x / CIEXYZ.D65.X) - Fxyz(y / CIEXYZ.D65.Y));
        lab.B = 200.0 * (Fxyz(y / CIEXYZ.D65.Y) - Fxyz(z / CIEXYZ.D65.Z));

        return lab;
    }
    /// <summary>
    /// Converts CIELab to CIEXYZ.
    /// </summary>
    public static CIEXYZ LabtoXYZ(double l, double a, double b)
    {
        double delta = 6.0 / 29.0;

        double fy = (l + 16) / 116.0;
        double fx = fy + (a / 500.0);
        double fz = fy - (b / 200.0);

        return new CIEXYZ(
            (fx > delta) ? CIEXYZ.D65.X * (fx * fx * fx) : (fx - 16.0 / 116.0) * 3 * (
                delta * delta) * CIEXYZ.D65.X,
            (fy > delta) ? CIEXYZ.D65.Y * (fy * fy * fy) : (fy - 16.0 / 116.0) * 3 * (
                delta * delta) * CIEXYZ.D65.Y,
            (fz > delta) ? CIEXYZ.D65.Z * (fz * fz * fz) : (fz - 16.0 / 116.0) * 3 * (
                delta * delta) * CIEXYZ.D65.Z
            );
    }
    /// <summary>
    /// Converts CIELab to RGB.
    /// </summary>
    public static RGB LabtoRGB(double l, double a, double b)
    {
        var xyz = LabtoXYZ(l, a, b);
        return XYZtoRGB(xyz.X, xyz.Y, xyz.Z);
    }
    public static double MaxColorDistance = GetColorDistance(new (255, 255, 255), new (0, 0, 0));
    /// <summary>
    /// Best formular as L*a*b works
    /// </summary>
    /// <param name="e1"></param>
    /// <param name="e2"></param>
    /// <returns></returns>
    public static double GetColorDistance(RGB e1, RGB e2)
    {
        int rmean = (e1.Red + e2.Red) >>1;
        int r = e1.Red - e2.Red;
        int g = e1.Green - e2.Green;
        int b = e1.Blue - e2.Blue;
        return Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8));
    }
    public static double GetColorDistance(Color c1,Color c2, double? MaxDistance = null)
    {
        MaxDistance ??= MaxColorDistance;
        int rmean = (c1.R + c2.R) >> 1;
        int r = c1.R - c2.R;
        int g = c1.G - c2.G;
        int b = c1.B - c2.B;
        return Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8))
            / MaxDistance.GetValueOrDefault();
    }

    public static void RangeTest()
    {
        int[] values = new int[] { 0, 255 };

        RGB e1 = new RGB();
        RGB e2 = new RGB();
        var results = new List<double>();

        for (int i1 = 0; i1 < values.Length; i1++)
        {
            e1.Red = values[i1];
            for (int j1 = 0; j1 < values.Length; j1++)
            {
                e1.Green = values[j1];
                for (int k1 = 0; k1 < values.Length; k1++)
                {
                    e1.Blue = values[k1];

                    for (int i2 = 0; i2 < values.Length; i2++)
                    {
                        e2.Red = values[i2];
                        for (int j2 = 0; j2 < values.Length; j2++)
                        {
                            e2.Green = values[j2];
                            for (int k2 = 0; k2 < values.Length; k2++)
                            {
                                e2.Blue = values[k2];

                                double d = GetColorDistance(e1, e2);
                                results.Add(d);
                                Console.WriteLine($"{e1}-{e2}={d}");
                            }
                        }
                    }

                }
            }
        }
        Console.WriteLine($"Max:{results.Max()}");
        Console.WriteLine($"Min:{results.Min()}");
    }
}
