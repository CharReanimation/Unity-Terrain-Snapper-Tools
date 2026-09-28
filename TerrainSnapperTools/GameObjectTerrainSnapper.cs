#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class GameObjectTerrainSnapper : EditorWindow
{
    // Show Window
    [MenuItem("Tools/Terrain Snapper Tools/GameObject Terrain Snapper")]
    public static void ShowWindow()
    {
        GetWindow<GameObjectTerrainSnapper>("Terrain Snapper");
    }

    // On GUI
    private void OnGUI()
    {
        EditorGUILayout.Space();

        // Label
        EditorGUILayout.LabelField(
            "GameObject Terrain Snapper",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space();

        // Help Box
        EditorGUILayout.HelpBox(
            "Select one or more GameObjects in the scene, " +
            "then snap their Transform position to the nearest Terrain.",
            MessageType.Info
        );

        EditorGUILayout.Space();

        // Snap Selected GameObjects
        if (GUILayout.Button(
            "Snap Selected GameObject(s) To Terrain",
            GUILayout.Height(35)))
        {
            SnapSelectedObjects();
        }
    }


    // Snap Selected Objects
    private static void SnapSelectedObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("Please select at least one GameObject.");
            return;
        }

        Terrain[] terrains = Terrain.activeTerrains;

        if (terrains == null || terrains.Length == 0)
        {
            Debug.LogWarning("No Terrain found in the scene.");
            return;
        }

        int objectCount = 0;

        foreach (GameObject selectedObject in selectedObjects)
        {
            if (selectedObject == null)
                continue;

            Transform target = selectedObject.transform;

            Vector3 worldPos = target.position;

            Terrain terrain = FindNearestTerrain(worldPos, terrains);

            if (terrain == null)
                continue;

            // Sample Terrain Height
            worldPos.y = terrain.SampleHeight(worldPos) + terrain.transform.position.y;

            // Set Position
            Undo.RecordObject(target, "Snap GameObject To Terrain");

            target.position = worldPos;
            objectCount++;
        }

        // Debug
        Debug.Log(
            $"GameObject Terrain Snapper: " +
            $"Snapped {objectCount} GameObject(s) to Terrain."
        );
    }


    // Find Nearest Terrain
    private static Terrain FindNearestTerrain(Vector3 worldPosition, Terrain[] terrains)
    {
        Terrain nearestTerrain = null;
        float nearestDistance = float.MaxValue;

        foreach (Terrain terrain in terrains)
        {
            if (terrain == null) continue;

            Vector3 terrainPosition = terrain.transform.position;
            Vector3 terrainSize = terrain.terrainData.size;

            float minX = terrainPosition.x;
            float maxX = terrainPosition.x + terrainSize.x;

            float minZ = terrainPosition.z;
            float maxZ = terrainPosition.z + terrainSize.z;

            // Object is inside this Terrain
            if (worldPosition.x >= minX &&
                worldPosition.x <= maxX &&
                worldPosition.z >= minZ &&
                worldPosition.z <= maxZ)
            {
                return terrain;
            }

            // Find closest point on Terrain bounds
            float closestX = Mathf.Clamp(worldPosition.x, minX, maxX);
            float closestZ = Mathf.Clamp(worldPosition.z, minZ, maxZ);
            Vector3 closestPoint = new Vector3(closestX, worldPosition.y, closestZ);

            float distance = Vector3.SqrMagnitude(worldPosition - closestPoint);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTerrain = terrain;
            }
        }
        return nearestTerrain;
    }
}
#endif