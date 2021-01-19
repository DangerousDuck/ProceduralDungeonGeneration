using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DelaunyTriangulation 
{

    private DelaunyTriangulation()
    {

    }
    
    //https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
    // TODO: moeglicherweise ein problem mit negativen zahlen. Wegen supertriangle? Vielleicht zu floating point ungenauigkeiten bei super triangle? okay supertriangle verkleinern hat geholfen
    public static List<GraphTriangle> Triangulate(List<Vector2> roomPositions_)
    {
        List<GraphTriangle> triangulation = new List<GraphTriangle>();

        // determine the supertriangle
        float minX = float.MaxValue;
        float maxX = float.MinValue;

        float minY = float.MaxValue;
        float maxY = float.MinValue;

        for (int i = 0; i < roomPositions_.Count; i++)
        {
            float valueX = roomPositions_[i].x;
            float valueY = roomPositions_[i].y;

            if (valueX > maxX)
                maxX = valueX;
            if (valueX < minX)
                minX = valueX;
            if (valueY > maxY)
                maxY = valueY;
            if (valueY < minY)
                minY = valueY;
        }

        // supertriangle: https://github.com/bl4ckb0ne/delaunay-triangulation/blob/master/dt/delaunay.cpp
        float dx = maxX - minX;
        float dy = maxY - minY;
        float deltaMax = Mathf.Max(dx, dy);
        float midx = (minX + maxX) / 2;
        float midy = (minY + maxY) / 2;

        GraphTriangle superTriangle = new GraphTriangle(new GraphVertex(new Vector2(midx - 3 * deltaMax, midy - deltaMax)), 
            new GraphVertex(new Vector2(midx, midy + 3 * deltaMax)), new GraphVertex(new Vector2(midx + 3 * deltaMax, midy - deltaMax)));

        triangulation.Add(superTriangle);


        for (int i = 0; i < roomPositions_.Count; i++)
        {
            HashSet<GraphTriangle> badTriangles = new HashSet<GraphTriangle>();

            for (int j = triangulation.Count - 1; j >= 0; j--)
            {
                Vector2 center = CalculateCircumcircleCenter(triangulation[j]);
                float radius = CalculateCircumcircleRadius(triangulation[j]);

                if (Vector2.Distance(new Vector2(roomPositions_[i].x, roomPositions_[i].y), center) < radius)
                {
                    badTriangles.Add(triangulation[j]);
                }
            }

            HashSet<GraphEdge> polygon = new HashSet<GraphEdge>();

            foreach (GraphTriangle triangle in badTriangles)
            {
                bool contains1 = false;
                bool contains2 = false;
                bool contains3 = false;
                foreach (GraphTriangle triangle1 in badTriangles)
                {
                    if (!triangle.Equals(triangle1))
                    {

                        if (triangle.edge1.Equals(triangle1.edge1) || triangle.edge1.Equals(triangle1.edge2) || triangle.edge1.Equals(triangle1.edge3))
                        {
                            contains1 = true;
                        }
                        if (triangle.edge2.Equals(triangle1.edge1) || triangle.edge2.Equals(triangle1.edge2) || triangle.edge2.Equals(triangle1.edge3))
                        {
                            contains2 = true;
                        }
                        if (triangle.edge3.Equals(triangle1.edge1) || triangle.edge3.Equals(triangle1.edge2) || triangle.edge3.Equals(triangle1.edge3))
                        {
                            contains3 = true;
                        }
                    }


                }

                if (!contains1)
                    polygon.Add(triangle.edge1);
                if (!contains2)
                    polygon.Add(triangle.edge2);
                if (!contains3)
                    polygon.Add(triangle.edge3);


            }

            foreach (GraphTriangle triangle in badTriangles)
            {
                triangulation.Remove(triangle);
            }

            foreach (GraphEdge edge in polygon)
            {
                triangulation.Add(new GraphTriangle(new GraphVertex(roomPositions_[i]), edge.a, edge.b));
            }

        }

        for (int i = triangulation.Count - 1; i >= 0; i--)
        {
            if (triangulation[i].a == superTriangle.a || triangulation[i].a == superTriangle.b || triangulation[i].a == superTriangle.c ||
                    triangulation[i].b == superTriangle.a || triangulation[i].b == superTriangle.b || triangulation[i].b == superTriangle.c ||
                    triangulation[i].c == superTriangle.a || triangulation[i].c == superTriangle.b || triangulation[i].c == superTriangle.c)
            {
                triangulation.RemoveAt(i);
            }
        }

        return triangulation;


    }

    //todo: only visivle without editor reload
    public static void DrawTriangles(List<GraphTriangle> triangleList)
    {
        for (int i = 0; i < triangleList.Count; i++)
        {
            Vector3 vertex1 = new Vector3(triangleList[i].a.position.x, 1.0f, triangleList[i].a.position.y) * 10;
            Vector3 vertex2 = new Vector3(triangleList[i].b.position.x, 1.0f, triangleList[i].b.position.y) * 10;
            Vector3 vertex3 = new Vector3(triangleList[i].c.position.x, 1.0f, triangleList[i].c.position.y) * 10;

            Debug.DrawLine(vertex1, vertex2, Color.red, 0.1f);
            Debug.DrawLine(vertex2, vertex3, Color.red, 0.1f);
            Debug.DrawLine(vertex3, vertex1, Color.red, 0.1f);
        }
    }

    private static Vector2 CalculateCircumcircleCenter(GraphTriangle t)
    {
        Vector2 a = t.a.position;
        Vector2 b = t.b.position;
        Vector2 c = t.c.position;

        float d = 2.0f * (a.x * (b.y - c.y) + b.x * (c.y - a.y) + c.x * (a.y - b.y));

        float x = ((a.x * a.x + a.y * a.y) * (b.y - c.y) + (b.x * b.x + b.y * b.y) * (c.y - a.y) +
            (c.x * c.x + c.y * c.y) * (a.y - b.y)) / d;

        float y = ((a.x * a.x + a.y * a.y) * (c.x - b.x) + (b.x * b.x + b.y * b.y) * (a.x - c.x) +
            (c.x * c.x + c.y * c.y) * (b.x - a.x)) / d;

        return new Vector2(x, y);
    }

    private static float CalculateCircumcircleRadius(GraphTriangle t)
    {
        float lenAB = Vector2.Distance(t.a.position, t.b.position);
        float lenBC = Vector2.Distance(t.b.position, t.c.position);
        float lenCA = Vector2.Distance(t.c.position, t.a.position);


        float prod = lenAB * lenBC * lenCA;

        float s = (lenAB + lenBC + lenCA) / 2.0f;
        float area = Mathf.Sqrt(s * (s - lenAB) * (s - lenBC) * (s - lenCA));

        float circleRadius = prod / (4 * area);

        return circleRadius;
    }




}

