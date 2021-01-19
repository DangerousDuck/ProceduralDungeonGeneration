using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphStructure
{

    public List<GraphVertex> vertexList = new List<GraphVertex>();
    public List<GraphEdge> edgeList = new List<GraphEdge>(); 

    public GraphStructure()
    {

    }

    //TODO: use copy constructor or reference?
    // assumes that no two vertices are at the same position 
    public static GraphStructure GenerateGraphStructureFromTriangles(List<GraphTriangle> triangleList)
    {
        GraphStructure graphStructure = new GraphStructure();

        for (int i = 0; i < triangleList.Count; i++)
        {
            GraphVertex vertex1 = new GraphVertex(triangleList[i].a);
            GraphVertex vertex2 = new GraphVertex(triangleList[i].b);
            GraphVertex vertex3 = new GraphVertex(triangleList[i].c);



            if (!graphStructure.vertexList.Contains(vertex1))
            {
                graphStructure.vertexList.Add(vertex1);
            }
            else
            {
                vertex1 = graphStructure.vertexList.Find(x => vertex1.position.Equals(x.position));
            }

            if (!graphStructure.vertexList.Contains(vertex2))
            {
                graphStructure.vertexList.Add(vertex2);
            }
            else
            {
                vertex2 = graphStructure.vertexList.Find(x => vertex2.position.Equals(x.position));
            }

            if (!graphStructure.vertexList.Contains(vertex3))
            {
                graphStructure.vertexList.Add(vertex3);
            }
            else
            {
                vertex3 = graphStructure.vertexList.Find(x => vertex3.position.Equals(x.position));
            }

            GraphEdge edge1 = new GraphEdge(vertex1, vertex2);
            GraphEdge edge2 = new GraphEdge(vertex2, vertex3);
            GraphEdge edge3 = new GraphEdge(vertex3, vertex1);

            if (!graphStructure.edgeList.Contains(edge1))
                graphStructure.edgeList.Add(edge1);
            if (!graphStructure.edgeList.Contains(edge2))
                graphStructure.edgeList.Add(edge2);
            if (!graphStructure.edgeList.Contains(edge3))
                graphStructure.edgeList.Add(edge3);


        }

        for(int i = 0; i < graphStructure.edgeList.Count; i++ )
        {
            AddEdgeRefToVertList(graphStructure.vertexList, graphStructure.edgeList[i], i);

        }

        return graphStructure;
    }

    //https://de.wikipedia.org/wiki/Algorithmus_von_Kruskal
    public static GraphStructure GetMinimumSpanningTree(GraphStructure graphStructure)
    {
        GraphStructure msp = new GraphStructure();

        foreach(GraphVertex vertex in graphStructure.vertexList)
        {
            msp.vertexList.Add(new GraphVertex(vertex.position));
        }

        graphStructure.edgeList.Sort((x, y) => Vector2.Distance(x.a.position, x.b.position).CompareTo(Vector2.Distance(y.a.position, y.b.position)));

        List<GraphEdge> edges = graphStructure.edgeList;

        int addedEdgesSum = 0;
        for (int i = 0; i < edges.Count; i++)
        {
            int posA = graphStructure.vertexList.IndexOf(edges[i].a);
            int posB = graphStructure.vertexList.IndexOf(edges[i].b);

            GraphEdge newEdge = new GraphEdge(msp.vertexList[posA], msp.vertexList[posB]);

            msp.edgeList.Add(newEdge);
            AddEdgeRefToVertList(msp.vertexList, newEdge, addedEdgesSum);
            if(IsCyclic(msp))
            {
                msp.edgeList.RemoveAt(msp.edgeList.Count - 1);
                RemoveEdgeRefToVertList(msp.vertexList, newEdge);
            }
            else
            {
                addedEdgesSum++;
            }

        }

        return msp;
    }

    //basic idea: https://www.geeksforgeeks.org/detect-cycle-undirected-graph/
    private static bool IsCyclic(GraphStructure graph)
    {
        bool[] visited = new bool[graph.vertexList.Count];
        
        for(int i = 0; i < visited.Length; i++)
        {
            visited[i] = false;
        }

        for(int i = 0; i < visited.Length; i++)
        {
            if(!visited[i])
            {
                if (IsCyclicUtil(graph, i, visited, -1))
                    return true;
            }
        }

        return false;
    }

    //basic idea: https://www.geeksforgeeks.org/detect-cycle-undirected-graph/
    private static bool IsCyclicUtil(GraphStructure graph, int index,  bool[] visited, int parent)
    {
        visited[index] = true;

        foreach(int i in graph.vertexList[index].neighbourRefList)
        {

            int ind = graph.edgeList[i].a == graph.vertexList[index] ? graph.vertexList.IndexOf(graph.edgeList[i].b) : 
                graph.vertexList.IndexOf(graph.edgeList[i].a);

            if(!visited[ind])
            {
                if(IsCyclicUtil(graph, ind, visited, index))
                {
                    return true;
                }

            }

            else if( ind != parent)
            {
                return true;
            }
        }
        return false;
    }

    //todo: only visivle without editor reload
    public static void DrawEdges(GraphStructure graph)
    {
        foreach(GraphEdge edge in graph.edgeList)
        {
            Debug.DrawLine(new Vector3(edge.a.position.x, 1.1f, edge.a.position.y) * 10, new Vector3(edge.b.position.x, 1.1f, edge.b.position.y) * 10,
                Color.blue, 0.1f);
        }
    }

    private static void AddEdgeRefToVertList(List<GraphVertex> vertexList, GraphEdge edge, int index)
    {
        GraphVertex vertex = vertexList.Find(x => edge.a.position.Equals(x.position));
        vertex.neighbourRefList.Add(index);

        vertex = vertexList.Find(x => edge.b.position.Equals(x.position));
        vertex.neighbourRefList.Add(index);
    }

    private static void RemoveEdgeRefToVertList(List<GraphVertex> vertexList, GraphEdge edge)
    {

        GraphVertex vertex = vertexList.Find(x => edge.a.position.Equals(x.position));
        vertex.neighbourRefList.RemoveAt(vertex.neighbourRefList.Count - 1);


        vertex = vertexList.Find(x => edge.b.position.Equals(x.position));
        vertex.neighbourRefList.RemoveAt(vertex.neighbourRefList.Count - 1);

    }

}
