#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class EnemySightVisualizer
{
    static EnemySightVisualizer()
    {
        // Subscribe to the Scene View update loop globally
        SceneView.duringSceneGui += OnSceneGUI;
    }

    public static Vector3 GetViewArcLeftBoundary(this EnemySight sight)
    {
        Transform sightTransform = sight.fov.Transform;
        return Quaternion.AngleAxis(-sight.fov.Angle / 2f, sightTransform.up) * sightTransform.forward;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        // Fixed FindObjectsByType with required inactivity sort mode
        Alertness[] alertEnemies = Object.FindObjectsByType<Alertness>(FindObjectsSortMode.None);

        foreach (var enemy in alertEnemies)
        {
            if (enemy == null || enemy.sight == null) continue;

            // Choose color based on enemy alert state
            Color c = Color.green;
            if (enemy.alertStage == AlertStage.Intrigued)
                c = Color.Lerp(Color.green, Color.red, enemy.AlertPercentage / 100f);
            else if (enemy.alertStage == AlertStage.Alerted)
                c = Color.red;

            // Access data directly from EnemySight component
            EnemySight sight = enemy.sight;
            Transform sightTransform = sight.fov.Transform;
            Vector3 fovOriginPosition = sight.fov.Position;

            Handles.color = new Color(c.r, c.g, c.b, 0.15f);

            // Draw the solid field of view arc in the Scene View
            Handles.DrawSolidArc(
                fovOriginPosition,
                sightTransform.up,
                sight.GetViewArcLeftBoundary(),
                sight.fov.Angle,
                sight.fov.Distance);
        }
    }
}
#endif