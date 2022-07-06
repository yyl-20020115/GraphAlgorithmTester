using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace GraphAlgorithmTester;

public class SNodeSetEq : IEqualityComparer<SNodeSet>
{
    public bool Equals(SNodeSet x, SNodeSet y) => x.SequenceEqual(y);

    public int GetHashCode([DisallowNull] SNodeSet obj) => 0;
}

public class SNodeSet : HashSet<SNode>
{
    public SNodeSet() : base() { }
    public SNodeSet(int capacity) : base(capacity) { }
    public SNodeSet(IEnumerable<SNode> collection)
        : base(collection)
    {

    }
    public new SNodeSet UnionWith(IEnumerable<SNode> set)
    {
        base.UnionWith(set);
        return this;
    }
    public override string ToString() => string.Join(',', (IEnumerable<SNode>)this);
}

