using UnityEditor;
using UnityEngine;

public enum AlertStage
{
    Peaceful,
    Intrigued,
    Alerted
}

[RequireComponent(typeof(EnemySight))]
public class Alertness : MonoBehaviour
{
    public EnemySight sight;
    public AlertStage alertStage;
    [Range(0, 100)] public float alertLevel; // 0: Peaceful, 100: Alerted

    private void Awake()
    {
        if (sight == null)
            sight = GetComponent<EnemySight>();

        alertStage = AlertStage.Peaceful;
        alertLevel = 0;
    }

    private void Update()
    {
        bool playerInSight = sight.CanSeePlayer(out _);
        UpdateAlertState(playerInSight);
    }

    private void UpdateAlertState(bool playerInSight)
    {
        switch (alertStage)
        {
            case AlertStage.Peaceful:
                if (playerInSight)
                    alertStage = AlertStage.Intrigued;
                break;
            case AlertStage.Intrigued:
                if (playerInSight)
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
                if (!playerInSight)
                    alertStage = AlertStage.Intrigued;
                break;
        }
    }
}
