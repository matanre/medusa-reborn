
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FateThread : MonoBehaviour
{
    [Header("Thread Path")]
    [SerializeField] private Transform[] controlPoints;

    [Header("Thread Shape")]
    [SerializeField, Min(1f)] private float segmentsPerUnit = 10f;
    [SerializeField] private float sagAmount = 0.08f;
    [SerializeField] private float maxSag = 0.8f;

    [Header("Wave Settings")]
    [SerializeField] private float wavesPerUnit = 0.5f;
    [SerializeField] private float waveAmplitude = 0.1f;
    [SerializeField] private float waveSpeed = 1f;

    [Header("Floor Detection")]
    [SerializeField] private float floorHeight = 0f;
    [SerializeField] private float floorThreshold = 0.25f;
    [SerializeField] private float floorOffset = 0.03f;

    [Header("Tension")]
    [SerializeField] private float tightenSpeed = 3f;
    [SerializeField] private bool startTight = false;

    private LineRenderer line;
    private float tension;
    private float targetTension;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = true;

        tension = startTight ? 1f : 0f;
        targetTension = tension;
    }

    private void LateUpdate()
    {
        if (controlPoints == null || controlPoints.Length < 2)
            return;

        foreach (Transform point in controlPoints)
        {
            if (point == null)
                return;
        }

        // Smoothly transition between loose and tight.
        tension = Mathf.MoveTowards(
            tension,
            targetTension,
            Mathf.Max(0f, tightenSpeed) * Time.deltaTime
        );

        // Calculate total vertex count.
        int totalPoints = 1;

        for (int section = 0; section < controlPoints.Length - 1; section++)
        {
            float distance = Vector3.Distance(
                controlPoints[section].position,
                controlPoints[section + 1].position
            );

            totalPoints += GetSegmentCount(distance);
        }

        line.positionCount = totalPoints;

        int index = 0;
        float distanceAlongThread = 0f;

        for (int section = 0; section < controlPoints.Length - 1; section++)
        {
            Vector3 start = controlPoints[section].position;
            Vector3 end = controlPoints[section + 1].position;

            float distance = Vector3.Distance(start, end);
            int segments = GetSegmentCount(distance);

            // Detect sections lying near floor level.
            bool isOnFloor =
                Mathf.Abs(start.y - floorHeight) <= floorThreshold &&
                Mathf.Abs(end.y - floorHeight) <= floorThreshold;

            // Longer sections sag more, up to maxSag.
            float sectionSag = Mathf.Min(
                distance * sagAmount,
                maxSag
            );

            // Direction perpendicular to the section in XZ.
            Vector3 direction = end - start;
            direction.y = 0f;

            Vector3 sideways = direction.sqrMagnitude > 0.0001f
                ? Vector3.Cross(Vector3.up, direction.normalized)
                : Vector3.right;

            for (int i = 0; i <= segments; i++)
            {
                // Avoid duplicate vertices at shared points.
                if (section > 0 && i == 0)
                    continue;

                float t = i / (float)segments;
                Vector3 pos = Vector3.Lerp(start, end, t);

                // Zero displacement at control points.
                float envelope = Mathf.Sin(t * Mathf.PI);
                float looseness = 1f - tension;

                // Continuous wave phase across sections.
                float traveledDistance =
                    distanceAlongThread + distance * t;

                float wave = Mathf.Sin(
                    traveledDistance * wavesPerUnit *
                    Mathf.PI * 2f +
                    Time.time * waveSpeed
                );

                if (isOnFloor)
                {
                    // Snake across the XZ plane.
                    pos += sideways *
                           wave *
                           waveAmplitude *
                           envelope *
                           looseness;

                    // Raise intermediate vertices slightly
                    // to avoid clipping into the floor.
                    pos.y += floorOffset *
                             envelope *
                             looseness;
                }
                else
                {
                    // Hanging thread: downward sag.
                    pos.y -= envelope *
                             sectionSag *
                             looseness;

                    // Gentle sideways movement.
                    pos.z += wave *
                             waveAmplitude *
                             envelope *
                             looseness;
                }

                line.SetPosition(index++, pos);
            }

            distanceAlongThread += distance;
        }
    }

    private int GetSegmentCount(float distance)
    {
        return Mathf.Max(
            1,
            Mathf.CeilToInt(distance * segmentsPerUnit)
        );
    }

    public void Tighten()
    {
        targetTension = 1f;
    }

    public void Loosen()
    {
        targetTension = 0f;
    }
}
