using System;
using System.IO;

namespace GraphAlgorithmTester;

public static class Program
{
    public static void Main(string[] args)
    {
        Environment.CurrentDirectory += "..\\..\\..\\..";

        var ts = new TravellerProblemSolver();
        Utils.BuildGraph("CityGraph.txt", ts.Nodes, ts.Edges);

        using var writer = new StreamWriter("CityGraphResult.txt");
        ts.Solve(writer);
    }
}
