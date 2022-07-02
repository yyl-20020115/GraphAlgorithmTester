using System;

namespace GraphAlgorithmTester;

public static class Program
{
    public static void Main(string[] args)
    {
        Environment.CurrentDirectory += "..\\..\\..\\..";

        var ts = new TravellerProblemSolver();
        Utils.BuildGraph("CityGraph.txt", ts.Nodes, ts.Edges);

        ts.Solve();
    }
}
