using UnityEngine;

public class AxisRotator : MonoBehaviour
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    [Header("Rotation Settings")]
    [SerializeField] private RotationAxis axis = RotationAxis.Z;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private Space rotationSpace = Space.Self;

    private void Update()
    {
        if (Mathf.Approximately(rotationSpeed, 0f))
            return;

        Vector3 rotationAxis = axis switch
        {
            RotationAxis.X => Vector3.right,
            RotationAxis.Y => Vector3.up,
            RotationAxis.Z => Vector3.forward,
            _ => Vector3.forward
        };

        transform.Rotate(
            rotationAxis,
            rotationSpeed * Time.deltaTime,
            rotationSpace
        );
    }

    public void SetSpeed(float speed)
    {
        rotationSpeed = speed;
    }

    public void StopRotation()
    {
        rotationSpeed = 0f;
    }
}
