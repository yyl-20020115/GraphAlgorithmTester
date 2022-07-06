using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphAlgorithmTester;

public class MaxCliqueProblemSolver : ProblemSolver
{
    public class SNodeSetEq : IEqualityComparer<HashSet<SNode>>
    {
        public bool Equals(HashSet<SNode> x, HashSet<SNode> y) => x.SequenceEqual(y);

        public int GetHashCode([DisallowNull] HashSet<SNode> obj) => 0;
    }
    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        var hs = new Dictionary<SNode, HashSet<SNode>>();
        var visited = new HashSet<SNode>();
        foreach(var n in this.Nodes.Values)
        {
            var os = this.Edges.Where(e => e.O == n).Select(e=>e.T).ToHashSet();
            var ts = this.Edges.Where(e => e.T == n).Select(e=>e.O).ToHashSet();
            ts.UnionWith(os);
            hs.Add(n, ts);
            visited.UnionWith(ts);
        }

        while (true) {
            foreach (var p in hs)
            {
                var vh = p.Value;
                foreach (var v in vh.ToArray())
                {
                    var ov = this.Edges.Where(e => vh.Contains(e.T) && e.O == v).Select(e => e.T).ToHashSet();
                    var ot = this.Edges.Where(e => vh.Contains(e.O) && e.T == v).Select(e => e.O).ToHashSet();

                    ov.ExceptWith(visited);
                    ot.ExceptWith(visited);
                    if (ov.Count == 0 && ot.Count == 0) goto exit_while;
                    vh.UnionWith(ov);
                    vh.UnionWith(ot);
                    visited.UnionWith(ov);
                    visited.UnionWith(ot);
                }
            }
        }
    exit_while:
        var r = hs.Values.Distinct(new SNodeSetEq()).ToList();

        writer.WriteLine($"  Total {r.Count} solutions:");
        foreach (var solution in r)
        {
            //HashSet<SNode>
            writer.WriteLine($"    {solution}");
        }
        writer.WriteLine();

        ;
    }
}
