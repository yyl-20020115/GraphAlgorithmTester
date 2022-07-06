using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphAlgorithmTester;

public class BinaryGraphMaxMatchingProblemSolver : ProblemSolver
{
    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        writer.WriteLine("Binary Max Matching:");
        if (Nodes.Count < 2 || Edges.Count < 1)
        {
            writer.WriteLine("  Nodes count should >=2 and Edges count should >=1 too.");
        }
        else
        {
            writer.WriteLine("  Total {0} nodes: {1}", Nodes.Count, string.Join(',', this.Nodes.Values));
            writer.WriteLine("  Total {0} edges: {1}", Edges.Count, string.Join(',', this.Edges));

            var d = new BinaryGraphDeterminer(this.Nodes, this.Edges);
            var left = new List<SNode>();
            var right = new List<SNode>();
            if (!d.IsBinaryGraph(left, right))
            {
                writer.WriteLine("  Please input binary graphs!");
                return;
            }
            writer.WriteLine("  Total {0} nodes: {1}", Nodes.Count, string.Join(',', this.Nodes.Values));
            writer.WriteLine("  Left  {0} nodes: {1}", left.Count, string.Join(',', left));
            writer.WriteLine("  Right {0} nodes: {1}", right.Count, string.Join(',', right));
            writer.WriteLine("  Total {0} edges: {1}", Edges.Count, string.Join(',', this.Edges));
            var indices = this.Nodes.Values.Select(n => n.NodeIndex).ToHashSet();
            var paths = new List<Path>();
            var solution = new List<SEdge>();

            foreach (var node in left)
            {
                var outs = this.Edges.Where(e => e.O == node).ToList();
                foreach (var _out in outs)
                {
                    paths.Add(new(
                        new() { node, _out.T })
                    {
                        NodeCopies = new() { node.Copy(), _out.T.Copy() }
                    });
                }
            }
            //NP=P
            int step = 0;
            while (step++ < indices.Count)
            {
                int maxcount = paths.Max(p => p.NodesCount);
                //only keep the longest paths
                paths.RemoveAll(p => p.NodesCount < maxcount || p.IsPreTerminated(indices));
                foreach (var _path in paths.ToArray())
                {
                    var current = _path.End;
                    foreach (var _out in this.Edges.Where(e => e.O == current))
                    {
                        var se = _out;
                        var sn = _out.T;
                        if (!_path.HasVisited(sn))
                        {
                            var branch = _path.Copy();
                            var snode = sn.Copy();
                            branch.Nodes.Add(sn);
                            branch.NodeCopies.Add(snode);
                            paths.Add(branch);
                        }
                    }
                }
            }
            if (paths.Count > 0)
            {
                paths.Sort((x, y) => y.NodesCount - x.NodesCount);
                var best = paths.First();
                var edges = best.BuildCopyEdges();
                for (int i = 0; i < edges.Count; i += 2)
                {
                    solution.Add(edges[i]);
                }
                solution.Reverse();
            }
            if (solution.Count > 0)
            {
                writer.WriteLine($"  Found best solution:");
                foreach (var edge in solution)
                {
                    writer.WriteLine($"    {edge}");
                }
            }
            else
            {
                writer.WriteLine($"  No best solution found!");
            }
            writer.WriteLine();
        }
    }
}
