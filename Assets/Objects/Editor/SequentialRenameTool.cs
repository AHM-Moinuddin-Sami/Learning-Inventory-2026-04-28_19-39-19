using UnityEditor;
using UnityEngine;

public class SequentialRenameWindow : EditorWindow
{
    private string baseName = "Square";
    private int startNumber = 1;

    [MenuItem("Tools/Sequential Rename")]
    private static void OpenWindow()
    {
        GetWindow<SequentialRenameWindow>("Sequential Rename");
    }

    private void OnGUI()
    {
        GUILayout.Label("Rename Selected GameObjects", EditorStyles.boldLabel);

        baseName = EditorGUILayout.TextField("Base Name", baseName);
        startNumber = EditorGUILayout.IntField("Start Number", startNumber);

        if (GUILayout.Button("Rename Selected"))
        {
            RenameSelectedObjects();
        }
    }

    private void RenameSelectedObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No GameObjects selected.");
            return;
        }

        System.Array.Sort(selectedObjects, CompareHierarchyOrder);

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            Undo.RecordObject(selectedObjects[i], "Sequential Rename");

            selectedObjects[i].name = baseName + " " + (startNumber + i);

            EditorUtility.SetDirty(selectedObjects[i]);
        }
    }

    private int CompareHierarchyOrder(GameObject a, GameObject b)
    {
        if (a.transform.parent == b.transform.parent)
        {
            return a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex());
        }

        return string.Compare(a.name, b.name, System.StringComparison.Ordinal);
    }
}