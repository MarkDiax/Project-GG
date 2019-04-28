using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteInEditMode]
public class RopeCreator : EditorWindow
{
    [MenuItem("Tools/Rope/Create")]
    static void CreateRope() {
        GameObject clone = PrefabUtility.InstantiatePrefab(Selection.activeObject as GameObject) as GameObject;
        RopeBehaviour rope = clone.gameObject.GetComponentInChildren<RopeBehaviour>();
        rope.CreateRope();
    }

    [MenuItem("Tools/Rope/Create", true)]
    static bool ValidateCreateRope() {
        GameObject go = Selection.activeObject as GameObject;
        if (go == null)
            return false;

        return PrefabUtility.GetPrefabType(go) == PrefabType.Prefab || PrefabUtility.GetPrefabType(go) == PrefabType.PrefabInstance || PrefabUtility.GetPrefabType(go) == PrefabType.DisconnectedPrefabInstance;
    }
}
