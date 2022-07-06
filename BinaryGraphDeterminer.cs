using System.Collections.Generic;
using System.Linq;

namespace GraphAlgorithmTester;

public class BinaryGraphDeterminer : GraphDeterminator
{
    public enum NodeColor : uint
    {
        Uncolored = 0,
        Red = 1,
        Blue =2,
    }
    public BinaryGraphDeterminer(IDictionary<string, SNode> nodes, ISet<SEdge> edges)
        : base(nodes, edges) { }

    public bool IsBinaryGraph(List<SNode> reds = null, List<SNode> blues = null)
    {
        if (this.Nodes.Count <=1) return true;//special binary graph

        var colored = new Dictionary<SNode, NodeColor>();

        foreach (var edge in this.Edges)
        {
            var bo = colored.TryGetValue(edge.O, out var oc);
            var bt = colored.TryGetValue(edge.T, out var tc);
            if (bo && bt && oc == tc)
                return false;

            //try oc
            if (!bo)
            {
                //oc has no color, try tc
                if (!bt)
                {
                    //oc has no color, tc has no color
                    colored.Add(edge.O, NodeColor.Red);
                    colored.Add(edge.T, NodeColor.Blue);
                }
                else
                {
                    //oc has no color, tc has color
                    //set oc to opposite color
                    colored.Add(edge.O, tc == NodeColor.Red ? NodeColor.Blue : NodeColor.Red);
                }

            }
            else //oc has color,try tc
            {

                if (!bt)
                {
                    //tc has no color
                    //set tc
                    colored.Add(edge.T, oc == NodeColor.Red ? NodeColor.Blue : NodeColor.Red);
                }
            }
        }
        //have to all clorred
        if (colored.Count != this.Nodes.Count)
            return false;

        var rs = colored.Where(c => c.Value == NodeColor.Red).Select(n => n.Key).ToList();
        var bs = colored.Where(c => c.Value == NodeColor.Blue).Select(n => n.Key).ToList();
        if (rs.Count + bs.Count != this.Nodes.Count)
            return false;

        reds?.AddRange(rs);
        blues?.AddRange(bs);
        return true;
    }
}
