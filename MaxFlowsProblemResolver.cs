using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphAlgorithmTester;

public class MaxFlowsProblemResolver : ProblemSolver
{
    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        if (Nodes.Count >= 2 && Edges.Count > 0)
        {
            writer.WriteLine("MaxFlowsProblem:");
            writer.WriteLine("  Total {0} nodes", Nodes.Count);
            writer.WriteLine("  Total {0} edges", Edges.Count);

            var names = Nodes.Keys.ToHashSet();
            //first is in
            if (!Nodes.TryGetValue((start_name ??= Nodes.First().Key), out var start))
                start = Nodes.First().Value;
            //last is out
            if (!Nodes.TryGetValue((end_name ??= Nodes.First().Key), out var end))
                end = Nodes.Last().Value;

            foreach(var edge in Edges.Where(e => e.O == start))
            {
                start.Capacity += edge.Capacity;
            }
            foreach(var edge in Edges.Where(e => e.T == end))
            {
                end.Capacity += edge.Capacity;
            }

            var all = new HashSet<SNode>();
            var nodes = new HashSet<SNode>() { start};
            var nexts = new HashSet<SNode>();
            int level = 1;

            do
            {
                foreach (var node in nodes.ToArray())
                {
                    foreach (var edge in this.Edges.Where(e => e.O == node))
                    {
                        var t = edge.T;
                        t.Capacity 
                            = this.Edges.Where(e => e.T == t).Sum(e => e.Capacity);
                        t.Level = level;
                        nexts.Add(t);
                    }
                }
                level++;
                nodes = nexts;
                nexts = new();
                //if level is too many, break the loop in case 
                //we find loops in graph
            } while (nodes.Count > 0 && level <= names.Count);
            if (level > names.Count + 1)
            {
                //there is a loop!
                //we should break this edge and remove this node.
                this.Edges.RemoveWhere(
                    e => e.O.Level >= names.Count || e.T.Level >= names.Count);

                start.Level = 0;
                end.Level = names.Count - 1;

                var removeds = this.Nodes.Values.Where(n => n.Level >= names.Count).ToHashSet();

                foreach (var name in removeds.Select(r => r.Name))
                {
                    this.Nodes.Remove(name);
                }
                writer.WriteLine($"    Found a loop!");
                writer.WriteLine($"      Removed nodes:{string.Join(',',removeds)}");
            }
            //Build levels
            var levels = new List<HashSet<SNode>>();
            for(int i = 0; i < level - 1; i++)
            {
                levels.Add(this.Nodes.Values.Where(n => n.Level == i).ToHashSet());
            }
            foreach(var edge in this.Edges.ToArray())
            {
                //Insert fake nodes to ensure flows connected
                //We don't actually need to build edges since we just need the 
                //max flows value.
                for (int j = edge.O.Level + 1; j < edge.T.Level; j++)
                {
                    var snode = new SNode($"FAKE({edge.O}-{edge.T})");
                    snode.Capacity = edge.Capacity;
                    snode.Level = j;
                    levels[j].Add(snode);
                }
            }

            var maxflows = levels.Min(n => n.Sum(s => s.Capacity));

            writer.WriteLine($"    The max flows value is {maxflows}");
            writer.WriteLine();
        }
    }
}
