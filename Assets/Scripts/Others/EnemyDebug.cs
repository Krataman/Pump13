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
        style.normal.textColor = Color.white;

        GUILayout.BeginArea(new Rect(10, 10, 300, 400));
        GUILayout.Label("--- Enemy AI Debug Info ---", style);
        GUILayout.Label($"Player Position: {player.position}", style);
        GUILayout.Label($"Enemy Position: {enemyAI.transform.position}", style);
        GUILayout.Label($"Enemy Destination: {enemyAI.GetComponent<NavMeshAgent>().destination}", style);
        GUILayout.Label($"Enemy Speed: {enemyAI.GetComponent<NavMeshAgent>().speed}", style);
        GUILayout.Label($"Vision Range: {enemyAI.visionRange}", style);
        GUILayout.Label($"Vision Angle: {enemyAI.visionAngle}", style);
        GUILayout.Label($"Normal Sound Range: {enemyAI.normalSoundRange}", style);
        GUILayout.Label($"Stealth Sound Range: {enemyAI.stealthSoundRange}", style);
        GUILayout.Label($"Chase Speed: {enemyAI.chaseSpeed}", style);
        GUILayout.Label($"Is Suspicious: {enemyAI.isSuspicious}", style);
        GUILayout.Label($"Is Chasing: {enemyAI.isChasing}", style);
        GUILayout.Label($"Is Stealth Mode: {playerController?.isStealth}", style);
        GUILayout.Label($"Last Player Position: {enemyAI.lastPlayerPosition}", style);
        GUILayout.EndArea();
    }
}
