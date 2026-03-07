using System;
using Script;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState gameState;
    public int currentAlivePlayers = 0;
    public int currentAliveEnemies = 0;

    public Action OnGameOver;
    public Action OnWaveStart;
    public Action OnGameWon;
    public Action OnLevelStart;

    public bool IsGamePaused = true;

    private bool _gameOverHandled = false;
    private bool _gameWonHandled = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        ChangeState(GameState.MainMenu);
        PopupManager.Instance.Show<MainMenuPopup>(t => t.OnClose(callback: InitGame));
    }

    private void InitGame()
    {
        IsGamePaused = false;
        _gameOverHandled = false;
        _gameWonHandled = false;

        InitializeManagers();

        OnLevelStart?.Invoke();
        
        ChangeState(GameState.Wave);
    }

    private static void InitializeManagers()
    {
        PlayerManager.Instance.RestartPlayerProgress();
        LaneManager.Instance.Initialize();
        WaveManager.Instance.ResetWaveManager();
        LevelManager.Instance.LoadLevel(0);
    }

    private void Update()
    {
        if (!IsGamePaused)
            HandleGameState();
    }

    private void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.MainMenu:
                HandleMainMenu();
                break;
            case GameState.Wave:
                HandleWavePhase();
                break;
            case GameState.Upgrade:
                HandleUpgradePhase();
                break;
            case GameState.GameOver:
                HandleGameOver();
                break;
            case GameState.Paused:
                HandlePausedGame();
                break;
            case GameState.GameWon:
                HandleWonGame();
                break;
        }
    }

    private void HandleMainMenu()
    {
        IsGamePaused = true;
        WaveManager.Instance.StopWave();
        EconomyManager.Instance.EndGoldGeneration();
    }

    private void HandleWonGame()
    {
        if (_gameWonHandled) return;
        _gameWonHandled = true;

        KillCurrentGame();
        OnGameWon?.Invoke();
        PopupManager.Instance.Show<GameWonPopup>(popup => popup.OnRetry(RestartGameFromBeginning));
    }

    private void HandlePausedGame()
    {
        IsGamePaused = true;
        EconomyManager.Instance.EndGoldGeneration();
    }


    private void HandleGameOver()
    {
        if (_gameOverHandled) return;
        _gameOverHandled = true;

        KillCurrentGame();
        OnGameOver?.Invoke();
        PopupManager.Instance.Show<GameOverPopup>(popup => popup.OnRetry(RestartGameFromBeginning));
    }

    private void KillCurrentGame()
    {
        IsGamePaused = true;
        WaveManager.Instance.StopWave();
        WaveManager.Instance.Genocide();
    }

    private void RestartGameFromBeginning()
    {
        HealAllBases();
        _gameOverHandled = false;
        IsGamePaused = false;
        InitGame();
    }

    private static void HealAllBases()
    {
        var bases = FindObjectsByType<GameBase>(FindObjectsSortMode.None);
        foreach (var b in bases)
        {
            Upgrader.HealBase(b);
        }
    }

    private void HandleUpgradePhase()
    {
        IsGamePaused = true;
        WaveManager.Instance.StopWave();
        EconomyManager.Instance.EndGoldGeneration();
    }

    private void HandleWavePhase()
    {
        IsGamePaused = false;
        WaveManager.Instance.StartNextWave();
        EconomyManager.Instance.StartGoldGeneration();
    }

    public void ChangeState(GameState state) => gameState = state;
    
}