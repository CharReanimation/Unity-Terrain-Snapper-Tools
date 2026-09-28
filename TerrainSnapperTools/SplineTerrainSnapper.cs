#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

public class SplineTerrainSnapper : EditorWindow
{
    // Show Window
    [MenuItem("Tools/Terrain Snapper Tools/Spline Terrain Snapper")]
    public static void ShowWindow()
    {
        GetWindow<SplineTerrainSnapper>("Spline Terrain Snapper");
    }

    // On GUI
    private void OnGUI()
    {
        EditorGUILayout.Space();

        // Label
        EditorGUILayout.LabelField("Spline Terrain Snapper", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        // Help Box
        EditorGUILayout.HelpBox(
            "Select one or more Spline Containers in the scene, " +
            "then snap every Knot to the nearest Terrain.",
            MessageType.Info
        );

        EditorGUILayout.Space();

        // Snap Selected Spline To Terrain
        if (GUILayout.Button("Snap Selected Spline(s) To Terrain", GUILayout.Height(35)))
        {
            SnapSelectedSplines();
        }
    }


    // Snap Selected Splines
    private static void SnapSelectedSplines()
    {
        Object[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("Please select at least one Spline Container.");
            return;
        }

        Terrain[] terrains = Terrain.activeTerrains;

        if (terrains == null || terrains.Length == 0)
        {
            Debug.LogWarning("No Terrain found in the scene.");
            return;
        }

        int containerCount = 0;
        int knotCount = 0;

        foreach (GameObject selectedObject in selectedObjects)
        {
            SplineContainer container = selectedObject.GetComponent<SplineContainer>();
            if (container == null) continue;

            Undo.RecordObject(container, "Snap Spline Knots To Terrain" );

            for (int s = 0; s < container.Splines.Count; s++)
            {
                var spline = container.Splines[s];

                for (int i = 0; i < spline.Count; i++)
                {
                    BezierKnot knot = spline[i];
                    Vector3 worldPos = container.transform.TransformPoint(knot.Position);
                    Terrain terrain = FindNearestTerrain(worldPos, terrains);

                    if (terrain == null) continue;

                    worldPos.y = terrain.SampleHeight(worldPos) + terrain.transform.position.y;
                    knot.Position = container.transform.InverseTransformPoint(worldPos);

                    // Round local Y to 2 decimal places
                    Vector3 localPos = knot.Position;
                    localPos.y = Mathf.Round(localPos.y * 100f) / 100f;
                    knot.Position = localPos;

                    spline[i] = knot;
                    knotCount++;
                }
            }
            // Set Dirty
            EditorUtility.SetDirty(container);
            containerCount++;
        }

        // Debug
        Debug.Log(
            $"Spline Terrain Snapper: " +
            $"Snapped {knotCount} knots in {containerCount} spline container(s)."
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
            Vector3 terrainSize =  terrain.terrainData.size;

            float minX = terrainPosition.x;
            float maxX = terrainPosition.x + terrainSize.x;

            float minZ = terrainPosition.z;
            float maxZ = terrainPosition.z + terrainSize.z;

            // Knot is inside this Terrain
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
            Vector3 closestPoint =  new Vector3(closestX, worldPosition.y, closestZ);
            float distance = Vector3.SqrMagnitude( worldPosition - closestPoint);

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