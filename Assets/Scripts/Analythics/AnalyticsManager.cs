using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private bool _isInitialized = false;
    private float _sessionStartTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == Firebase.DependencyStatus.Available)
            {
                _isInitialized = true;
                _sessionStartTime = Time.realtimeSinceStartup;
                
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                
                Debug.Log("[Analytics] Firebase initialized successfully");
                TrackSessionStart();
            }
            else
            {
                Debug.LogError($"[Analytics] Firebase init failed: {task.Result}");
            }
        });
    }

    public void TrackSessionStart()
    {
        if (!_isInitialized) return;

        FirebaseAnalytics.LogEvent("session_start_custom", new Firebase.Analytics.Parameter[]
        {
            new Firebase.Analytics.Parameter("platform", Application.platform.ToString()),
            new Firebase.Analytics.Parameter("app_version", Application.version)
        });
    }

    public void TrackSessionEnd()
    {
        if (!_isInitialized) return;

        float sessionDuration = Time.realtimeSinceStartup - _sessionStartTime;
        FirebaseAnalytics.LogEvent("session_end_custom", new Firebase.Analytics.Parameter[]
        {
            new Firebase.Analytics.Parameter("session_duration_seconds", (double)sessionDuration)
        });
    }

    public void TrackWaveStart(int waveNumber, int playerGold, int playerBaseHp)
    {
        if (!_isInitialized) return;

        FirebaseAnalytics.LogEvent("wave_start", new Firebase.Analytics.Parameter[]
        {
            new Firebase.Analytics.Parameter("wave_number", (long)waveNumber),
            new Firebase.Analytics.Parameter("player_gold", (long)playerGold),
            new Firebase.Analytics.Parameter("player_base_hp", (long)playerBaseHp)
        });
    }

    public void TrackWaveComplete(int waveNumber, int playerGold, int playerBaseHp,
         float waveDuration)
    {
        if (!_isInitialized) return;

        FirebaseAnalytics.LogEvent("wave_complete", new Firebase.Analytics.Parameter[]
        {
            new Firebase.Analytics.Parameter("wave_number", (long)waveNumber),
            new Firebase.Analytics.Parameter("player_gold", (long)playerGold),
            new Firebase.Analytics.Parameter("player_base_hp", (long)playerBaseHp),
            new Firebase.Analytics.Parameter("wave_duration_seconds", (double)waveDuration)
        });
    }

    public void TrackUnitSpawned(string unitType, int laneIndex, int cost, int currentGold)
    {
        if (!_isInitialized) return;

        FirebaseAnalytics.LogEvent("unit_spawned", new Firebase.Analytics.Parameter[]
        {
            new Firebase.Analytics.Parameter("unit_type", unitType),
            new Firebase.Analytics.Parameter("lane_index", (long)laneIndex),
            new Firebase.Analytics.Parameter("unit_cost", (long)cost),
            new Firebase.Analytics.Parameter("gold_after_spawn", (long)currentGold)
        });
    }

    public void TrackUnitKilled(string unitType, string killedByType, int laneIndex)
    {
        if (!_isInitialized) return;

        FirebaseAnalytics.LogEvent("unit_killed", new Firebase.Analytics.Parameter[]
        {
            new Firebase.Analytics.Parameter("unit_type", unitType),
            new Firebase.Analytics.Parameter("killed_by", killedByType),
            new Firebase.Analytics.Parameter("lane_index", (long)laneIndex)
        });
    }

    public void TrackGoldEarned(int amount, string source, int waveNumber)
    {
        if (!_isInitialized) return;

        // source: "passive", "enemy_kill", "wave_bonus"
        FirebaseAnalytics.LogEvent("gold_earned", new Firebase.Analytics.Parameter[]
        {
            new Firebase.Analytics.Parameter("amount", (long)amount),
            new Firebase.Analytics.Parameter("source", source),
            new Firebase.Analytics.Parameter("wave_number", (long)waveNumber)
        });
    }

    public void TrackGoldSpent(int amount, string spentOn, int waveNumber)
    {
        if (!_isInitialized) return;
        
        // spentOn: unit type name or "upgrade_unit", "upgrade_base"
        FirebaseAnalytics.LogEvent("gold_spent", new Firebase.Analytics.Parameter[]
        {
            new Firebase.Analytics.Parameter("amount", (long)amount),
            new Firebase.Analytics.Parameter("spent_on", spentOn),
            new Firebase.Analytics.Parameter("wave_number", (long)waveNumber)
        });
    }

    public void TrackUpgradeChosen(string upgradeType, string upgradeTarget, int waveNumber)
    {
        if (!_isInitialized) return;

        // upgradeType: "unit_upgrade", "base_upgrade", "base_heal", "unlock_unit"
        FirebaseAnalytics.LogEvent("upgrade_chosen", new Firebase.Analytics.Parameter[]
        {
            new Firebase.Analytics.Parameter("upgrade_type", upgradeType),
            new Firebase.Analytics.Parameter("upgrade_target", upgradeTarget),
            new Firebase.Analytics.Parameter("wave_number", (long)waveNumber)
        });
    }

    public void TrackGameStart()
    {
        if (!_isInitialized) return;

        FirebaseAnalytics.LogEvent("game_start");
    }

    public void TrackGameOver(bool playerWon, int finalWave, float totalPlayTime,
        int totalUnitsSpawned, int totalGoldEarned)
    {
        if (!_isInitialized) return;

        FirebaseAnalytics.LogEvent("game_over", new Firebase.Analytics.Parameter[]
        {
            new Firebase.Analytics.Parameter("player_won", playerWon ? 1L : 0L),
            new Firebase.Analytics.Parameter("final_wave", (long)finalWave),
            new Firebase.Analytics.Parameter("total_play_time_seconds", (double)totalPlayTime),
            new Firebase.Analytics.Parameter("total_units_spawned", (long)totalUnitsSpawned),
            new Firebase.Analytics.Parameter("total_gold_earned", (long)totalGoldEarned)
        });
    }

    public void TrackButtonClicked(string buttonName, string screenName)
    {
        if (!_isInitialized) return;

        FirebaseAnalytics.LogEvent("button_clicked", new Parameter[]
        {
            new Parameter("button_name", buttonName),
            new Parameter("screen_name", screenName)
        });
    }
    

    public void TrackLaneSnapshot(int waveNumber, int[] unitsPerLane, int[] enemiesPerLane)
    {
        if (!_isInitialized) return;

        FirebaseAnalytics.LogEvent("lane_snapshot", new Parameter[]
        {
            new Parameter("wave_number", (long)waveNumber),
            new Parameter("lane_0_units", (long)unitsPerLane[0]),
            new Parameter("lane_1_units", (long)unitsPerLane[1]),
            new Parameter("lane_2_units", (long)unitsPerLane[2]),
            new Parameter("lane_0_enemies", (long)enemiesPerLane[0]),
            new Parameter("lane_1_enemies", (long)enemiesPerLane[1]),
            new Parameter("lane_2_enemies", (long)enemiesPerLane[2])
        });
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            TrackSessionEnd();
        }
    }

    private void OnApplicationQuit()
    {
        TrackSessionEnd();
    }
}