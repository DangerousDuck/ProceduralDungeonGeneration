using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonGeneration))]
public class DungeonGenerationEditor : Editor
{

    bool hideCollisionBoxes = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DungeonGeneration dungeonGeneration = target as DungeonGeneration;

        hideCollisionBoxes = EditorGUILayout.Toggle("Hide Collision Boxes", hideCollisionBoxes);

        if(hideCollisionBoxes)
        {
            dungeonGeneration.DisableCollisionBoxesGO();
        }
        else
        {
            dungeonGeneration.EnableCollisionBoxesGO();
        }

        if (GUILayout.Button("Generate new Dungeon"))
        {
            dungeonGeneration.GenerateDungeon();
        }

        if(GUILayout.Button("Clear"))
        {
            dungeonGeneration.ClearDungeonGeneration();
        }
    }


}
