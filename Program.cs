using System;
using System.IO;

namespace GraphAlgorithmTester;

public static class Program
{
    public static void Main(string[] args)
    {
        Environment.CurrentDirectory += "..\\..\\..\\..";

        var solver = new TravellerProblemSolver();

        Utils.BuildGraph("CityGraphInput.txt", solver.Nodes, solver.Edges);

        using var writer = new StreamWriter("CityGraphResult.txt");
        solver.Solve(writer);
    }
}
