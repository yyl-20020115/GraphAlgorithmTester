using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphAlgorithmTester;

public class HamitonianCycleProblemSolver :ProblemSolver
{

    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        writer.WriteLine("HamitonianCycleProblem:");
        if (Nodes.Count < 2 || Edges.Count < 2)
        {
            writer.WriteLine("  Nodes count should >=2 and Edges count should >=2 too.");
        }
        else
        {
            writer.WriteLine("  Total {0} nodes: {1}", Nodes.Count, string.Join(',', this.Nodes.Values));
            writer.WriteLine("  Total {0} edges: {1}", Edges.Count, string.Join(',', this.Edges));

            var names = Nodes.Keys.ToHashSet();

            if (!Nodes.TryGetValue((start_name ??= Nodes.First().Key), out var start))
                start = Nodes.First().Value;

            if (!Nodes.TryGetValue((end_name ??= Nodes.First().Key), out var end))
                end = Nodes.First().Value;

            var paths = new List<Path>();
            var solutions = new List<Path>();

            var outs = this.Edges.Where(e => e.O == start).ToList();
            foreach (var _out in outs)
            {
                paths.Add(new Path(start, _out.T)
                {
                    NodeCopies = new() { start.Copy(), _out.T.Copy() }
                });
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
                        if ((_path.HasVisited(sn) && sn.Name == start.Name) || !_path.HasVisited(sn))
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
                solutions.AddRange(paths);
            }
            var compacts = solutions.Distinct(new PathUndirEq()).ToList();
            writer.WriteLine($"  Total {solutions.Count} solutions:");
            foreach (var solution in solutions)
            {
                writer.WriteLine($"    {solution}");
            }
            writer.WriteLine($"  Total {compacts.Count} compact solutions:");

            foreach (var compact in compacts)
            {
                writer.WriteLine($"    {compact.ToString(true)}");
            }
            writer.WriteLine();
        }
    }
}
