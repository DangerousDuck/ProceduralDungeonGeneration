using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.gamasutra.com/blogs/AAdonaac/20150903/252889/Procedural_Dungeon_Generation_Algorithm.php
public class DungeonGeneration : MonoBehaviour
{
    public int RoomGenerationCount = 50; // not the final number of rooms
    public int RoomGenerationRadius = 10;
    public int MaxPhysicsSimulationSteps = 10000;
    public GameObject RoomGenerationPrefab;

    List<GameObject> roomGenerationGOList = new List<GameObject>();
    List<GameObject> finalRoomGOList = new List<GameObject>();

    // only for visualization
    List<GraphTriangle> graphTriangleList = new List<GraphTriangle>();
    GraphStructure minimumSpanningTree = new GraphStructure(); 

    List<Vector2> roomCenterList = new List<Vector2>();
    //List<DungeonRoom> roomFinalList = new List<DungeonRoom>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateDungeon()
    {

        ClearDungeonGeneration();

        GenerateRooms();

        Physics2D.simulationMode = SimulationMode2D.Script;

        // TODO: 1 in ~15 times the simulation stops after exactly step 24 or 25. Why?
        for(int i = 0; i < MaxPhysicsSimulationSteps; i++)
        {
            Physics2D.Simulate(Time.fixedDeltaTime);
   
            bool separationDone = true;

            for (int j = 0; j < roomGenerationGOList.Count; j++)
            {
                if (roomGenerationGOList[j].GetComponent<Rigidbody2D>().IsAwake())
                {
                    separationDone = false;
                }
            }
            if (separationDone)
            {
                //Debug.Log(i);
                break;
            }

        }

        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;


        roomGenerationGOList.Sort((x, y) => GetArea(x.transform).CompareTo(GetArea(y.transform)));

        float areaSum = 0.0f;
        for (int i = 0; i < roomGenerationGOList.Count; i++)
        {
            areaSum += GetArea(roomGenerationGOList[i].transform); 
        }

        float averageArea = areaSum / roomGenerationGOList.Count;

        float roomGenerationThreshold = averageArea * 1.25f;


        for (int i = roomGenerationGOList.Count - 1; i >= 0; i--)
        {

            if (GetArea(roomGenerationGOList[i].transform) > roomGenerationThreshold)
            {
                roomCenterList.Add(roomGenerationGOList[i].transform.position);
            }
            else
            {
                if (Application.isEditor)
                {
                    DestroyImmediate(roomGenerationGOList[i]);
                }
                else
                {
                    Destroy(roomGenerationGOList[i]);
                }

                roomGenerationGOList.RemoveAt(i);
            }

        }

        for (int i = 0; i < roomGenerationGOList.Count; i++)
        {
            GameObject room = GameObject.CreatePrimitive(PrimitiveType.Plane);
            room.transform.position = new Vector3(roomGenerationGOList[i].transform.position.x, 0.0f, roomGenerationGOList[i].transform.position.y) * 10.0f;
            room.transform.localScale = new Vector3(roomGenerationGOList[i].transform.localScale.x, roomGenerationGOList[i].transform.localScale.z,
                    roomGenerationGOList[i].transform.localScale.y);
            room.name = "Final Room Position";
            room.transform.parent = gameObject.transform;
            finalRoomGOList.Add(room);
        }

        graphTriangleList = DelaunyTriangulation.Triangulate(roomCenterList);
        DelaunyTriangulation.DrawTriangles(graphTriangleList);
        GraphStructure graph = GraphStructure.GenerateGraphStructureFromTriangles(graphTriangleList);
        minimumSpanningTree = GraphStructure.GetMinimumSpanningTree(graph);
        GraphStructure.DrawEdges(minimumSpanningTree);

    }


    private void GenerateRooms()
    {
        for(int i = 0; i < RoomGenerationCount; i++)
        {
            GameObject roomGO = Instantiate(RoomGenerationPrefab);
            Vector2 randomPositon = Random.insideUnitCircle * RoomGenerationRadius;

            Vector3 size = new Vector3(Random.Range(3, 10), 1.0f, Random.Range(3, 10));

            roomGO.transform.position = randomPositon; //new Vector3(randomPositon.x, 0, randomPositon.y);
            roomGO.transform.localScale = new Vector3(size.x, size.z, size.y);//new Vector3(Random.Range(1, 10), 1, Random.Range(1, 10));
            roomGO.transform.SetParent(gameObject.transform);

            Rigidbody2D rb = roomGO.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0.0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            roomGenerationGOList.Add(roomGO);

        }
    }

    public void DisableCollisionBoxesGO()
    {
        foreach(GameObject GO in roomGenerationGOList)
        {
            GO.SetActive(false);
        }
    }

    public void EnableCollisionBoxesGO()
    {
        foreach (GameObject GO in roomGenerationGOList)
        {
            GO.SetActive(true);
        }
    }

    public void ClearDungeonGeneration()
    {
        roomCenterList.Clear();

        DestroyGOList(roomGenerationGOList);
        DestroyGOList(finalRoomGOList);
    }

    private static float GetArea(Transform transform)
    {
        return transform.localScale.x * 2.0f + transform.localScale.y * 2.0f;
    }

    public static void DestroyGOList(List<GameObject> GOList)
    {
        if (Application.isEditor)
        {
            foreach (GameObject go in GOList)
            {
                DestroyImmediate(go);
            }
        }
        else
        {
            foreach (GameObject go in GOList)
            {
                Destroy(go);
            }
        }

        GOList.Clear();
    }
    
    private void OnValidate()
    {
        DelaunyTriangulation.DrawTriangles(graphTriangleList);
        GraphStructure.DrawEdges(minimumSpanningTree);
    }

}