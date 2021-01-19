using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphVertex
{
    public Vector2 position;

    public List<int> neighbourRefList = new List<int>();
    
    public GraphVertex(Vector2 position_)
    {
        position = position_;
    }

    public GraphVertex(GraphVertex vertex_)
    {
        position = vertex_.position;
        neighbourRefList = vertex_.neighbourRefList;
    }

    public override int GetHashCode()
    {

        return (position.x + position.y).GetHashCode();
    }

    public override bool Equals(object obj)
    {
        GraphVertex other = obj as GraphVertex;

        // reicht oder noch neighbourRefList?
        if (position.Equals(other.position) )
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
