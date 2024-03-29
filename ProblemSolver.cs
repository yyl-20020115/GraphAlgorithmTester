﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraphAlgorithmTester;
 
public abstract class ProblemSolver
{
    public SortedDictionary<string, SNode> Nodes = new();
    public HashSet<SEdge> Edges = new();

    public abstract void Solve(TextWriter writer, string start_name = null, string end_name = null);

    public static void BuildGraph(string path, IList<SNode> nodes, ISet<SEdge> edges, bool WithWeight = false, string[] parameters = null)
    {
        parameters ??= Array.Empty<string>();
        var pres_count = parameters.Length;
        var pres_index = 0;
        var node_index = 0;
        var edge_index = 0;
        using var reader = new StreamReader(path);
        while (reader.ReadLine() is string line)
        {
            if ((line = line.Trim()).StartsWith('#')) continue;
            if (pres_index < pres_count)
            {
                parameters[pres_index++] = line;
                continue;
            }
            var dn = 0;
            var p = line.IndexOf("->"); //directional
            if (p >= 0) //this is edge
            {
                //node -> node : value
                var so = line[..p].Trim();
                var st = line[(p + 2)..].Trim();
                var sp = st.IndexOf(':');
                if (sp >= 0)
                {
                    var dt = st[(sp + 1)..].Trim();
                    st = st[..(sp)].Trim();
                    if (int.TryParse(dt, out dn))
                    {
                        //ok
                    }
                }
                var o = new SNode(so) { NodeIndex = node_index++ };
                var t = new SNode(st) { NodeIndex = node_index++ };
                nodes.Add(o);
                nodes.Add(t);
                edges.Add(new SEdge(o, t, dn) { WithWeight = WithWeight, EdgeIndex = edge_index++ });
            }
            else if ((p = line.IndexOf('-')) >= 0)
            {
                //node - node : value
                var so = line[..p].Trim();
                var st = line[(p + 1)..].Trim(); //indirectional
                var sp = st.IndexOf(':');
                if (sp >= 0)
                {
                    var dt = st[(sp + 1)..].Trim();
                    st = st[..(sp)].Trim();
                    if (int.TryParse(dt, out dn))
                    {
                        //ok
                    }
                }
                var o = new SNode(so) { NodeIndex = node_index++ };
                var t = new SNode(st) { NodeIndex = node_index++ };
                edges.Add(new SEdge(o, t, dn) { WithWeight = WithWeight, EdgeIndex = edge_index++ });
                edges.Add(new SEdge(t, o, dn) { WithWeight = WithWeight, EdgeIndex = edge_index++ });
            }
            else if (line.Length > 0) //this is node 
            {
                //node:value
                var st = line;
                var sp = st.IndexOf(':');
                if (sp >= 0)
                {
                    var dt = st[(sp + 1)..].Trim();
                    st = st[..(sp)].Trim();
                    if (int.TryParse(dt, out dn))
                    {
                        //ok
                    }
                }

                nodes.Add(new SNode(st) { Offset=dn, NodeIndex = node_index++ });
            }
        }
    }

