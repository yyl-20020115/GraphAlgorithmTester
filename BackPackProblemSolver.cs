using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphAlgorithmTester;

public class BackPackProblemSolver : ProblemSolver
{
    public string[] Parameters = new string[1];
    public uint MaxWeight = 0;
    public new List<SNode> Nodes { get; } = new();

    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        writer.WriteLine("BackPackProblem:(The graph solution)");
        if (this.Nodes.Count <= 1)
        {
            writer.WriteLine("This problem needs at least 2 nodes");
        }//>=2
        else
        {
            if (this.Parameters.Length == 1 && this.Parameters[0] is string uc)
            {
                if (uc.StartsWith("M=") && uc.Length > 2)
                {
                    if(!uint.TryParse(uc[2..], out this.MaxWeight))
                    {
                        writer.WriteLine("MaxWeight M should be an integer!");
                    }
                }
            }

            var NodeValues = new Dictionary<SNode,int>();
            foreach (var node in this.Nodes.OrderBy(n => n.NodeIndex))
            {
                if (!int.TryParse(node.Name, out var value))
                {
                    writer.WriteLine($"Node {node.Name} should be a number!");
                    return;
                }
                else
                {
                    NodeValues.Add(node,value);
                }
            }
            //now we have sum and nodes
            //we link every node to all other nodes
            foreach (var node in this.Nodes)
            {
                //undirectional
                var forwards = this.Nodes.Where(n => n != node).Select(n => new SEdge(node, n)).ToList();
                var backwards = forwards.Select(n => n.Reverse()).ToList();
                this.Edges.UnionWith(forwards);
                this.Edges.UnionWith(backwards);
            }
            //now we do path searching
            var indices = this.Nodes.Select(n => n.NodeIndex).ToHashSet();

            var paths = new List<Path>();
            //Make every node to start a new path:
            foreach (var node in this.Nodes)
            {
                var outs = this.Edges.Where(e => e.O == node).ToList();
                foreach (var _out in outs)
                {
                    paths.Add(new (node)
                    {
                        Length = node.Offset.GetValueOrDefault(),
                        NodeCopies = new() { node.Copy() },
                    });
                }
            }
            var solutions = new List<Path>();
            //Process with all paths
            int counts = indices.Count;
            int step = 0;
            while (step++ < counts)
            {
                solutions.AddRange(paths.Where(p => p.Length <= this.MaxWeight));
                //we don't need to find all possible solutions
                paths.RemoveAll(p => p.Nodes.Count < step || p.Length > this.MaxWeight || p.IsPreTerminated(indices));
                foreach (var _path in paths.ToArray())
                {
                    var current = _path.End;

                    foreach (var _out in this.Edges.Where(e => e.O == current))
                    {
                        var se = _out;
                        var sn = _out.T;
                        if (!_path.HasVisited(sn))
                        {
                            var branch = _path.Copy();
                            var snode = sn.Copy();
                            snode.Offset = sn.Offset;
                            branch.Length += sn.Offset.GetValueOrDefault();
                            branch.Nodes.Add(sn);
                            branch.NodeCopies.Add(snode);
                            paths.Add(branch);
                        }
                    }
                }
            }
            //var eq = new PathSumEq(NodeValues);
            solutions = solutions.Distinct(new PathUndirEq()).ToList();

            int max_price = solutions.Max(path=>PathSumEq.GetPathSum(path,NodeValues));

            solutions = solutions.Where(path => PathSumEq.GetPathSum(path, NodeValues) == max_price).ToList();

            if (solutions.Count > 0)
            {
                writer.WriteLine($"  There are {solutions.Count} solutions (with assembling order):");
                foreach (var solution in solutions)
                {
                    writer.WriteLine($"    {solution}(Weight), {PathSumEq.GetPathSum(solution, NodeValues)}(Price)");
                }
            }
            else
            {
                writer.WriteLine($"  There is no solution to this problem.");
            }

            writer.WriteLine();

        }
    }
}
