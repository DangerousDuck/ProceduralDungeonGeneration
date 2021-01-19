using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoom
{

    public Vector3 Position { get; set; }
    public Vector3 Size { get; set; }
    public int Id { get; set; }

    public float FloorArea
    {
        get { return Size.x * 2.0f + Size.y * 2.0f +  Size.z * 2.0f; }
    }

    /*
    public DungeonRoom(Vector3 position_, Vector3 size_)
    {
        position = position_;
        size = size_;
    }
    */

    public DungeonRoom(int id_, Vector3 position_, Vector3 size_)
    {
        Id = id_;
        Position = position_;
        Size = size_;
    }

}
