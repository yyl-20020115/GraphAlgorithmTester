using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphAlgorithmTester;

public class MaxCliqueProblemSolver : ProblemSolver
{
    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        var sets = new Dictionary<SNode, SNodeSet>();
        foreach(var n in this.Nodes.Values)
        {
            sets.Add(n, 
                new SNodeSet(this.Edges.Where(e => e.O == n).Select(e => e.T))
                .UnionWith(this.Edges.Where(e => e.T == n).Select(e => e.O)));
        }
        int step = 0;
        while (step++<this.Nodes.Count) 
        {
            foreach (var p in sets)
            {
                var vh = p.Value;
                foreach (var v in vh.ToArray())
                {
                    var ov = this.Edges.Where(e => vh.Contains(e.T) && e.O == v).Select(e => e.T).ToHashSet();
                    var ot = this.Edges.Where(e => vh.Contains(e.O) && e.T == v).Select(e => e.O).ToHashSet();

                    if (ov.Count == 0 && ot.Count == 0) goto exit_while;
                    vh.UnionWith(ov);
                    vh.UnionWith(ot);
                }
            }
        }
    exit_while:
        var r = sets.Values.Distinct(new SNodeSetEq()).ToList();
        if (r.Count > 0)
        {
            r.Sort((x, y) => x.Count - y.Count);
            writer.WriteLine($"  Total {r.Count} solutions:");
            foreach (var solution in r)
            {
                writer.WriteLine($"    {solution}");
            }
            writer.WriteLine($"  Max solutions has {r.First().Count} nodes");

        }
        writer.WriteLine();
    }
}
