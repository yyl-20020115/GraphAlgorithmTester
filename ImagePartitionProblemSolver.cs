using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GraphAlgorithmTester;

public class ImagePartitionProblemSolver : ProblemSolver
{
    public struct Point
    {
        public int X;
        public int Y;
        public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Point a, Point b) => !(a == b);
        public Point(long V)
        {
            this.Y = (int)(V >> 32);
            this.X = (int)(V & ~0);
        }
        public Point(int X = 0, int Y = 0)
        {
            this.X = X;
            this.Y = Y;
        }
        public static implicit operator long(Point p) => (((long)p.Y << 32) | (long)p.X);
        public static implicit operator Point(long v) => new (v);
        public override string ToString() => $"({X},{Y})";
        public bool IsValid(int X0, int Y0, int W, int H) => this.X >= X0 && this.Y >= Y0 && this.X < X0 + W && this.Y < Y0 + H;
        public override bool Equals([NotNullWhen(true)] object obj) => obj is Point p && this == p;
        public override int GetHashCode() => this.X ^ this.Y;
        public bool IsNextTo(Region region) => this.IsNextTo(region.Points);
        public bool IsNextTo(HashSet<Point> points)
        {
            var x = this.X;
            var y = this.Y;
            return points.Any(p => p.X >= x - 1 && p.X <= x + 1 && p.Y >= y - 1 && p.Y <= y + 1);
        }
    }
    public record Region(Color Color, Point First, HashSet<Point> Points)
    {
        public int Area => this.Points.Count;
        public HashSet<Point> Points { init; get; } = new();
        public Path GetBorderPath()
        {
            if (this.Points.Count == 0) return new Path(this.Color, new());

            int xmin = this.Points.Min(p => p.X);
            int xmax = this.Points.Max(p => p.X);
            int ymin = this.Points.Min(p => p.Y);
            int ymax = this.Points.Max(p => p.Y);
            int xmid = (xmin + xmax) >> 1;
            int ymid = (ymin + ymax) >> 1;
            var points = new List<Point>();

            for (int i = xmin; i <= xmax; i++)
            {
                if (i == xmin)
                {
                    points.AddRange(this.Points.Where(p => p.X == i).OrderByDescending(p => p.Y));
                }
                else if (i == xmax)
                {
                    points.AddRange(this.Points.Where(p => p.X == i).OrderBy(p => p.Y));
                }
                else
                {
                    var ls = this.Points.Where(p => p.X == i).ToList();
                    if (ls.Count > 0)
                    {
                        var ms = ls.Min(p => p.Y);
                        points.AddRange(ls.Where(p => p.Y == ms));

                    }
                }
            }
            for (int i = xmax; i >= xmin; i--)
            {
                if (i == xmax)
                {
                    points.AddRange(this.Points.Where(p => p.X == i).OrderBy(p => p.Y));

                }
                else if (i == xmin)
                {
                    points.AddRange(this.Points.Where(p => p.X == i).OrderByDescending(p => p.Y));
                }
                else
                {
                    var ls = this.Points.Where(p => p.X == i).ToList();
                    if (ls.Count > 0)
                    {
                        var ms = ls.Max(p => p.Y);
                        points.AddRange(ls.Where(p => p.Y == ms));
                    }
                }
            }
            return new Path(this.Color, points.Distinct().ToList());
        }
    }
    public record Path(Color Color, List<Point> Points)
    {
        public Color InvertColor
            => Color.FromArgb(
                (byte)~this.Color.R,
                (byte)~this.Color.G,
                (byte)~this.Color.B
                );
    }

    public bool CompactRegions(Color color, ICollection<Region> regions, List<Region> compact)
    {
        var rs = regions.ToList();
        //rc has same color
        var rc = rs.Where(r => r.Color == color).ToList();
        if (rc.Count > 0)
        {
            var rc0 = rc[0];
            var any = false;
            do
            {
                any = false;
                for (int i = 1; i < rc.Count; i++)
                {
                    if (rc0.Points.Overlaps(rc[i].Points))
                    {
                        any = true;
                        rc0.Points.UnionWith(rc[i].Points);
                        rc.Remove(rc[i]);
                    }
                }
            } while (any);
            compact.AddRange(rc.Where(r => r.Points.Count > 0));
        }
        return rc.Count > 0;
    }

    public void SolveBlock(DirectBitmap bitmap, HashLookups<Color, Region> colregions, int x0, int y0, int w, int h, bool tailx=false, bool taily = false)
    {
        if (tailx)
        {
            w = bitmap.Width - x0;
        }
        if (taily)
        {
            h = bitmap.Height - y0;
        }

        var visiting = new Dictionary<long, Region>();
        var trace = new HashLookups<long, long>();

        var p0 = new Point(x0 + (w >> 1), y0 + (h >> 1));
        var r0 = new Region(bitmap.GetPixel(p0.X, p0.Y), p0, new());
        if (!visiting.ContainsKey(p0))
        {
            visiting[p0] = r0;
            colregions.Add(r0.Color, r0);
        }

        while (visiting.Count > 0)
        {
            var ps = visiting.ToArray();
            visiting.Clear();

            foreach (var r in ps)
            {
                var p = new Point(r.Key);
                var cps = new Point[]
                {
                    new Point(p.X-1,p.Y-1),
                    new Point(p.X,p.Y-1),
                    new Point(p.X+1,p.Y-1),
                    new Point(p.X+1,p.Y),
                    new Point(p.X+1,p.Y+1),
                    new Point(p.X,p.Y+1),
                    new Point(p.X-1,p.Y+1),
                    new Point(p.X-1,p.Y),
                };
                for (int d = 0; d < cps.Length; d++)
                {
                    var cp = cps[d];
                    if (cp.IsValid(x0, y0, w, h)
                        && !visiting.ContainsKey(cp)) 
                    {
                        //NOTICE: this is the trick to minimize cost of the pixels
                        //Check if this route has been gone through
                        if (trace.TryGetValue(cp, out HashSet<long> sp) && sp.Contains(p))
                            continue;
                        trace.Add(cp, p);
                        visiting.Add(cp, r.Value);
                    }
                }
            }
            var ch = false;
            foreach (var r in visiting.ToArray())
            {
                var cp = new Point(r.Key);
                var Color = bitmap.GetPixel(cp.X, cp.Y);
                if (colregions.TryGetValue(Color, out HashSet<Region> sets))
                {
                    if (sets.Contains(r.Value))
                    {
                        ch |= r.Value.Points.Add(cp);
                    }
                    else
                    {
                        var fr = sets.Where(s => cp.IsNextTo(s)).FirstOrDefault();
                        if (fr != null)
                        {
                            ch |= fr.Points.Add(cp);
                            visiting[cp] = fr;
                        }
                        else
                        {
                            ch = true;
                            sets.Add(visiting[cp] = new (Color, cp, new()));
                        }
                    }
                }
                else //no such color
                {
                    ch |= true;
                    colregions.Add(Color, visiting[cp] = new (Color, cp, new() { cp }));
                }
            }
            if (!ch) break;
        }
    }

    protected (int ws, int hs) Factor(int count)
    {
        int r = (int)Math.Sqrt(count);
        for (int i = r; i >= 2; i--)
        {
            if (count % i == 0) return (count / i, i);
        }
        return (count, 1);
    }
    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        var fn = "Partition.png";
        using var bitmap = Bitmap.FromFile(fn) as Bitmap;

        int w = bitmap.Width;
        int h = bitmap.Height;

        if (w == 0 || h == 0)
        {

            return;
        }

        using var direct = new DirectBitmap(w, h);

        using (var g = Graphics.FromImage(direct.Bitmap))
        {
            g.DrawImage(bitmap, new System.Drawing.Point(0, 0));
        }

        var colregions = new HashLookups<Color, Region>();
        var sw = new Stopwatch();
        sw.Start();

        int cpus = 6;

        int delta = w / cpus;

        var subs = new List<HashLookups<Color, Region>>();
        var segments = new List<(HashLookups<Color, Region> sub ,int x0,int w0,int i)>();
        for(int i = 0; i < cpus; i++)
        {
            var sub = new HashLookups<Color, Region>();
            subs.Add(sub);
            int x0 = (i+0) * delta;
            int x1 = (i+1) * delta;
            if (x0 > 0)
            {
                x0--;
            }
            int w0 = x1 - x0;
            segments.Add((sub, x0, w0, i)); 
        }
        segments.AsParallel().ForAll(
            (a) => this.SolveBlock(direct, a.sub, a.x0, 0, a.w0, h, a.i == cpus - 1));

        foreach(var sub in subs)
        {
            colregions.AddRange(sub);
        }

        var regions = new List<Region>();
        foreach (var ctr in colregions)
        {
            this.CompactRegions(ctr.Key, ctr.Value, regions);
        }
        sw.Stop();
        System.Diagnostics.Debug.WriteLine($"Used time:{sw.Elapsed}");

        if (regions.Count > 0)
        {
            using var g2 = Graphics.FromImage(bitmap);
            //here we get all regions
            foreach (var region in regions)
            {
                var path = region.GetBorderPath();
                var pen = new Pen(Color.White, 2.0f);
                //foreach (var p in region.Points)
                //{
                //    bitmap.SetPixel(p.X, p.Y, path.InvertColor);
                //}
                if (path.Points.Count > 0)
                {
                    var pt = default(Point);
                    var p0 = path.Points[0];
                    for (int i = 1; i < path.Points.Count; i++)
                    {
                        pt = path.Points[i];
                        g2.DrawLine(pen, p0.X, p0.Y, pt.X, pt.Y);
                        p0 = pt;
                    }

                    pt = path.Points[0];
                    g2.DrawLine(pen, p0.X, p0.Y, pt.X, pt.Y);
                }

            }
            bitmap.Save(System.IO.Path.ChangeExtension(fn, "-with-regions.png"));
        }
    }
}
