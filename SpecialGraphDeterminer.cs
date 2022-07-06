using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphAlgorithmTester
{
    public class SpecialGraphDeterminer
    {
        public readonly IDictionary<string, SNode> Nodes;
        public readonly ISet<SEdge> Edges;
        public SpecialGraphDeterminer(IDictionary<string, SNode> nodes, ISet<SEdge> edges)
        {
            Nodes = nodes;
            Edges = edges;
        }

        public bool IsK5() => this.Nodes.Count == 5 && new CompleteGraphDeterminer(this.Nodes, this.Edges).IsCompleteGraph(true);
        public bool IsK33() => this.Nodes.Count == 6 && new CompleteGraphDeterminer(this.Nodes, this.Edges).IsCompleteGraph(true)
                && new BinaryGraphDeterminer(this.Nodes, this.Edges).IsBinaryGraph();

    }
}
