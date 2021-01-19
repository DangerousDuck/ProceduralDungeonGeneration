using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphEdge
{
    public GraphVertex a;
    public GraphVertex b;

    public GraphEdge(GraphVertex a_, GraphVertex b_)
    {
        a = a_;
        b = b_;
    }

    public GraphEdge(GraphEdge edge_)
    {
        a = edge_.a;
        b = edge_.b;
    }


    public override int GetHashCode()
    {

        float sum = a.position.x + a.position.y + b.position.x + b.position.y;

        return sum.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        GraphEdge other = obj as GraphEdge;

        if ((a.position == other.a.position || a.position == other.b.position) && (b.position == other.a.position || b.position == other.b.position))
        {
            return true;
        }

        return false;

    }

}