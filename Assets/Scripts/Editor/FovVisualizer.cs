#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GlobalFOVVisualizer
{
    static GlobalFOVVisualizer()
    {
        // Subscribe to the Scene View update loop globally
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        FieldOfView[] enemiesFOVs = Object.FindObjectsByType<FieldOfView>(FindObjectsSortMode.None);

        foreach (FieldOfView enemyFOV in enemiesFOVs)
        {
            if (enemyFOV == null) continue;

            // Choose color based on enemy state
            // TODO: this should be on a different script; ALERT Stage and FOV should be decoupled

            Color c = Color.green;
            if (enemyFOV.alertStage == AlertStage.Intrigued)
                c = Color.Lerp(Color.green, Color.red, enemyFOV.alertLevel / 100f);
            else if (enemyFOV.alertStage == AlertStage.Alerted)
                c = Color.red;

            Handles.color = new Color(c.r, c.g, c.b, 0.1f);
            Handles.DrawSolidArc(
                enemyFOV.transform.position,
                enemyFOV.transform.up,
                Quaternion.AngleAxis(-enemyFOV.angle / 2f, enemyFOV.transform.up) * enemyFOV.transform.forward,
                enemyFOV.angle,
                enemyFOV.radius);

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