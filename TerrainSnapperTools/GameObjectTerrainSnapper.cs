#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class GameObjectTerrainSnapper : EditorWindow
{
    // Align To Terrain
    private bool alignToTerrain = false;


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
        EditorGUILayout.LabelField("GameObject Terrain Snapper", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        // Help Box
        EditorGUILayout.HelpBox(
            "Select one or more GameObjects in the scene, " +
            "then snap their Transform position to the nearest Terrain.",
            MessageType.Info
        );

        EditorGUILayout.Space();

        // Align To Terrain Toggle
        alignToTerrain = EditorGUILayout.Toggle("Align To Terrain", alignToTerrain);

        EditorGUILayout.Space();

        // Snap Selected GameObjects
        if (GUILayout.Button(
            "Snap Selected GameObject(s) To Terrain",
            GUILayout.Height(35)))
        {
            SnapSelectedObjects(alignToTerrain);
        }
    }


    // Snap Selected Objects
    private static void SnapSelectedObjects(bool alignToTerrain)
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

            // Align Rotation To Terrain
            if (alignToTerrain)
            {
                AlignRotationToTerrain(target, terrain, worldPos);
            }

            objectCount++;
        }

        // Debug
        Debug.Log(
            $"GameObject Terrain Snapper: " +
            $"Snapped {objectCount} GameObject(s) to Terrain."
        );
    }


    // Align Object Rotation To Terrain Normal
    private static void AlignRotationToTerrain(Transform target, Terrain terrain, Vector3 worldPosition)
    {
        TerrainData terrainData = terrain.terrainData;

        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrainData.size;

        if (terrainSize.x <= 0f || terrainSize.z <= 0f) return;

        // Convert world position to normalized Terrain coordinates
        float normalizedX = (worldPosition.x - terrainPosition.x) / terrainSize.x;
        float normalizedZ = (worldPosition.z - terrainPosition.z) / terrainSize.z;

        normalizedX = Mathf.Clamp01(normalizedX);
        normalizedZ = Mathf.Clamp01(normalizedZ);

        // Get Terrain surface normal
        Vector3 localNormal = terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);

        // Convert normal to world space
        Vector3 worldNormal = terrain.transform.TransformDirection(localNormal).normalized;

        // Preserve the object's original forward direction as much as possible
        Vector3 originalForward = target.forward;

        // Project forward direction onto the terrain surface
        Vector3 projectedForward = Vector3.ProjectOnPlane(originalForward, worldNormal);

        // Handle cases where forward is almost parallel to the normal
        if (projectedForward.sqrMagnitude < 0.0001f)
        {
            projectedForward = Vector3.ProjectOnPlane(target.right, worldNormal);
        }

        // Final fallback
        if (projectedForward.sqrMagnitude < 0.0001f)
        {
            projectedForward = Vector3.ProjectOnPlane(Vector3.forward,worldNormal);
        }

        projectedForward.Normalize();

        // Align local Y axis to terrain normal
        // Preserve the original heading as much as possible
        Quaternion targetRotation = Quaternion.LookRotation(projectedForward,worldNormal);

        // Apply rotation
        target.rotation = targetRotation;
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