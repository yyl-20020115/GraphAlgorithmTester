using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphAlgorithmTester;

public class MutexGraphProblemSolver : ProblemSolver
{
    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        //# 元素=[1,2,3,4,5,6,7,8,9] 互斥=[(1,4),(2,5),(1,5),(5,6),(7,8),(3,9),(2,8),(4,5)]
        //# 把元素组成 N 个组, 保证互斥元素不在同一个组里, 并且 N 最小
        writer.WriteLine("MutexGraphProblem:");
        if (Nodes.Count < 1 || Edges.Count < 1)
        {
            writer.WriteLine("  Nodes count should >=1 and Edges count should >=1 too.");
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
            
            start.Color = 0;


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
                var removeds = this.Nodes.Values.Where(n => n.LevelIndex >= names.Count).ToHashSet();
                writer.WriteLine($"    Found a loop!");
                writer.WriteLine($"      Removed nodes:{string.Join(',', removeds)}");
            }
            //Build levels
            var levels = new List<SNodeSet>();
            for (int i = 0; i < level_index - 1; i++)
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

            var pathcolors = new Dictionary<Path, HashSet<int>>();
            var pathgroups = new Dictionary<Path, Dictionary<int,SNodeSet>>();
            pathcolors.Add(new Path(start), new() { 0 });
            //TODO:
            {
                var path = default(Path);
                var colors = pathcolors[path];
                foreach (var level in levels)
                {
                    foreach (var node in level)
                    {
                        var acs = colors.ToHashSet();
                        var ancester = this.Edges.Where(e => e.T == (node)).Select(e => e.O).ToList();
                        var ccs = ancester.Select(c => c.Color.GetValueOrDefault()).ToHashSet();
                        acs.ExceptWith(ccs);
                        if (acs.FirstOrDefault() is int color)
                        {
                            node.Color = color;
                        }
                        else
                        {
                            colors.Add(colors.Count);
                            node.Color = colors.Last();
                        }
                    }
                }
                var groups = pathgroups[path];
                foreach (var c in colors)
                {
                    groups[c] = new SNodeSet(this.Nodes.Values.Where(n => n.Color == c));
                }
            }
            //TODO:
            var mckey = pathcolors.OrderByDescending(x=>x.Value.Count).Select(x=>x.Key).First();
            var _groups = pathgroups[mckey];
            writer.WriteLine($"  Best solution, total {_groups.Count} groups:");
            foreach(var g in _groups)
            {
                writer.WriteLine($"    color:{g.Key}:{g.Value})");
            }

            writer.WriteLine();
        }
    }
}
