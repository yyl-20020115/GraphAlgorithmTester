using System.Collections.Generic;
using System.Linq;

namespace GraphAlgorithmTester;

public class CompleteGraphDeterminer
{
    public readonly IDictionary<string, SNode> Nodes;
    public readonly ISet<SEdge> Edges;
    public CompleteGraphDeterminer(IDictionary<string, SNode> nodes, ISet<SEdge> edges)
    {
        Nodes = nodes;
        Edges = edges;
    }
    public bool IsCompleteGraph(bool undirectional = false)
    {
        if (this.Edges.Count == 0 || this.Nodes.Count == 0) return false;

        var edges = new List<SEdge>();
        var nodes = this.Nodes.Values.ToList();

        for(int i= 0; i < nodes.Count; i++)
        {
            for(int j= i+1; j < edges.Count; j++)
            {
                var e = new SEdge(nodes[i], nodes[j]);
                edges.Add(e);
                if (!undirectional)
                {
                    edges.Add(e.Reverse());
                }
            }
        }
        foreach(var edge in edges)
        {
            //one edge for one pair of nodes
            if (1 != this.Edges.Count(e => edge.IsDupOf(e, undirectional)))
                return false;
        }
        return true;
    }
}
