using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphAlgorithmTester;

public class MaxFlowsProblemResolver : ProblemSolver
{
    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        writer.WriteLine("MaxFlowsProblem:");
        if (Nodes.Count < 2 || Edges.Count == 0)
        {
            writer.WriteLine("  Nodes count should >=2 and Edges count should >=1 too.");
        }
        else
        {
            writer.WriteLine("  Total {0} nodes: {1}", Nodes.Count, string.Join(',',this.Nodes.Values));
            writer.WriteLine("  Total {0} edges: {1}", Edges.Count, string.Join(',',this.Edges));

            var names = Nodes.Keys.ToHashSet();
            //first is in
            if (!Nodes.TryGetValue((start_name ??= Nodes.First().Key), out var start))
                start = Nodes.First().Value;
            //last is out
            if (!Nodes.TryGetValue((end_name ??= Nodes.Last().Key), out var end))
                end = Nodes.Last().Value;
             
            var loops = this.ReverseLoops(start, names);
            if (loops > 0)
            {
                writer.WriteLine($"    Found {loops} loop(s), and they're now reversed!");
            }

            //first capacity is calculated backwards
            start.OutCapacity = start.InCapacity = Edges.Where(e => e.O == start).Sum(e => e.Capacity);
            end.OutCapacity = end.InCapacity = Edges.Where(e => e.T == end).Sum(e => e.Capacity);

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
                        if (t != end &&t!=start)
                        {
                            t.InCapacity
                                = this.Edges.Where(e => e.T == t).Sum(e => e.Capacity);
                            t.OutCapacity
                                = this.Edges.Where(e => e.O == t).Sum(e => e.Capacity);
                        }
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
            foreach (var edge in this.Edges.ToArray())
            {
                //Insert fake nodes to ensure flows connected
                //We don't actually need to build edges since we just need the 
                //max flows value.
                for (int j = edge.O.LevelIndex + 1; j < edge.T.LevelIndex; j++)
                {
                    levels[j].Add(new($"FAKE({edge.O}-{edge.T})")
                    {
                        InCapacity = edge.Capacity,
                        OutCapacity = edge.Capacity,
                        LevelIndex = j
                    });
                }
            }
            var inps = new List<int>(levels.Select(l => l.Sum(n => n.InCapacity)));

            var deltas = new HashLookups<int, int>();
            //NOTICE: only focus on nodes with DeltaCapacity<0
            //and substract the delta from the previous level.
            //BTW, first and last nodes are not necessory to be considerred.
            for (var i = levels.Count - 2; i > 0; i--)
            {
                var level = levels[i];
                int delta = level.Where(n => n.DeltaCapacity < 0)
                    .Sum(n => n.DeltaCapacity);
                //we don't consider further influnce,
                //because finally we need the min value (capacity) of all bottle necks.
                //Any previous bottle neck which is wider (more capacity)
                //we just let it be,
                if (delta < 0)
                {
                    //delta<0
                    //keep index and possible delta
                    var p = i - 1;
                    deltas.Add(p, delta);
                    var top = inps[p] + delta; 
                    while(p-->0)
                    {
                        if (top < inps[p])
                        {
                            //FIXED:
                            //we should always keep the influnce
                            //of the post-levels
                            //and when we need to apply,
                            //we use the max value (or min value for negative 
                            deltas.Add(p, (top - inps[p]));
                        }
                    }
                }
            }
            //apply for subs (adding minus value)
            foreach(var t in deltas)
            {
                if (t.Key >= 0 && t.Key < inps.Count)
                {
                    inps[t.Key] += t.Value.Min();
                }
            }

            //find the narrowest part of the flow stream and get the flow value
            var maxflows = inps.Min();
            int ilevel = 0;
            writer.WriteLine($"  Total {inps.Count} levels:");
            foreach (var level in levels)
            {
                writer.Write($"    Level {ilevel}, flows = {inps[ilevel]}: ");
                ilevel++;
                var first = true;
                foreach(var n in level)
                {
                    if (!first) writer.Write(", ");
                    writer.Write(
                        $"{n.Name} (IN:{n.InCapacity}, OUT:{n.OutCapacity}, DELTA:{n.DeltaCapacity})");
                    first = false;
                }
                writer.WriteLine();
            }
            writer.WriteLine($"  The max flows value is {maxflows}");
            writer.WriteLine();
        }
    }
}
