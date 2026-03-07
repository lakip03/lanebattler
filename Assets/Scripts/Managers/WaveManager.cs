using System;
using System.Collections;
using Script;
using UnityEngine;
using UnityEngine.Serialization;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    public Level currentLevel;
    public Wave currentWave;

    private bool _isWaveActive = false;
    [SerializeField] private int currentWaveNumber = 0;

    [FormerlySerializedAs("_waveTimer")] [SerializeField]
    private float waveTimer = 0;

    public Option<Action> OnWaveComplete;

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

    private void Update()
    {
        waveTimer += Time.deltaTime;
    }

    public void LoadFirstWaveOfLevel(Level level)
    {
        currentLevel = level;
        currentWaveNumber = 0;
        StartWave(0);
        Debug.Log($"Loaded Level: {level.levelNumber} with {level.GetTotalNumberOfWaves()} waves");
    }

    private void StartWave(int number)
    {
        if (_isWaveActive || currentLevel is null)
            return;
        
        AnalyticsManager.Instance.TrackWaveStart(number,
            PlayerManager.Instance.playerGold.CurrentBalance,
            (int)PlayerManager.Instance.GetPlayerBase().currentHP);
        currentWave = currentLevel.GetWaveByWaveNumber(number);
        SetupWave(number);

        Debug.Log($"Starting Wave {number} of Level {currentLevel.levelNumber}");

        StartCoroutine(SpawnWave());
    }

    public void StartNextWave()
    {
        if (_isWaveActive || currentLevel is null)
            return;

        if (currentWaveNumber >= LevelManager.Instance.GetLastWave())
        {
            StartWave(LevelManager.Instance.GetLastWave());
            return;
        }

        var nextWave = currentWaveNumber + 1;
        StartWave(nextWave);
    }

    public void StopWave()
    {
        _isWaveActive = false;
    }

    IEnumerator SpawnWave()
    {
        if (currentWave.spawnGroups == null || currentWave.spawnGroups.Count == 0)
        {
            Debug.LogWarning("No spawn groups in this wave!");
            yield break;
        }

        foreach (EnemySpawnGroup group in currentWave.spawnGroups)
        {
            StartCoroutine(SpawnGroupWithDelay(group));
        }
    }

    IEnumerator SpawnGroupWithDelay(EnemySpawnGroup group)
    {
        while (waveTimer < group.groupStartTimeInSeconds)
        {
            yield return null;
        }

        yield return StartCoroutine(SpawnGroup(group));
    }

    IEnumerator SpawnGroup(EnemySpawnGroup group)
    {
        for (int i = 0; i < group.count; i++)
        {
            SpawnTheEnemy(group.enemy, group.laneIndex);

            if (i < group.count - 1) // Don't wait after last spawn
            {
                yield return new WaitForSeconds(group.spawnDelay);
            }
        }
    }

    public void Genocide()
    {
        Debug.Log("Genocide started");

        StopAllCoroutines();

        var everyone = FindObjectsByType<GameUnit>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var e in everyone)
        {
            e.ForceDestroy();
        }

        GameManager.Instance.currentAliveEnemies = 0;
        GameManager.Instance.currentAlivePlayers = 0;
    }

    public void ResetWaveManager()
    {
        _isWaveActive = false;
        currentWaveNumber = 0;
        waveTimer = 0;
        currentWave = null;
        StopAllCoroutines();
    }

    private void SpawnTheEnemy(UnitData groupEnemy, int groupLaneIndex)
    {
        GameUnit enemy = Spawner.Instance.SpawnEnemy(groupEnemy, groupLaneIndex);
        enemy.Initialize();
    }

    private void SetupWave(int number)
    {
        currentWaveNumber = number;
        _isWaveActive = true;
        waveTimer = 0;
    }

    public void TryCompleteWave()
    {
        if (IsWaveCompleted())
        {
            AnalyticsManager.Instance.TrackWaveComplete(
                currentWaveNumber,
                PlayerManager.Instance.playerGold.CurrentBalance,
                (int)PlayerManager.Instance.GetPlayerBase().currentHP,
                waveTimer);
            GameManager.Instance.ChangeState(GameState.Upgrade);
            PopupManager.Instance.Show<UpgradePopup>(popup =>
            {
                popup.OnClose((() =>
                {
                    StartNextWave();
                    GameManager.Instance.IsGamePaused = false;
                    GameManager.Instance.ChangeState(GameState.Wave);
                }));
            });
        }
    }

    private bool IsWaveCompleted()
    {
        if (!_isWaveActive)
            return true;

        bool allEnemiesDead = GameManager.Instance.currentAliveEnemies <= 0;
        bool allPlayerDead = GameManager.Instance.currentAlivePlayers <= 0;

        return allEnemiesDead && allPlayerDead;
    }
}