using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphTriangle
{
    //TODO: vertex?f
    public GraphVertex a;
    public GraphVertex b;
    public GraphVertex c;

    public GraphEdge edge1;
    public GraphEdge edge2;
    public GraphEdge edge3;

    public GraphTriangle(GraphVertex a_, GraphVertex b_, GraphVertex c_)
    {
        a = a_;
        b = b_;
        c = c_;

        edge1 = new GraphEdge(a, b);
        edge2 = new GraphEdge(b, c);
        edge3 = new GraphEdge(c, a);


    }


    
    public override int GetHashCode()
    {

        float sum = a.position.x + a.position.y + b.position.x + b.position.y + c.position.x + c.position.y;

        return sum.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        GraphTriangle other = obj as GraphTriangle;

        if ((a == other.a || a == other.b || a == other.c) && (b == other.a || b == other.b || b == other.c)
            && (c == other.a || c == other.b || c == other.c))
        {
            return true;
        }

        return false;
    }
    
}
