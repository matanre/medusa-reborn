using UnityEngine;

public class GizmoIndicator : MonoBehaviour
{
    public Color color = Color.blue;
    public float size = 0.2f;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, size);
    }
}