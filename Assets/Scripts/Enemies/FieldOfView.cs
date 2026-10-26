using UnityEngine;
using System;

public class FieldOfView : MonoBehaviour
{
    [Range(1, 20)] public float radius = 8f;
    [Range(0, 360)] public float horizontalAngle = 100f; // in degrees
    public Transform viewOrigin;

    public float Angle => horizontalAngle;
    public float Distance => radius;
    public Transform Transform => viewOrigin;
    public Vector3 Forward => viewOrigin.forward;
    public Vector3 Position => viewOrigin.position;
}
