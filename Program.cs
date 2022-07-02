using System;
using System.Collections.Generic;

namespace GraphAlgorithmTester;

public static class Program
{
    public static void Main(string[] args)
    {
        Environment.CurrentDirectory += "..\\..\\..\\..";

        var nodes = new Dictionary<string,SNode>();
        var edges = new HashSet<SEdge>();

        Utils.BuildGraph("CityGraph.txt", nodes, edges);

        var ts = new TravellerProblemSolver();
        foreach(var node in nodes)
        {
            ts.Nodes.Add(node.Key, node.Value);
        }
        ts.Edges.UnionWith(edges);

        ts.Solve();

    }
}
