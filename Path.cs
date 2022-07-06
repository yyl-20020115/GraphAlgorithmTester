using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GraphAlgorithmTester;

public record class Path(List<SNode> Nodes)
{

    public class DirEq : IEqualityComparer<Path>
    {
        public bool Equals(Path x, Path y)
            => x.EqualToDirectionally(y);

        public int GetHashCode([DisallowNull] Path path) => 0;
    }
    public class UndEq : IEqualityComparer<Path>
    {
        public bool Equals(Path x, Path y) => x.EqualToUndirectionally(y);

        public int GetHashCode([DisallowNull] Path path) => 0;
    }


    public bool Bidirectional = false;

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
    public Path Copy() =>
        new(this.Nodes.ToList()) { Length = this.Length, NodeCopies = this.NodeCopies.ToList() };
    public override string ToString() => this.ToString(this.Bidirectional);
    public virtual string ToString(bool bd) => string.Join(
        (bd ? " - " : " -> "), this.NodeCopies) + (this.Length.HasValue ? " = " + this.Length.Value:"");
}

