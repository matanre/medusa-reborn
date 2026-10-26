using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum AlertStage
{
    Peaceful,
    Intrigued,
    Alerted
}

public class FieldOfView : MonoBehaviour
{
    [Range(1, 20)] public float radius = 8f;
    [Range(0, 360)] public float angle = 100f; // in degrees

    public AlertStage alertStage;
    [Range(0, 100)] public float alertLevel; // 0: Peaceful, 100: Alerted

    private void Awake()
    {
        alertStage = AlertStage.Peaceful;
        alertLevel = 0;
    }

    //private void OnDrawGizmos()
    //{
    //    float alpha = 0.3f;
    //    Color shadyGreen = Color.green * new Color(1, 1, 1, alpha); ;
    //    Handles.color = shadyGreen;
    //    Handles.DrawSolidDisc(transform.position, transform.up, fovRadius);
    //}

    private void Update()
    {
        bool playerInFOV = false;
        Collider[] targetsInFOV = Physics.OverlapSphere(
            transform.position, radius);
        foreach (Collider c in targetsInFOV)
        {
            if (c.CompareTag("Player"))
            {
                float signedAngle = Vector3.Angle(
                    transform.forward,
                    c.transform.position - transform.position);
                if (Mathf.Abs(signedAngle) < angle / 2)
                    playerInFOV = true;
                break;
            }
        }

        _UpdateAlertState(playerInFOV);
    }

    private void _UpdateAlertState(bool playerInFOV)
    {
        switch (alertStage)
        {
            case AlertStage.Peaceful:
                if (playerInFOV)
                    alertStage = AlertStage.Intrigued;
                break;
            case AlertStage.Intrigued:
                if (playerInFOV)
                {
                    alertLevel++;
                    if (alertLevel >= 100)
                        alertStage = AlertStage.Alerted;
                }
                else
                {
                    alertLevel--;
                    if (alertLevel <= 0)
                        alertStage = AlertStage.Peaceful;
                }
                break;
            case AlertStage.Alerted:
                if (!playerInFOV)
                    alertStage = AlertStage.Intrigued;
                break;
        }
    }

}
