using System.Collections.Generic;

namespace GraphAlgorithmTester;

public class SpecialGraphDeterminer : GraphDeterminator 
{
    public SpecialGraphDeterminer(IDictionary<string, SNode> nodes, ISet<SEdge> edges)
            : base(nodes, edges) { }

    public bool IsK5() => this.Nodes.Count == 5 && new CompleteGraphDeterminer(this.Nodes, this.Edges).IsCompleteGraph(true);
    public bool IsK33() => this.Nodes.Count == 6 && new CompleteGraphDeterminer(this.Nodes, this.Edges).IsCompleteGraph(true)
            && new BinaryGraphDeterminer(this.Nodes, this.Edges).IsBinaryGraph();
}
