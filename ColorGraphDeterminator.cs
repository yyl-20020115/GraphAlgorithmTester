using System.Collections.Generic;
using System.Linq;

namespace GraphAlgorithmTester;

public class ColorGraphDeterminator :GraphDeterminator
{
    public ColorGraphDeterminator(IDictionary<string, SNode> nodes, ISet<SEdge> edges)
        : base(nodes, edges) { }

    public bool IsColorGraph()
    {
        //one edge can not have both nodes having same color (offset)
        return this.Edges.Any(e => e.O.Offset.HasValue && e.O.Offset == e.T.Offset);
    }
}
