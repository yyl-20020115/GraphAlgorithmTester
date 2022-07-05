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
            writer.WriteLine("  Total {0} nodes: {1}", Nodes.Count, string.Join(',',this.Nodes.Values));
            writer.WriteLine("  Total {0} edges: {1}", Edges.Count, string.Join(',',this.Edges));

            var names = Nodes.Keys.ToHashSet();
            //first is in
            if (!Nodes.TryGetValue((start_name ??= Nodes.First().Key), out var start))
                start = Nodes.First().Value;
            //last is out
            if (!Nodes.TryGetValue((end_name ??= Nodes.Last().Key), out var end))
                end = Nodes.Last().Value;

            //first capacity is calculated backwards
            start.Capacity = Edges.Where(e => e.O == start).Sum(e => e.Capacity);

            var all = new HashSet<SNode>();
            var nodes = new HashSet<SNode>() { start};
            var nexts = new HashSet<SNode>();
            int level_index = 1;

            do
            {
                foreach (var node in nodes.ToArray())
                {
                    foreach (var edge in this.Edges.Where(e => e.O == node))
                    {
                        var t = edge.T;
                        t.Capacity 
                            = this.Edges.Where(e => e.T == t).Sum(e => e.Capacity);
                        t.LevelIndex = level_index;
                        nexts.Add(t);
                    }
                }
                level_index++;
                nodes = nexts;
                nexts = new();
                //if level is too many, break the loop in case 
                //we find loops in graph
            } while (nodes.Count > 0 && level_index <= names.Count);
            if (level_index > names.Count + 1)
            {
                //there is a loop!
                //we should break this edge and remove this node.
                start.LevelIndex = 0;
                end.LevelIndex = names.Count - 1;
                var removeds = this.Nodes.Values.Where(n => n.LevelIndex >= names.Count).ToHashSet();
                writer.WriteLine($"    Found a loop!");
                writer.WriteLine($"      Removed nodes:{string.Join(',',removeds)}");
            }
            //Build levels
            var levels = new List<HashSet<SNode>>();
            for(int i = 0; i < level_index - 1; i++)
            {
                var level = this.Nodes.Values.Where(n => n.LevelIndex == i).ToHashSet();
                if (level.Count > 0)
                {
                    levels.Add(level);
                    all.UnionWith(level);
                }
            }
            if(levels.Count ==0)
            {
                writer.WriteLine($"    Unable to build levels!");
                return;
            }
            else if (!levels.Last().Contains(end))
            {
                writer.WriteLine($"    The graph can not reach to the end!");
                return;
            }
            foreach (var edge in this.Edges.Where(
                //check edge to avoid involving loop nodes
                e => all.Contains(e.O) && all.Contains(e.T)).ToArray())
            {
                //Insert fake nodes to ensure flows connected
                //We don't actually need to build edges since we just need the 
                //max flows value.
                for (int j = edge.O.LevelIndex + 1; j < edge.T.LevelIndex; j++)
                {
                    levels[j].Add(new($"FAKE({edge.O}-{edge.T})")
                    {
                        Capacity = edge.Capacity,
                        LevelIndex = j
                    });
                }
            }

            var maxflows = levels.Min(n => n.Sum(s => s.Capacity));

            writer.WriteLine($"    The max flows value is {maxflows}");
            writer.WriteLine();
        }
    }
}
