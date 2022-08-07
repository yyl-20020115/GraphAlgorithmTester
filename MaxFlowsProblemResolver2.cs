using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphAlgorithmTester;

public class MaxFlowsProblemResolver2 : ProblemSolver
{
    public List<T> Interleave<T>(List<T> first, List<T> second)
    {
        var result = new List<T>();
        int min = Math.Min(first.Count, second.Count);
        for (int i = 0; i < min; i++)
        {
            result.Add(first[i]);
            result.Add(second[i]);
        }
        if (first.Count > min)
        {
            result.AddRange(first.Skip(min));
        }
        if (second.Count > min)
        {
            result.AddRange(second.Skip(min));
        }
        return result;
    }
    public List<(int, int)> Downgrade(List<int> sequence, int p)
    {
        var copy = sequence.ToList();
        var result = new List<(int, int)>();
        var top = copy[p];
        for (int i = p - 1; i >= 0; i--)
        {
            if (copy[i] > top)
            {
                var delta = top - copy[i];
                copy[i] = top;
                result.Add((i, delta));
            }
        }
        return result;
    }
    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        writer.WriteLine("MaxFlowsProblem2:");
        if (Nodes.Count < 2 || Edges.Count == 0)
        {
            writer.WriteLine("  Nodes count should >=2 and Edges count should >=1 too.");
        }
        else
        {
            writer.WriteLine("  Total {0} nodes: {1}", Nodes.Count, string.Join(',', this.Nodes.Values));
            writer.WriteLine("  Total {0} edges: {1}", Edges.Count, string.Join(',', this.Edges));

            var names = Nodes.Keys.ToHashSet();
            //first is in
            if (!Nodes.TryGetValue((start_name ??= Nodes.First().Key), out var start))
                start = Nodes.First().Value;
            //last is out
            if (!Nodes.TryGetValue((end_name ??= Nodes.Last().Key), out var end))
                end = Nodes.Last().Value;

            //reverse loops
            var loops = this.ReverseLoops(start, names);
            if (loops > 0)
            {
                writer.WriteLine($"    Found {loops} loop(s), and they're now reversed!");
            }

            //first capacity is calculated backwards
            start.OutCapacity = start.InCapacity = Edges.Where(e => e.O == start).Sum(e => e.Capacity);
            end.OutCapacity = end.InCapacity = Edges.Where(e => e.T == end).Sum(e => e.Capacity);

            var all = new HashSet<SNode>();
            var nodes = new HashSet<SNode>() { start };
            var nexts = new HashSet<SNode>();
            int level_index = 1;

            do
            {
                foreach (var node in nodes.ToArray())
                {
                    foreach (var edge in this.Edges.Where(e => e.O == node))
                    {
                        var t = edge.T;
                        if (t != end && t != start)
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
            } while (nodes.Count > 0 && level_index <= names.Count);
            

            //Build levels
            var levels = new List<HashSet<SNode>>();
            for (int i = 0; i < level_index - 1; i++)
            {
                var _level = this.Nodes.Values.Where(n => n.LevelIndex == i).ToHashSet();
                if (_level.Count > 0)
                {
                    levels.Add(_level);
                    all.UnionWith(_level);
                }
            }

            if (levels.Count == 0)
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
                //Insert fake node and build edges to ensure flows connected
                SNode previous = null;
                for (int j = edge.O.LevelIndex + 1; j < edge.T.LevelIndex; j++)
                {
                    previous ??= edge.O;
                    var fake = new SNode($"FAKE({edge.O}-{edge.T})")
                    {
                        InCapacity = edge.Capacity,
                        OutCapacity = edge.Capacity,
                        LevelIndex = j
                    };
                    levels[j].Add(fake);
                    this.Edges.Add(new (previous, fake, edge.Capacity));
                    previous = fake;
                }
                if (previous != null)
                {
                    this.Edges.Add(new (previous, edge.T, edge.Capacity));
                    //we should remove the original edge
                    this.Edges.Remove(edge);
                }
            }




            var inps = new List<int>(levels.Select(l => l.Sum(n => n.InCapacity)));
            var outs = new List<int>(levels.Select(l => l.Sum(n => n.OutCapacity)));

            var maxflows = inps.Min();

            (int index, int delta, HashSet<SNode> nodes)? top_negs = null;
            //if any node in any level has greater out capacity than in capacity,
            //we think this network is ready for max flows
            //otherwise, we check the node and it's affection.
            for (var i = levels.Count - 1; i > 0; i--)
            {
                var _level = levels[i];
                var negs = _level.Where(n => n.DeltaCapacity < 0).ToHashSet();
                if (negs.Any())
                {
                    top_negs = (i, negs.Sum(n => n.DeltaCapacity),negs);
                    break;
                }
            }

            //find the narrowest part of the flow stream and get the flow value
            int ilevel = 0;
            writer.WriteLine($"  Total {inps.Count} levels:");
            foreach (var _level in levels)
            {
                writer.Write($"    Level {ilevel}, flows = {inps[ilevel]}: ");
                ilevel++;
                var firstline = true;
                foreach (var n in _level)
                {
                    if (!firstline) writer.Write(", ");
                    writer.Write(
                        $"{n.Name} (IN:{n.InCapacity}, OUT:{n.OutCapacity}, DELTA:{n.DeltaCapacity})");
                    firstline = false;
                }
                writer.WriteLine();
            }
            writer.WriteLine($"  The max flows value is {maxflows}");
            writer.WriteLine();
        }
    }
}
