using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphAlgorithmTester;

public class GraphColoringProblemSolver:ProblemSolver
{
    public uint UsableColors = 5;
    public string[] Parameters = new string[1];
    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        writer.WriteLine("GraphColoringProblem:");

        if (Nodes.Count < 1 || Edges.Count < 1)
        {
            writer.WriteLine("  Nodes count should >=1 and Edges count should >=1 too.");
        }
        else
        {
            if (this.Parameters.Length == 1 && this.Parameters[0] is string uc)
            {
                if (uc.StartsWith("C=") && uc.Length > 2)
                {
                    uint.TryParse(uc[2..], out this.UsableColors);
                }
            }

            writer.WriteLine("  Total {0} nodes: {1}", Nodes.Count, string.Join(',', this.Nodes.Values));
            writer.WriteLine("  Total {0} edges: {1}", Edges.Count, string.Join(',', this.Edges));

            var names = Nodes.Keys.ToHashSet();
            var groups = new Dictionary<HashSet<int>, Dictionary<int, SNodeSet>>();

            var edge_collection = new HashSet<SEdge>();

            foreach (var start in Nodes.Values)
            {
                if (edge_collection.Count > 0)
                {
                    this.Edges.UnionWith(edge_collection);
                }
                foreach (var n in this.Nodes.Values)
                {
                    n.Offset = null;
                }

                var nodes = new HashSet<SNode>() { start };
                var nexts = new HashSet<SNode>();
                var all = new HashSet<SNode>();
                int level_index = 1;

                do
                {
                    foreach (var node in nodes.ToArray())
                    {
                        var edges = this.Edges.Where(e => e.O == node).ToArray();
                        foreach (var edge in edges)
                        {
                            var t = edge.T;
                            t.LevelIndex = level_index;
                            nexts.Add(t);
                            //remove the directional 
                            edge_collection.UnionWith(
                                this.Edges.Where(e => e.T == node && e.O == t).ToList()
                                );
                            
                            int c = this.Edges.RemoveWhere(e => e.T == node && e.O == t);
                            if (c != 0)
                            {

                            }
                        }
                    }
                    level_index++;
                    nodes = nexts;
                    nexts = new();
                    //if level is too many, break the loop in case 
                    //we find loops in graph
                } while (nodes.Count > 0 && level_index - 1 < names.Count);
                if (level_index > names.Count + 1)
                {
                    //there is a loop!
                    //we should break this edge and remove this node.
                    start.LevelIndex = 0;
                    var removeds = this.Nodes.Values.Where(n => n.LevelIndex >= names.Count).ToHashSet();
                    writer.WriteLine($"    Found a loop!");
                    writer.WriteLine($"      Removed nodes:{string.Join(',', removeds)}");
                }
                //Build levels
                start.LevelIndex = 0;
                var levels = new List<SNodeSet>();

                for (int i = 0; i < level_index; i++)
                {
                    var level = new SNodeSet(this.Nodes.Values.Where(n => n.LevelIndex == i));
                    if (level.Count > 0)
                    {
                        levels.Add(level);
                        all.UnionWith(level);
                    }
                }
                if (levels.Count == 0)
                {
                    writer.WriteLine($"    Unable to build levels!");
                    return;
                }
                int last = 0;
                int current = last;
                var colors = new HashSet<int>() { };
                var delayed = new HashLookups<int, SNode>();
                for (int idx = 0; idx < levels.Count; idx++)
                {
                    if (delayed.TryGetValue(idx, out HashSet<SNode> delays))
                    {
                        foreach (var node in delays)
                        {
                            //processing input first
                            var ins = this.Edges.Where(e => e.T == node).Select(e => e.O).ToList();
                            var cos = ins.Select(_in => _in.Color is int i ? i : -1).ToHashSet();
                            cos.Remove(-1);
                            if (cos.Count < colors.Count)
                            {
                                var ccs = colors.ToHashSet();
                                ccs.ExceptWith(cos);
                                last = current;
                                current = ccs.First();
                            }
                            else //cos.Count == colors.Count (can not be >)
                            {
                                last = current;
                                cos.Add(current = colors.Count);
                                colors.Add(colors.Count);
                            }
                            node.Offset = current;
                        }
                    }
                    var level = levels[idx];
                    foreach (var node in level)
                    {
                        if (node.Offset == null)
                        {
                            node.Offset = current;
                        }
                    }
                    //has to be
                    colors.Add(current);
                    var found = false;
                    foreach (var node in level)
                    {
                        var outs = this.Edges.Where(e => e.O == node).Select(e => e.T).ToList();
                        if (outs.Count > 0)
                        {
                            foreach (var o in outs)
                            {
                                if (node.LevelIndex + 1 < o.LevelIndex)
                                {
                                    delayed.Add(o.LevelIndex, o); //delay processing
                                }
                                else if (node.LevelIndex + 1 == o.LevelIndex)
                                {
                                    found = true;
                                }
                                else
                                {
                                    found = true;
                                }
                            }
                        }
                    }
                    if (found)
                    {
                        var ch = colors.ToHashSet();
                        ch.Remove(current);
                        current = ch.Count == 0 ? 1 : ch.OrderBy(v=>v).First();
                    }
                }
                var group = new Dictionary<int, SNodeSet>();
                foreach (var c in colors)
                {
                    group[c] = new SNodeSet(
                        this.Nodes.Values.Where(n => n.Color == c));
                }
                groups.Add(colors, group);
            }
            
            var groups_ = new Dictionary<HashSet<int>, Dictionary<int, SNodeSet>>();
            foreach (var kv in groups.Where(g=>g.Key.Count <= UsableColors).DistinctBy(g => g.Key.Count))
            {
                groups_.Add(kv.Key, kv.Value);
            }

            groups = groups_;
            if (groups.Count > 0)
            {
                writer.WriteLine($"  NOTICE: the solution is wrong if nodes are not all connected");

                writer.WriteLine($"  Best solution, total {groups.Count} groups:");
                foreach (var g in groups)
                {
                    writer.WriteLine($"    group:");
                    foreach (var p in g.Value)
                    {
                        writer.WriteLine($"      color: {p.Key} {p.Value})");
                    }
                }
            }
            else
            {
                writer.WriteLine($"  No solution found!");
            }
        }
    }

}
