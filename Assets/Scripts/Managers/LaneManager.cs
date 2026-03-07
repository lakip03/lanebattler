using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MovementBehavior
{
    public static LaneManager Instance { get; private set; }
    
    [SerializeField]private List<Lane> lanes = new List<Lane>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize() => lanes.ForEach((lane) => lane.laneIndex = lanes.IndexOf(lane));

    public Vector3 GetPlayerSpawnPoint(int laneIndex) => lanes[laneIndex].playerSpawnPoint.position;
    public Vector3 GetEnemySpawnPoint(int laneIndex) => lanes[laneIndex].enemySpawnPoint.position;
    
    public Lane GetLane(int laneIndex) => lanes[laneIndex];
}