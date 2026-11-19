using UnityEngine;

public class ThreadPositionGenerator : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform guideA;
    [SerializeField] private Transform guideB;
    [SerializeField] private Transform endPoint;

    private void LateUpdate()
    {
        line.positionCount = 4;

        line.SetPosition(0, startPoint.position);
        line.SetPosition(1, guideA.position);
        line.SetPosition(2, guideB.position);
        line.SetPosition(3, endPoint.position);
    }
}