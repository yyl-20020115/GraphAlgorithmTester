﻿namespace GraphAlgorithmTester;

public class SEdge
{
    public SNode O;
    public SNode T;
    public int Weight = 0;
    public int Length { get => this.Weight; set => this.Weight = value; }
    public int Capacity { get => this.Weight; set => this.Weight = value; }
    public SEdge(SNode o, SNode t, int w = 0)
    {
        this.O = o;
        this.T = t;
        this.Weight = w;
    }

    public SEdge Duplicate() => new(this.O, this.T, this.Weight);

    public override string ToString() => O.ToString() + "->" + T.ToString();

    public SEdge Reverse() => new(this.T, this.O, this.Weight);
}
