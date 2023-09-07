using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;

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
            this.Y = GetY(V);
            this.X = GetX(V);
        }
        public static int GetX(long V) => (int)(V & ~0);
        public static int GetY(long V) => (int)(V >> 32);
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
        public bool IsNextTo(HashSet<long> points)
        {
            var x = this.X;
            var y = this.Y;
            return points.Any(
              p => Point.GetX(p) >= x - 1 
                && Point.GetX(p) <= x + 1 
                && Point.GetY(p) >= y - 1 
                && Point.GetY(p) <= y + 1
                );
        }
    }
    public record Region(Color Color, long First, HashSet<long> Points)
    {
        public int Area => this.Points.Count;
        public HashSet<long> Points { init; get; } = new();
        public Path GetBorderPath()
        {
            if (this.Points.Count == 0) return new Path(this.Color, new());

            int xmin = this.Points.Min(p => Point.GetX(p));
            int xmax = this.Points.Max(p => Point.GetX(p));
            int ymin = this.Points.Min(p => Point.GetY(p));
            int ymax = this.Points.Max(p => Point.GetY(p));
            int xmid = (xmin + xmax) >> 1;
            int ymid = (ymin + ymax) >> 1;
            var points = new List<long>();

            for (int i = xmin; i <= xmax; i++)
            {
                if (i == xmin)
                {
                    points.AddRange(this.Points.Where(p => Point.GetX(p) == i).OrderByDescending(p => Point.GetY(p)));
                }
                else if (i == xmax)
                {
                    points.AddRange(this.Points.Where(p => Point.GetX(p) == i).OrderBy(p => Point.GetY(p)));
                }
                else
                {
                    var ls = this.Points.Where(p => Point.GetX(p) == i).ToList();
                    if (ls.Count > 0)
                    {
                        var ms = ls.Min(p => Point.GetY(p));
                        points.AddRange(ls.Where(p => (Point.GetY(p) == ms)));

                    }
                }
            }
            for (int i = xmax; i >= xmin; i--)
            {
                if (i == xmax)
                {
                    points.AddRange(this.Points.Where(p => Point.GetX(p) == i).OrderBy(p => Point.GetY(p)));
                }
                else if (i == xmin)
                {
                    points.AddRange(this.Points.Where(p => Point.GetX(p) == i).OrderByDescending(p => Point.GetY(p)));
                }
                else
                {
                    var ls = this.Points.Where(p => Point.GetX(p) == i).ToList();
                    if (ls.Count > 0)
                    {
                        var ms = ls.Max(p => Point.GetY(p));
                        points.AddRange(ls.Where(p => Point.GetY(p) == ms));
                    }
                }
            }
            return new Path(this.Color, points.Distinct().ToList());
        }
    }
    public record Path(Color Color, List<long> Points)
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
                    new (p.X-1,p.Y-1),
                    new (p.X,p.Y-1),
                    new (p.X+1,p.Y-1),
                    new (p.X+1,p.Y),
                    new (p.X+1,p.Y+1),
                    new (p.X,p.Y+1),
                    new (p.X-1,p.Y+1),
                    new (p.X-1,p.Y),
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
                            if (ch |= fr.Points.Add(cp))
                            {
                                visiting[cp] = fr;
                            }
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

    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        var fn = "Partition.png";
        using var bitmap = Bitmap.FromFile(fn) as Bitmap;

        int w = bitmap.Width;
        int h = bitmap.Height;

        if (w == 0 || h == 0) return;

        using var direct_bitmap = new DirectBitmap(w, h);
        using (var g = Graphics.FromImage(direct_bitmap.Bitmap))
        {
            g.DrawImage(bitmap, new System.Drawing.Point(0, 0));
        }

        var colregions = new HashLookups<Color, Region>();
        var sw = new Stopwatch();
        sw.Start();

        int cpus = 1;// Environment.ProcessorCount;

        int delta = w / cpus;
        
        var subs = new List<HashLookups<Color, Region>>();
        var segments = new List<(HashLookups<Color, Region> sub ,int x0,int w0,int i)>();
        for(int i = 0; i < cpus; i++)
        {
            var sub = new HashLookups<Color, Region>();
            subs.Add(sub);
            int x0 = (i+0) * delta;
            int x1 = (i+1) * delta;
            x0 = x0 > 0 ? x0 - 1 : 0;
            int w0 = x1 - x0;
            segments.Add((sub, x0, w0, i)); 
        }
        segments.AsParallel().ForAll(
            (a) => this.SolveBlock(direct_bitmap, a.sub, a.x0, 0, a.w0, h, a.i == cpus - 1));

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
            //draw the region borders
            foreach (var region in regions)
            {
                var path = region.GetBorderPath();
                var pen = new Pen(Color.White, 1.0f);

                if (path.Points.Count > 0)
                {
                    var pt = default(Point);
                    var p0 = (Point)path.Points[0];
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
