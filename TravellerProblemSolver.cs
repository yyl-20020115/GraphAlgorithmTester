using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphAlgorithmTester;

public class TravellerProblemSolver
{
    public SortedDictionary<string, SNode> Nodes = new();
    public HashSet<SEdge> Edges = new();

    public record class Path(List<SNode> Nodes)
    {
        public List<SNode> NodeCopies = new();
        public SNode Start => this.Nodes.FirstOrDefault();
        public SNode End => this.Nodes.LastOrDefault();
        public int Length = 0;
        public bool HasVisited(SNode node) => this.Nodes.Any(n => n.Name == node.Name);

        public bool IsPreTerminated(HashSet<string> names) => this.Nodes.Count < names.Count + 1 && names.Any(name => this.Nodes.Count(n => n.Name == name) >= 2);
        public Path Copy() =>
            new(this.Nodes.ToList()) { Length = this.Length, NodeCopies = this.NodeCopies.ToList() };
        public override string ToString() => string.Join(" -> ", this.NodeCopies) + $" = {this.Length}";
    }

    public void Solve(TextWriter writer)
    {
        if (Nodes.Count > 0 && Edges.Count > 0)
        {
            writer.WriteLine("Total {0} nodes", Nodes.Count);
            writer.WriteLine("Total {0} edges", Edges.Count);

            var names = Nodes.Keys.ToHashSet();
            var start = Nodes.First().Value;
            var paths = new List<Path>();
            var solutions = new List<Path>();

            var outs = this.Edges.Where(e => e.O == start).ToList();
            foreach (var _out in outs)
            {
                paths.Add(new(
                    new() { start, _out.T })
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
            paths.Sort((x, y) => x.Length - y.Length);
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
            writer.WriteLine($"Total {solutions.Count} solutions");
            foreach (var solution in solutions)
            {
                writer.WriteLine($"    {solution}");
            }
        }
    }
}
