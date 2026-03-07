using System;
using System.Collections.Generic;
using System.Linq;
using Script;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] public List<Level> levels;
    [SerializeField] private int currentLevelIndex = 0;

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

    private void Start()
    {
        RegisterLevels();
    }

    private void RegisterLevels()
    {
        var levelList = Resources.LoadAll<Level>("Levels");
        foreach (var level in levelList)
        {
            levels.Add(level);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        Level level = levels[levelIndex];
        WaveManager.Instance.LoadFirstWaveOfLevel(level);
        
        Debug.Log($"Loaded Level {level.levelNumber}");
    }

    public int GetLastWave() => GetCurrentLevel()?.waves.Max(wave => wave.waveNumber) ?? 0;

    private Level GetCurrentLevel()
    {
        if (currentLevelIndex < 0 || currentLevelIndex >= levels.Count)
            return null;
        
        return levels[currentLevelIndex];
    }
}