using System;
using UnityEngine;
using Random = System.Random;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Passive Gold Generation")] [SerializeField]
    private int goldPerSecond = 2;

    [SerializeField] private float goldAccumulator = 0;

    [SerializeField] private bool isGeneratingGold = false;

    [Header("Kill Rewards")] [SerializeField]
    private int minKillGold = 5;

    [SerializeField] private int maxKillGold = 10;

    public Action OnGoldChangedEvent;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        GameManager.Instance.OnWaveStart += OnWaveStarted;
        GameManager.Instance.OnGameOver += OnGameOver;
    }

    private void Update()
    {
        if (isGeneratingGold)
        {
            GeneratePassiveGold();
        }
    }

    private void GeneratePassiveGold()
    {
        goldAccumulator += goldPerSecond * Time.deltaTime;

        if (goldAccumulator >= goldPerSecond)
        {
            int goldToAward = Mathf.FloorToInt(goldAccumulator);
            PlayerManager.Instance.playerGold.AddGold(goldToAward);
            goldAccumulator -= goldToAward;
        }
    }

    public void PayForUnit(UnitData unitData, int laneIndex)
    {
        var unitCost = unitData.goldCost;
        if (!PlayerManager.Instance.playerGold.CanAfford(unitCost))
            return;
        
        PlayerManager.Instance.playerGold.SpendGold(unitCost, "on_unit_spent");
        GameUnit unit = Spawner.Instance.SpawnPlayerUnit(unitData, laneIndex);
        unit.Initialize();
    }

    public void StartGoldGeneration() => isGeneratingGold = true;
    public void EndGoldGeneration() => isGeneratingGold = false;

    public void OnEnemyKilled()
    {

        Random random = new Random();
        int goldReward = random.Next(minKillGold, maxKillGold);
        AnalyticsManager.Instance.TrackGoldEarned(
            goldReward,
            "killed_enemy",
            WaveManager.Instance.currentWave.waveNumber
        );
        PlayerManager.Instance.playerGold.AddGold(goldReward);
        Debug.Log(goldReward + "GOLD given to player for killing");
    }
    

    private void OnWaveStarted()
    {
        StartGoldGeneration();
    }

    private void OnGameOver()
    {
        EndGoldGeneration();
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnWaveStart -= OnWaveStarted;
        GameManager.Instance.OnGameOver -= OnGameOver;
    }
}