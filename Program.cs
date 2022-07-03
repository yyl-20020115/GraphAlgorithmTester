using System;
using System.IO;

namespace GraphAlgorithmTester;

public static class Program
{
    public static void Main(string[] args)
    {
        Environment.CurrentDirectory += "..\\..\\..\\..";

        var traveller = new TravellerProblemSolver();
        var hamilton = new HamitonianCycleProblemSolver();

        ProblemSolver.BuildGraph("CityGraphInput.txt", traveller.Nodes, traveller.Edges);
        ProblemSolver.BuildGraph("HamiltonianCycle.txt", hamilton.Nodes, hamilton.Edges);

        using var writer = new StreamWriter("Result.txt");
        traveller.Solve(writer);
        hamilton.Solve(writer);

    }
}
