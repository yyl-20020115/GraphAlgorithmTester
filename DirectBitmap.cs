using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphAlgorithmTester;

public class DirectBitmap : IDisposable
{
    public readonly Bitmap Bitmap;
    protected readonly int[] Bits;
    public readonly int Height;
    public readonly int Width;

    protected GCHandle BitsHandle;

    public DirectBitmap(int width, int height)
    {
        this.Width = width;
        this.Height = height;
        this.Bits = new int[width * height];
        this.BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
        this.Bitmap = new Bitmap(width, height, width * 4,
            PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
    }

    public void SetPixel(int x, int y, Color colour)
    {
        SetPixelRGB(x, y, colour.ToArgb());
    }

    public void SetPixelRGB(int x, int y, int colour)
    {
        Bits[x + (y * Width)] = colour;
    }

    public Color GetPixel(int x, int y)
    {
        return Color.FromArgb(GetPixelRGB(x, y));
    }

    public int GetPixelRGB(int x, int y)
    {
        return Bits[x + (y * Width)];
    }

    private bool Disposed = false;

    public void Dispose()
    {
        if (Disposed) return;
        Disposed = true;
        Bitmap.Dispose();
        BitsHandle.Free();
    }
}
