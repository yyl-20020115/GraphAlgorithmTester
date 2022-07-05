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
        var maxflows = new MaxFlowsProblemResolver();

        ProblemSolver.BuildGraph("CityGraphInput.txt", traveller.Nodes, traveller.Edges, true);
        ProblemSolver.BuildGraph("HamiltonianCycle.txt", hamilton.Nodes, hamilton.Edges);
        ProblemSolver.BuildGraph("MaxFlows1.txt", maxflows.Nodes, maxflows.Edges,true);

        using var writer = new StreamWriter("Result.txt");
        traveller.Solve(writer);
        hamilton.Solve(writer);
        maxflows.Solve(writer);
    }
}
