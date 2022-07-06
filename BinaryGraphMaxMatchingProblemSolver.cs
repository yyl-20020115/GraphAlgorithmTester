using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphAlgorithmTester;

public class BinaryGraphMaxMatchingProblemSolver : ProblemSolver
{
    public override void Solve(TextWriter writer, string start_name = null, string end_name = null)
    {
        writer.WriteLine("Binary Max Matching:");
        if (Nodes.Count < 2 || Edges.Count < 1)
        {
            writer.WriteLine("  Nodes count should >=2 and Edges count should >=1 too.");
        }
        else
        {
            writer.WriteLine("  Total {0} nodes: {1}", Nodes.Count, string.Join(',', this.Nodes.Values));
            writer.WriteLine("  Total {0} edges: {1}", Edges.Count, string.Join(',', this.Edges));

            var d = new BinaryGraphDeterminer(this.Nodes, this.Edges);
            var left = new List<SNode>();
            var right = new List<SNode>();
            if (!d.IsBinaryGraph(left, right))
            {
                writer.WriteLine("  Please input binary graphs!");
                return;
            }

            //TODO: how to make the match?


        }
    }
}
