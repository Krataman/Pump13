using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting; // Pro debug vizualizaci

public class EnemyAI : MonoBehaviour
{
    public Transform player;

    [Header("Vision Settings")]
    public float visionRange = 10f;
    public float visionAngle = 45f;

    [Header("Sound Detection Settings")]
    public float normalSoundRange = 7f;
    public float stealthSoundRange = 3f;
    public float chaseSpeed = 4f;

    [Header("Suspicion Settings")]
    public float suspicionReactionTime = 1f; // Time to react suspiciously before going back to normal
    public bool isSuspicious = false;
    private bool isSuspicionTriggered = false;
    private bool isSuspicionEnded = false;
    public bool isChasing = false;

    [Header("Debug Colors")]
    public Color visionColor = Color.red;         // Barva kužole vidění
    public Color visionRangeColor = Color.yellow; // Barva kruhu maximálního vidění
    public Color normalSoundColor = Color.blue;   // Barva normálního zvukového radiusu
    public Color stealthSoundColor = Color.cyan;  // Barva stealth radiusu

    private NavMeshAgent agent;
    private PlayerController playerController;
    public Vector3 lastPlayerPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerController = player.GetComponent<PlayerController>();
        lastPlayerPosition = player.position;
    }

    void Update()
    {
        if (!DetectPlayerVision())
        {
            DetectPlayerSound();
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            // Zkontrolujte, zda agent skutečně dosáhl cíle (je na poslední pozici)
            if (!agent.pathPending)
            {
                // Tady spustíte nějakou akci, například:
                Debug.Log("Agent dorazil na cílovou pozici!");

                // Můžete volitelně zavolat nějakou metodu nebo spustit nějakou logiku
                PerformActionWhenReachedDestination();
            }
        }
    }

    bool DetectPlayerVision()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange);
        foreach (Collider hit in hits)
        {
            if (hit.transform == player)
            {
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToPlayer);

                if (angle < visionAngle)
                {
                    RaycastHit rayHit;
                    if (Physics.Raycast(transform.position, directionToPlayer, out rayHit, visionRange))
                    {
                        if (rayHit.transform == player)
                        {
                            StartChase();
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    void DetectPlayerSound()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool isPlayerMoving = Vector3.Distance(lastPlayerPosition, player.position) > 0.5f;

        // If the player is standing still and not in stealth, do not trigger the chase
        if (!isPlayerMoving && !playerController.isStealth)
        {
            return; // Exit without doing anything, no chase or movement
        }

        // If player enters stealth radius, immediately hear them
        if (distance < stealthSoundRange)
        {
            StartChase();
            return;
        }

        // Normal sound detection when player is moving and not in stealth
        if (distance < normalSoundRange && isPlayerMoving && !playerController.isStealth)
        {
            if (!isSuspicionTriggered)
            {
                // Trigger suspicion reaction (look around)
                isSuspicionTriggered = true;
                StartCoroutine(SuspicionReaction());
            }

            // Don't move towards the player until the suspicion is resolved
            if (!isSuspicious)
            {
                return; // Skip chasing if still in suspicion
            }

            StartChase();
        }

        lastPlayerPosition = player.position;
    }

    void StartChase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        isChasing = true;
    }

    IEnumerator SuspicionReaction()
    {
        // Rotate left and right a few times to simulate suspicion
        float rotationSpeed = 45f; // Degrees per second
        float timeElapsed = 0f;
        bool playerStoppedMoving = false;

        while (timeElapsed < suspicionReactionTime / 2f)
        {
            // Check if the player has stopped moving
            if (Vector3.Distance(lastPlayerPosition, player.position) < 1f)
            {
                playerStoppedMoving = true;
            }

            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Pause for a moment to simulate the enemy thinking
        yield return new WaitForSeconds(0.2f);

        // Turn right (back to original direction)
        timeElapsed = 0f;
        while (timeElapsed < suspicionReactionTime / 2f)
        {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // If player stopped moving during the suspicion reaction, do not chase
        if (playerStoppedMoving)
        {
            isSuspicionTriggered = false;
            isSuspicious = false; // Do not chase if player has stopped moving
        }
        else
        {
            // After suspicion, start chasing
            isSuspicionTriggered = false;
            isSuspicious = true; // Now the enemy is aware and in a suspicious state
        }
    }

    void PerformActionWhenReachedDestination()
    {
        StopChase();
    }

    void StopChase()
    {
        agent.speed = 0;
        isSuspicious = false;
        isChasing = false;
    }

    // Debug drawing for visualizing detection zones
    void OnDrawGizmos()
    {
        if (player == null) return;

        // Vision range circle (kruh maximální vzdálenosti vidění)
        Gizmos.color = visionRangeColor;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Zorné pole (kužel vidění)
        Gizmos.color = visionColor;
        Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, visionAngle, 0) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * visionRange);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * visionRange);

#if UNITY_EDITOR
        Handles.color = new Color(visionColor.r, visionColor.g, visionColor.b, 0.2f);
        Handles.DrawSolidArc(transform.position, Vector3.up, leftBoundary, visionAngle * 2, visionRange);
#endif

        // Detekční zvukové oblasti
        Gizmos.color = normalSoundColor;
        Gizmos.DrawWireSphere(transform.position, normalSoundRange);
        Gizmos.color = stealthSoundColor;
        Gizmos.DrawWireSphere(transform.position, stealthSoundRange);
    }
}
