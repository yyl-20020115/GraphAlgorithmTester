namespace GraphAlgorithmTester;

public class SEdge
{
    public SNode O;
    public SNode T;
    public int Length = 0;

    public SEdge(SNode o, SNode t, int len = 0)
    {
        this.O = o;
        this.T = t;
        this.Length = len;
    }

    public SEdge Duplicate() => new(this.O, this.T, this.Length);

    public override string ToString() => O.ToString() + "->" + T.ToString();

    public SEdge Reverse() => new(this.T, this.O, this.Length);
}
