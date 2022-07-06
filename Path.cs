using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GraphAlgorithmTester;


public class PathDirEq : IEqualityComparer<Path>
{
    public bool Equals(Path x, Path y)
        => x.EqualToDirectionally(y);

    public int GetHashCode([DisallowNull] Path path) => 0;
}
public class PathUndirEq : IEqualityComparer<Path>
{
    public bool Equals(Path x, Path y) => x.EqualToUndirectionally(y);

    public int GetHashCode([DisallowNull] Path path) => 0;
}


public record class Path(List<SNode> Nodes)
{

    public bool Undirectional = false;
    public int NodesCount => this.Nodes.Count;
    public List<SNode> NodeCopies = new();
    public SNode Start => this.Nodes.FirstOrDefault();
    public SNode End => this.Nodes.LastOrDefault();
    public int? Length = null;
    public bool CheckLength(int length)
    {
        if (this.Length == null) return false;

        for(int i = this.NodeCopies.Count - 1; i >= 0; i--)
        {
            length -= this.NodeCopies[i].Offset.GetValueOrDefault();
            if (length == 0)
                return true;
        }
        return false;
    }
    public bool HasVisited(SNode node) => this.Nodes.Any(n => n.Name == node.Name);

    public bool EqualToDirectionally(Path path) => this.Nodes.SequenceEqual(path.Nodes);
    public bool EqualToUndirectionally(Path path)
    {
        if (EqualToDirectionally(path)) return true;
        var rev = new List<SNode>(this.Nodes);
        rev.Reverse();
        return rev.SequenceEqual(path.Nodes);
    }
    public bool IsPreTerminated(HashSet<string> names) => this.Nodes.Count < names.Count + 1 && names.Any(name => this.Nodes.Count(n => n.Name == name) >= 2);
    public bool IsPreTerminated(HashSet<int> indices) => this.Nodes.Count < indices.Count + 1 && indices.Any(index => this.Nodes.Count(n => n.NodeIndex == index) >= 2);

    public List<SEdge> BuildEdges(bool undirectional = false)
    {
        var edges = new List<SEdge>();
        for(int i = 0; i < this.Nodes.Count-1;i++)
        {
            var edge = new SEdge(this.Nodes[i], this.Nodes[i + 1]);
            edges.Add(edge);
            if (undirectional)
            {
                edges.Add(edge.Reverse());
            }
        }
        return edges;
    }
    public List<SEdge> BuildCopyEdges(bool undirectional = false)
    {
        var edges = new List<SEdge>();
        for (int i = 0; i < this.NodeCopies.Count - 1; i++)
        {
            var edge = new SEdge(this.NodeCopies[i], this.NodeCopies[i + 1]);
            edges.Add(edge);
            if (undirectional)
            {
                edges.Add(edge.Reverse());
            }
        }
        return edges;
    }

    public Path Copy() =>
        new(this.Nodes.ToList()) { Length = this.Length, NodeCopies = this.NodeCopies.ToList() };
    public override string ToString() => this.ToString(this.Undirectional);
    public virtual string ToString(bool bd) => string.Join(
        (bd ? " - " : " -> "), this.NodeCopies) + (this.Length.HasValue ? " = " + this.Length.Value:"");
}

