using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            writer.WriteLine("  Total {0} edges: {1}", Edges.Count, string.Join(',', this.Edges));

            var names = Nodes.Keys.ToHashSet();

            var paths = new List<Path>();
            var solutions = new List<Path>();

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
            while (step++ < names.Count)
            {
                paths.RemoveAll(p => p.Nodes.Count <= step || p.IsPreTerminated(names));
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
                var celists = paths.Select(p => p.BuildCopyEdges()).ToList();
                foreach (var list in celists)
                {

                }

                solutions.AddRange(paths);
            }

            //with in all paths, no node is used again.



            writer.WriteLine($"  Total {solutions.Count} solutions:");
            foreach (var solution in solutions)
            {
                writer.WriteLine($"    {solution}");
            }



            writer.WriteLine();
        }


        //TODO: how to make the match?
   

    }

}
