using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldOfView : MonoBehaviour
{
    [Header("Vision Cone Parameters")]
    public float distance = 10;
    [Range(0.1f, 5f)] public float height = 1f;
    [Range(0, 360)] public float angle = 30;
    public Color meshColor = Color.red;
    //[SerializeField] private int meshResolution = 30; // High count = smoother arc curve

    [Header("View over Walls / Obstacles")]
    [SerializeField] private LayerMask obstacleMask;

    private MeshFilter meshFilter;
    private Mesh mesh;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        //viewMesh = new Mesh { name = "FOV Mesh" };
        //meshFilter.mesh = viewMesh;
    }

    Mesh CreateWedgeMesh()
    {
        var mesh = new Mesh() { name = "Enemy FoV mesh" };

        int triangleCount = 8;
        int vertexCount = triangleCount * 3;

        var vertices = new Vector3[vertexCount];
        var triangles = new int[vertexCount];

        var bottomCenter = Vector3.zero;
        var bottomLeft   = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        var bottomRight  = Quaternion.Euler(0, +angle, 0) * Vector3.forward * distance;

        var topCenter = bottomCenter + Vector3.up * height;
        var topLeft   = bottomLeft + Vector3.up * height;
        var topRight  = bottomRight + Vector3.up * height;

        return mesh;
    }

    private void Update()
    {

    }

    private void LateUpdate()
    {
    }
}