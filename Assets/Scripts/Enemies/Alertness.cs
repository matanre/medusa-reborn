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
    [Header("Components")]
    public EnemySight sight;

    [Header("State & Levels")]
    public AlertStage alertStage = AlertStage.Peaceful;
    [Range(0, 100)] public float alertLevel = 0f; // 0: Peaceful, 100: Fully Busted

    [Header("Design-Friendly Timing (In Seconds)")]
    [Tooltip("Time in seconds it takes to go from Peaceful (0) to Busted (100) while the player is continuously in sight.")]
    [SerializeField] private float timeToSpot = 2.0f;

    [Tooltip("Time in seconds it takes for the alert meter to fully drain back down to 0 after the player breaks line of sight.")]
    [SerializeField] private float timeToForget = 3.0f;

    public float AlertPercentage => alertLevel;


    private void Awake()
    {
        if (sight == null)
            sight = GetComponent<EnemySight>();

        alertStage = AlertStage.Peaceful;
        alertLevel = 0f;
    }

    private void Update()
    {
        bool playerInSight = sight.CanSeePlayer(out Transform playerTransform);
        UpdateAlertState(playerInSight, playerTransform);
    }

    private void UpdateAlertState(bool playerInSight, Transform playerTransform)
    {
        // Dynamically calculate speed based on user-defined seconds
        float buildRate = 100f / Mathf.Max(timeToSpot, 0.01f);
        float decayRate = 100f / Mathf.Max(timeToForget, 0.01f);

        if (playerInSight)
        {
            // Fill up the alert meter over time
            alertLevel += buildRate * Time.deltaTime;
            alertLevel = Mathf.Clamp(alertLevel, 0f, 100f);

            // Optional: Face the player when spotted
            Vector3 lookDir = (playerTransform.position - transform.position);
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 8f * Time.deltaTime);
            }
        }
        else
        {
            // Drain the alert meter when player is hidden
            alertLevel -= decayRate * Time.deltaTime;
            alertLevel = Mathf.Clamp(alertLevel, 0f, 100f);
        }

        // Determine Stage based on alertLevel thresholds
        AlertStage previousStage = alertStage;

        if (alertLevel >= 100f)
        {
            alertStage = AlertStage.Alerted; // BUSTED / Game Over
        }
        else if (alertLevel > 0f)
        {
            alertStage = AlertStage.Intrigued; // Suspicious / Yellow
        }
        else
        {
            alertStage = AlertStage.Peaceful; // Patrol / Green
        }

        // If stage changed, trigger any necessary behavior changes
        if (previousStage != alertStage)
        {
            HandleStageTransition(alertStage);
        }
    }

    private void HandleStageTransition(AlertStage newStage)
    {
        switch (newStage)
        {
            case AlertStage.Peaceful:
                Debug.Log($"{name}: Returned to Peaceful.");
                break;
            case AlertStage.Intrigued:
                Debug.Log($"{name}: Now Intrigued/Suspicious.");
                break;
            case AlertStage.Alerted:
                TriggerGameOver();
                break;
        }
    }

    private void TriggerGameOver()
    {
        Debug.LogWarning($"<color=red><b>[GAME OVER] {name} caught the player!</b></color>");
        // TODO: Hook up your game over UI or scene reload here
    }
}