using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphAlgorithmTester;

public class TravellerProblemSolver : ProblemSolver
{
    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        writer.WriteLine("TravellerProblem:");
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
                    Length = _out.Length,
                    NodeCopies = new() { start.Copy(), _out.T.Copy(_out.Length) }
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
                            snode.Offset = se.Length;
                            branch.Length += se.Length;
                            branch.Nodes.Add(sn);
                            branch.NodeCopies.Add(snode);
                            paths.Add(branch);
                        }
                    }
                }
            }
            paths.Sort((x, y) => x.Length.GetValueOrDefault() - y.Length.GetValueOrDefault());
            if (paths.Count > 0)
            {
                var d0 = paths[0].Length;
                for (int i = 0; i < paths.Count; i++)
                {
                    if (paths[i].Length <= d0)
                    {
                        solutions.Add(paths[i]);
                    }
                }
            }
            writer.WriteLine($"  Total {solutions.Count} solutions:");
            foreach (var solution in solutions)
            {
                writer.WriteLine($"    {solution}");
            }
            writer.WriteLine();
        }
    }
}
