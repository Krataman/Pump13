using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDebug : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemyAI enemyAI;
    public Transform player;
    private PlayerController playerController;

    private void Start()
    {
        if (enemyAI == null)
        {
            Debug.LogError("EnemyAI reference is missing!");
        }

        if (player == null)
        {
            Debug.LogError("Player reference is missing!");
        }
        else
        {
            playerController = player.GetComponent<PlayerController>();
        }
    }

    private void OnGUI()
    {
        if (enemyAI == null || player == null) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 16;
        style.richText = true;

        GUILayout.BeginArea(new Rect(10, 10, 300, 400));
        GUILayout.Label("<color=white>--- Enemy AI Debug Info ---</color>", style);
        GUILayout.Label($"<color=green>Player Position: {player.position} </color>", style);
        GUILayout.Label($"<color=green>Last Player Position: {enemyAI.lastPlayerPosition} </color>", style);

        GUILayout.Label($"<color=red>Enemy Position: {enemyAI.transform.position} </color>", style);
        GUILayout.Label($"<color=red>Enemy Destination: {enemyAI.GetComponent<NavMeshAgent>().destination} </color>", style);

        GUILayout.Label($"<color=blue>Enemy Speed: {enemyAI.GetComponent<NavMeshAgent>().speed} </color>", style);
        GUILayout.Label($"<color=blue>Patrolling speed: {enemyAI.patrolingSpeed} </color>", style);
        GUILayout.Label($"<color=blue>Chase Speed: {enemyAI.chaseSpeed} </color>", style);

        GUILayout.Label($"<color=yellow>Is Suspicious: {enemyAI.isSuspicious} </color>", style);
        GUILayout.Label($"<color=yellow>Is Suspicion Triggered: {enemyAI.isSuspicionTriggered} </color>", style);
        GUILayout.Label($"<color=yellow>Do PATROLLING: {enemyAI.doPatrolling} </color>", style);
        GUILayout.Label($"<color=yellow>Is Stealth Mode: {playerController?.isStealth} </color>", style);

        GUILayout.EndArea();
    }
}
