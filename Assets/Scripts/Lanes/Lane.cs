using System;
using UnityEngine;

[System.Serializable]
public class Lane : MonoBehaviour
{
    public int laneIndex;
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerSpawnPoint.position, 0.1f);
        Gizmos.DrawWireSphere(enemySpawnPoint.position, 0.1f);
    }
}