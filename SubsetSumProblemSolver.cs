using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphAlgorithmTester;

public class SubsetSumProblemSolver :ProblemSolver
{
    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        writer.WriteLine("SubsetSumProblem:");
        if (this.Nodes.Count <=1)
        {
            writer.WriteLine("This problem needs at least 2 nodes");
        }//>=2
        else
        {
            int sum = 0;
            SNode? first = null;
            var values = new List<int>();
            foreach(var node in this.Nodes.Values.OrderBy(n=>n.NodeIndex))
            {
                if (!int.TryParse(node.Name, out var value))
                {
                    writer.WriteLine($"Node {node.Name} should be a number!");
                    return;
                }
                else
                {
                    if(first == null)
                    {
                        first = node;
                        sum = value;
                    }
                    else 
                    {
                        values.Add(value);
                    }
                }
            }
            if (first != null)
            {
                Nodes.Remove(first.Name);
            }
            //now we have sum and nodes
            //we link every node to all other nodes
            foreach (var node in Nodes.Values)
            {
                //undirectional
                var forwards = this.Nodes.Values.Where(n => n != node).Select(n => new SEdge(node, n)).ToList();
                var backwards = forwards.Select(n => n.Reverse()).ToList();
                this.Edges.UnionWith(forwards);
                this.Edges.UnionWith(backwards);
            }
            //now we do path searching
            var names = Nodes.Keys.ToHashSet();

            Path? solution = null;

            foreach (var pair in this.Nodes)
            {
                var start = pair.Value;
                var end = start;
                var paths = new List<Path>();

                var outs = this.Edges.Where(e => e.O == start).ToList();
                foreach (var _out in outs)
                {
                    paths.Add(new(
                        new() { start, _out.T })
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
                        if (_path.CheckLength(sum))
                        {
                            //we found the sum and don't needs to 
                            solution = _path;
                            goto exit_while;
                        }
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
            }

        exit_while:

            if (solution != null)
            {
                writer.WriteLine($"  The solutions is:");
                writer.WriteLine($"    {solution}");

            }
            else
            {
                writer.WriteLine($"  There is no solution to this problem.");
            }

            writer.WriteLine();

        }
    }
}
