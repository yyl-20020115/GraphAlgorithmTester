namespace GraphAlgorithmTester;

public class SEdge
{
    public SNode O;
    public SNode T;
    public int EdgeIndex = 0;
    public int Weight = 0;
    public bool WithWeight = false;

    public int Length { get => this.Weight; set => this.Weight = value; }
    public int Capacity { get => this.Weight; set => this.Weight = value; }
    public SEdge(SNode o, SNode t, int w = 0)
    {
        this.O = o;
        this.T = t;
        this.Weight = w;
    }
    public bool IsDupOf(SEdge target, bool undirectional = false)
        => undirectional 
        ? (this.O == target.O && this.T == target.T || this.O == target.T && this.T == target.O) 
        : (this.O == target.O && this.T == target.T)
        ;

    public SEdge Duplicate() => new(this.O, this.T, this.Weight);
    public bool IsEitherEndingPoint(SNode n)
        => this.O == n || this.T == n;
    public SNode GetOtherEndingPoint(SNode n)
        => this.O == n ? this.T : (this.T == n ? this.O : null);
    public override string ToString() =>  
        (this.WithWeight?$"{this.O}-[{this.Weight}]->{this.T}":$"{this.O}->{this.T}");

    public SEdge Reverse() => new(this.T, this.O, this.Weight);
}
