using System;
using System.IO;

namespace GraphAlgorithmTester;

public static class Program
{
    public static void Main(string[] args)
    {
        Environment.CurrentDirectory += "..\\..\\..\\..\\data\\";

        var traveller = new TravellerProblemSolver();
        var hamilton = new HamitonianCycleProblemSolver();
        var maxflows = new MaxFlowsProblemResolver();
        var subset = new SubsetSumProblemSolver();
        var binary = new BinaryGraphMaxMatchingProblemSolver();
        var coloring = new GraphColoringProblemSolver();

        ProblemSolver.BuildGraph("CityGraphInput.txt", traveller.Nodes, traveller.Edges, true);
        ProblemSolver.BuildGraph("HamiltonianCycle.txt", hamilton.Nodes, hamilton.Edges);
        ProblemSolver.BuildGraph("MaxFlows1.txt", maxflows.Nodes, maxflows.Edges, true);
        ProblemSolver.BuildGraph("Subset1.txt", subset.Nodes, subset.Edges);
        ProblemSolver.BuildGraph("BinaryMatching1.txt", binary.Nodes, binary.Edges);
        ProblemSolver.BuildGraph("GraphColoring3.txt", coloring.Nodes, coloring.Edges, parameters:coloring.Parameters);

        using var writer = new StreamWriter("Result.txt");
        traveller.Solve(writer);
        hamilton.Solve(writer);
        maxflows.Solve(writer);
        subset.Solve(writer);
        binary.Solve(writer);
        coloring.Solve(writer);
    }
}
