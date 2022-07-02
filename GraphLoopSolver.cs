using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace GraphAlgorithmTester;

public class GraphLoopSolver
{
    public SortedDictionary<string, SNode> Nodes { get; } = new();
    public HashSet<SEdge> Edges { get; } = new();

    public void Solve(string path = "Output.txt")
    {
        if (Nodes.Count > 0 && Edges.Count > 0)
        {
            using var writer = new StreamWriter(path);
            writer.WriteLine("Total {0} nodes", Nodes.Count);
            writer.WriteLine("Total {0} edges", Edges.Count);
            var t = GraphTools.GetAllLoops(Edges, out var loops);
            if (t)
            {
                writer.WriteLine("Total {0} min loops:", loops.Count);
                foreach (var loop in loops)
                {
                    writer.WriteLine(loop.ToString());
                }
            }
        }
    }
}
