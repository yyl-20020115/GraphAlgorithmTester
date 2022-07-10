using System;
using System.Collections.Generic;
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
            this.Y = (int)(V >> 32);
            this.X = (int)(V & ~0);
        }
        public Point(int X = 0, int Y = 0)
        {
            this.X = X;
            this.Y = Y;
        }
        public static implicit operator long(Point p) => (((long)p.Y << 32) | (long)p.X);
        public static implicit operator Point(long v)=>new Point(v);
        public override string ToString() => $"({X},{Y})";
        public bool IsValid(int W, int H) => this.X >= 0 && this.Y >= 0 && this.X < W && this.Y < H;
        public override bool Equals([NotNullWhen(true)] object obj) => obj is Point p && this == p;
        public override int GetHashCode() => this.X ^ this.Y;
        public bool IsNextTo(Region region)
        {
            var x = this.X;
            var y = this.Y;
            return region.Points.Any(p => p.X >= x - 1 && p.X <= x + 1 && p.Y >= y - 1 && p.Y <= y + 1);
        }
    }
    public record Region(Color Color,Point First, HashSet<Point> Points)
    {
        public int Area => this.Points.Count;
        public HashSet<Point> Points { init; get; } = new();
        public Path GetBorderPath()
        {
            int xmin = this.Points.Min(p => p.X);
            int xmax = this.Points.Max(p => p.X);
            int ymin = this.Points.Min(p => p.Y);
            int ymax = this.Points.Max(p => p.Y);
            int xmid = (xmin + xmax) >> 1;
            int ymid = (ymin + ymax) >> 1;
            var points = new List<Point>();
            
            for(int i = xmin; i <= xmax; i++)
            {
                if(i == xmin)
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
                    var ms = ls.Min(p => p.Y);
                    points.AddRange(ls.Where(p=>p.Y==ms));
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
                    var ms = ls.Max(p => p.Y);
                    points.AddRange(ls.Where(p => p.Y == ms));
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

    public bool CompactRegions(ICollection<Region> regions, List<Region> compact)
    {
        var cs = regions.Select(r => r.Color).ToHashSet();
        if (cs.Count != 1) return false;

        var fs = cs.Single();
        var rs = regions.ToList();
        //rc has same color
        var rc = rs.Where(r => r.Color == fs).ToList();
        if (rc.Count > 0)
        {
            for (int t = 0; t < rc.Count; t++)
            {
                for (int i = 1; i < rc.Count; i++)
                {
                    //combine regions
                    if (rc[i].Points.Overlaps(rc[t].Points))
                    {
                        rc[t].Points.UnionWith(rc[i].Points);
                        rc[i].Points.Clear();
                    }
                }
            }
            compact.AddRange(rc.Where(c => c.Points.Count > 0));
        }
        return rc.Count > 0;
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
        //middle point
        int x0 = (w >> 1);
        int y0 = (h >> 1);
        var colregions = new HashLookups<Color, Region>();        
        var visiting = new Dictionary<long, Region>() { [new Point(x0,y0)]= new Region(bitmap.GetPixel(x0,y0),new(x0,y0),new()) };
        var last = new HashSet<long>();
        colregions.Add(visiting.First().Value.Color, visiting.First().Value);
        int area = w * h;
        while (visiting.Count>0)
        {
            var ps = visiting.ToArray();
            last = visiting.Keys.ToHashSet();
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
                for (int d = 0;d<cps.Length ; d++)
                {
                    var cp = cps[d];
                    if ( cp.IsValid(w,h) && cp != r.Key && !visiting.ContainsKey(cp) && 
                        !last.Contains(cp))
                    {
                        visiting.Add(cp, r.Value);
                    }
                }
                //for (int dy = -1; dy <= +1; dy++)
                //{
                //    int y = r.Key.Y + dy;
                //    y = y < 0 ? 0 : y;
                //    y = y >= h ? h - 1 : y;

                //    for (int dx = -1; dx <= +1; dx++)
                //    {
                //        int x = r.Key.X + dx;
                //        x = x < 0 ? 0 : x;
                //        x = x >= w ? w - 1 : x;
                //    }
                //}
            }
            var ch = false;
            foreach(var r in visiting.ToArray())
            {
                var cp =new Point(r.Key);
                var Color = bitmap.GetPixel(cp.X, cp.Y);
                if (colregions.TryGetValue(Color, out HashSet<Region> rs))
                {
                    if (rs.Contains(r.Value))
                    {
                        ch |= r.Value.Points.Add(cp);
                    }
                    else
                    {
                        var fr = rs.Where(s => cp.IsNextTo(s)).FirstOrDefault();
                        if(fr!=null)
                        {
                            ch |= fr.Points.Add(cp);
                            visiting[cp] = fr;
                        }
                        else
                        {
                            ch = true;
                            rs.Add(visiting[cp] = new Region(Color, cp, new()));
                        }
                    }
                }
                else //no such color
                {
                    ch |= true;
                    colregions.Add(Color, visiting[cp] = new Region(Color, cp, new() { cp }));
                }
            }
            if (!ch) break;
        }

        var regions = new List<Region>();
        foreach(var ctr in colregions)
        {
            this.CompactRegions(ctr.Value, regions);
        }

        if (regions.Count > 0)
        {
            using var g = Graphics.FromImage(bitmap);
            //here we get all regions
            foreach (var region in regions)
            {
                var path = region.GetBorderPath();
                var pen = new Pen(Color.White,2.0f);
                //foreach(var p in region.Points)
                //{
                //    bitmap.SetPixel(p.X, p.Y,path.InvertColor);
                //}
                if (path.Points.Count > 0)
                {
                    var pt = default(Point);
                    var p0 = path.Points[0];
                    for(int i = 1; i < path.Points.Count; i++)
                    {
                        pt = path.Points[i];
                        g.DrawLine(pen, p0.X, p0.Y, pt.X, pt.Y);
                        p0 = pt;
                    }

                    pt = path.Points[0];
                    g.DrawLine(pen, p0.X, p0.Y, pt.X, pt.Y);
                }

            }
            bitmap.Save(System.IO.Path.ChangeExtension(fn, "-with-regions.png"));
        }
    }
}
