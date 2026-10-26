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

    private static void OnSceneGUI(SceneView sceneView)
    {
        Alertness[] alertEnemies = Object.FindObjectsByType<Alertness>(FindObjectsSortMode.None);

        foreach (var enemy in alertEnemies)
        {
            if (enemy == null) continue;
            if (enemy.sight == null) continue;

            // Choose color based on enemy state
            // TODO: this should be on a different script; ALERT Stage and FOV should be decoupled

            Color c = Color.green;
            if (enemy.alertStage == AlertStage.Intrigued)
                c = Color.Lerp(Color.green, Color.red, enemy.alertLevel / 100f);
            else if (enemy.alertStage == AlertStage.Alerted)
                c = Color.red;

            FieldOfView fov = enemy.sight.fov;

            Handles.color = new Color(c.r, c.g, c.b, 0.1f);
            Handles.DrawSolidArc(
                fov.Position,
                fov.Transform.up,
                Quaternion.AngleAxis(-fov.Angle / 2f, enemy.transform.up) * enemy.transform.forward,
                fov.Angle,
                fov.Distance);

            //Handles.color = c;
            //enemyFOV.radius = Handles.ScaleValueHandle(
            //    enemyFOV.radius,
            //    enemyFOV.transform.position + enemyFOV.transform.forward * enemyFOV.radius,
            //    enemyFOV.transform.rotation,
            //    3,
            //    Handles.SphereHandleCap,
            //    1);
        }
    }
}
#endif