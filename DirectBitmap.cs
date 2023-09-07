using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

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
#pragma warning disable CA1416 // 验证平台兼容性
        this.Bitmap = new Bitmap(width, height, width * 4,
            PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
#pragma warning restore CA1416 // 验证平台兼容性
    }

    public void SetPixel(int x, int y, Color color) => SetPixelRGB(x, y, color.ToArgb());

    public void SetPixelRGB(int x, int y, int color) => Bits[x + (y * Width)] = color;

    public Color GetPixel(int x, int y) => Color.FromArgb(GetPixelRGB(x, y));

    public int GetPixelRGB(int x, int y) => Bits[x + (y * Width)];

    private bool Disposed = false;

    public void Dispose()
    {
        if (Disposed) return;
        Disposed = true;
#pragma warning disable CA1416 // 验证平台兼容性
        Bitmap.Dispose();
#pragma warning restore CA1416 // 验证平台兼容性
        BitsHandle.Free();
    }
}
