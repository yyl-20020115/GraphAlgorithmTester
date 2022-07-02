namespace GraphAlgorithmTester;

public class SEdge
{
    public SNode O { get; set; }
    public SNode T { get; set; }
    public double Length { get; set; }
    public object Origin => this.O;

    public object Target => this.T;

    public SEdge(SNode o, SNode t, double len = 0)
    {
        this.O = o;
        this.T = t;
        this.Length = len;
    }

    public SEdge Duplicate() => new SEdge(this.O, this.T,this.Length);

    public override string ToString() => O.ToString() + "->" + T.ToString();

    public SEdge Reverse() => new SEdge(this.T, this.O,this.Length);
}