    public static void BuildGraph(string path, IDictionary<string, SNode> nodes, ISet<SEdge> edges, bool WithWeight = false, string[] parameters = null)
    {
        parameters ??= Array.Empty<string>();
        var node_index = 0;
        var edge_index = 0;
        var pres_count = parameters.Length;
        var pres_index = 0;
        using var reader = new StreamReader(path);
        while (reader.ReadLine() is string line)
        {
            if ((line = line.Trim()).StartsWith('#')) continue;
            if (pres_index < pres_count)
            {
                parameters[pres_index++] = line;
                continue;
            }
            var dn = 0;
            var p = line.IndexOf("->"); //directional
            if (p >= 0) //this is edge
            {
                var so = line[..p].Trim();
                var st = line[(p + 2)..].Trim();
                var sp = st.IndexOf(':');
                if (sp >= 0)
                {
                    var dt = st[(sp + 1)..].Trim();
                    st = st[..(sp)].Trim();
                    if (int.TryParse(dt, out dn))
                    {
                        //ok
                    }
                }
                if (!nodes.TryGetValue(so, out var o))
                    nodes.Add(so, o = new SNode(so) { NodeIndex = node_index++ });
                if (!nodes.TryGetValue(st, out var t))
                    nodes.Add(st, t = new SNode(st) { NodeIndex = node_index++ });
                edges.Add(new SEdge(o, t, dn) { WithWeight = WithWeight, EdgeIndex = edge_index++ });
            }
            else if ((p = line.IndexOf('-')) >= 0)
            {
                var so = line[..p].Trim();
                var st = line[(p + 1)..].Trim(); //indirectional
                var sp = st.IndexOf(':');
                if (sp >= 0)
                {
                    var dt = st[(sp + 1)..].Trim();
                    st = st[..(sp)].Trim();
                    if (int.TryParse(dt, out dn))
                    {
                        //ok
                    }
                }
                if (!nodes.TryGetValue(so, out var o))
                    nodes.Add(so, o = new SNode(so) { NodeIndex = node_index++ });
                if (!nodes.TryGetValue(st, out var t))
                    nodes.Add(st, t = new SNode(st) { NodeIndex = node_index++ });
                edges.Add(new SEdge(o, t, dn) { WithWeight = WithWeight, EdgeIndex = edge_index++ });
                edges.Add(new SEdge(t, o, dn) { WithWeight = WithWeight, EdgeIndex = edge_index++ });
            }
            else if (line.Length > 0) //this is node 
            {
                if (!nodes.TryGetValue(line, out var n))
                    nodes.Add(line, n = new SNode(line) { NodeIndex = node_index++ });
            }
        }
    }

    protected int BreakLoops(SNode start,HashSet<string> names,List<SEdge> brokens = null)
    {
        int count = 0;
        var paths = new List<Path>();
        var solutions = new List<Path>();

        var outs = this.Edges.Where(e => e.O == start).ToList();
        foreach (var _out in outs)
        {
            paths.Add(new Path(start, _out.T)
            {
                NodeCopies = new() { start.Copy(), _out.T.Copy() }
            });
        }

        //find and break loops
        int step = 0;
        while (step++ < names.Count)
        {
            paths.RemoveAll(p => p.Nodes.Count <= step || p.IsPreTerminated(names));
            foreach (var _path in paths.ToArray())
            {
                var current = _path.End;
                foreach (var _out in this.Edges.Where(e => e.O == current).ToArray())
                {
                    var se = _out;
                    var sn = _out.T;
                    if (_path.HasVisited(sn))
                    {
                        count++;
                        //found a loop
                        //remove this edge
                        this.Edges.Remove(_out);
                        brokens?.Add(_out);
                    }
                    else
                    {
                        var branch = _path.Copy();
                        var snode = sn.Copy();
                        branch.Nodes.Add(sn);
                        branch.NodeCopies.Add(snode);
                        paths.Add(branch);
                    }
                }
            }
        }
        return count;

    }
    protected int ReverseLoops(SNode start, HashSet<string> names, List<SEdge> brokens = null)
    {
        int count = 0;
        var paths = new List<Path>();
        var solutions = new List<Path>();

        var outs = this.Edges.Where(e => e.O == start).ToList();
        foreach (var _out in outs)
        {
            paths.Add(new Path(start, _out.T)
            {
                NodeCopies = new() { start.Copy(), _out.T.Copy() }
            });
        }

        //find and break loops
        int step = 0;
        while (step++ < names.Count)
        {
            paths.RemoveAll(p => p.Nodes.Count <= step || p.IsPreTerminated(names));
            foreach (var _path in paths.ToArray())
            {
                var current = _path.End;
                foreach (var _out in this.Edges.Where(e => e.O == current).ToArray())
                {
                    var se = _out;
                    var sn = _out.T;
                    if (_path.HasVisited(sn))
                    {
                        count++;
                        //found a loop
                        //remove this edge
                        this.Edges.Remove(_out);
                        //if there is no reversed one, add it
                        if(!Edges.Any(e=>e.O==_out.T && e.T == _out.O))
                        {
                            this.Edges.Add(_out.Reverse());
                        }
                        brokens?.Add(_out);
                    }
                    else
                    {
                        var branch = _path.Copy();
                        var snode = sn.Copy();
                        branch.Nodes.Add(sn);
                        branch.NodeCopies.Add(snode);
                        paths.Add(branch);
                    }
                }
            }
        }
        return count;

    }
}
