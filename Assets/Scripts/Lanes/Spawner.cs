using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; }

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

    public GameUnit SpawnEnemy(UnitData enemyData, int laneIndex)
    {
        ++GameManager.Instance.currentAliveEnemies;
        return SpawnUnit(enemyData, laneIndex, UnitOwner.Enemy);
    }
    
    public GameUnit SpawnPlayerUnit(UnitData playerData, int laneIndex)
    {
        AnalyticsManager.Instance.TrackUnitSpawned(playerData.unitType.ToString(),
            laneIndex,
            playerData.goldCost, 
            PlayerManager.Instance.playerGold.CurrentBalance);
        ++PlayerManager.Instance.TotalUnitsSpawned;
        ++GameManager.Instance.currentAlivePlayers;
        return SpawnUnit(playerData, laneIndex, UnitOwner.Player);
    }

    private GameUnit SpawnUnit(UnitData unitData, int laneIndex, UnitOwner unitOwner)
    {
        Vector3 spawnPosition = GetSpawnPosition(laneIndex, unitOwner);
        Quaternion spawnRotation = GetSpawnRotation(unitOwner);

        GameUnit gameUnit = CreateUnit(unitData, spawnPosition, spawnRotation);
        ConfigureUnit(gameUnit, unitData, laneIndex, unitOwner);

        LogUnitSpawn(unitData, laneIndex, unitOwner);

        return gameUnit;
    }
    
    private Vector3 GetSpawnPosition(int laneIndex, UnitOwner unitOwner)
    {
        if (unitOwner == UnitOwner.Player)
        {
            return LaneManager.Instance.GetPlayerSpawnPoint(laneIndex);
        }
    
        return LaneManager.Instance.GetEnemySpawnPoint(laneIndex);
    }

    private Quaternion GetSpawnRotation(UnitOwner unitOwner)
    {
        // Enemy units face forward (0 degrees), player units face backward (180 degrees)
        return unitOwner == UnitOwner.Enemy 
            ? Quaternion.Euler(0f, -180f, 0f)
            : Quaternion.Euler(0f, 0f, 0f);
    }

    private GameUnit CreateUnit(UnitData unitData, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject unitObject = Instantiate(GameUtils.GetPrefabByUnitData(unitData), spawnPosition, spawnRotation);
        return unitObject.GetComponent<GameUnit>();
    }

    private void ConfigureUnit(GameUnit gameUnit, UnitData unitData, int laneIndex, UnitOwner unitOwner)
    {
        gameUnit.SetUnitData(unitData);
        gameUnit.isPlayerUnit = (unitOwner == UnitOwner.Player);
        gameUnit.laneIndex = laneIndex;
    }

    private void LogUnitSpawn(UnitData unitData, int laneIndex, UnitOwner unitOwner)
    {
        Debug.Log($"Spawned {unitData.unitType} ({unitOwner.ToString().ToLower()}) in lane {laneIndex}");
    }

    private enum UnitOwner
    {
        Player,
        Enemy
    }
}