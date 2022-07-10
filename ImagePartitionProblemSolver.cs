using System.Collections.Generic;
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
        public Point(int X = 0,int Y = 0)
        {
            this.X = X;
            this.Y = Y;
        }
        public bool IsWithin(Region region)
            => this.X >= region.X0 - 1 
            && this.X <= region.X0 + 1
            && this.Y >= region.Y0 - 1 
            && this.Y <= region.Y0 + 1
            ;
    }
    public record Region(Color Color,int X0 ,int Y0, HashSet<Point> Points)
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
                    points.AddRange(this.Points.Where(p => p.X == i && p.Y == ymin));
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
                    points.AddRange(this.Points.Where(p => p.X == i && p.Y == ymax));
                }
            }
            return new Path(this.Color, points);
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
        var allpoints = new HashSet<Point>();
        var ctrpoints = new HashSet<Point>();
        var visiting = new HashSet<Point>() { new (x0,y0) };
        int count = w * h;
        while (allpoints.Count<count)
        {
            var ps = visiting.ToArray();
            allpoints.UnionWith(ps);
            visiting.Clear();

            foreach (var p in ps)
            {
                if(ctrpoints.Add(p))
                for (int dy = -1; dy <= +1; dy++)
                {
                    int y = p.Y + dy;
                    y = y < 0 ? 0 : y;
                    y = y >= h ? h - 1 : y;

                    for (int dx = -1; dx <= +1; dx++)
                    {
                        int x = p.X + dx;
                        x = x < 0 ? 0 : x;
                        x = x >= w ? w - 1 : x;
                        var cp = new Point(x, y);
                        //if (allpoints.Add(cp)) //first time in hashset
                        {
                            visiting.Add(cp);
                        }
                    }
                }
            }

            foreach(var p in visiting.ToArray())
            {
                var any = false;
                var Color = bitmap.GetPixel(p.X, p.Y);
                if (colregions.TryGetValue(Color, out var rs))
                {
                    foreach (var region in rs)
                    {
                        //p can be within many regions with same color
                        if (p.IsWithin(region))
                        {
                            any = true;
                            region.Points.Add(p);
                        }
                    }
                }
                if (!any)
                {
                    colregions.Add(Color,
                        new Region(Color, p.X, p.Y, new() { p }));
                }
            }
        }

        var regions = new List<Region>();
        foreach(var ctr in colregions)
        {
            this.CompactRegions(ctr.Value, regions);
            //regions.AddRange(ctr.Value);
        }

        if (regions.Count > 0)
        {
            using var g = Graphics.FromImage(bitmap);
            //here we get all regions
            foreach (var region in regions)
            {
                var path = region.GetBorderPath();
                var pen = new Pen(path.InvertColor,1.0f);
                if (path.Points.Count > 0)
                {
                    var p0 = path.Points[0];
                    for(int i = 1; i < path.Points.Count; i++)
                    {
                        var pt = path.Points[i];
                        g.DrawLine(pen, p0.X, p0.Y, pt.X, pt.Y);
                        p0 = pt;
                    }
                }
            }
            bitmap.Save(System.IO.Path.ChangeExtension(fn, "-with-regions.png"));
        }
    }
}
