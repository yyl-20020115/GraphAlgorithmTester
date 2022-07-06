using System.Collections.Generic;

namespace GraphAlgorithmTester
{
    public class GraphDeterminator
    {
        public readonly IDictionary<string, SNode> Nodes;
        public readonly ISet<SEdge> Edges;
        public GraphDeterminator(IDictionary<string, SNode> nodes, ISet<SEdge> edges)
        {
            Nodes = nodes;
            Edges = edges;
        }
    }
}
