using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPopup : PopupBase
{
    [SerializeField] private Button retryButton;

    private Action _onRetry;
    private Action _onMainMenu;

    private void Start()
    {
        retryButton.onClick.AddListener(OnRetryClicked);
    }

    private void OnRetryClicked()
    {
        _onRetry?.Invoke();
        AnalyticsManager.Instance.TrackButtonClicked("retry_button", "game_over");
        AnalyticsManager.Instance.TrackGameOver(false,
            WaveManager.Instance.currentWave.waveNumber,
            PlayerManager.Instance.Playtime,
            PlayerManager.Instance.TotalUnitsSpawned,
            PlayerManager.Instance.playerGold.TotalGoldEarned
            );
        PopupManager.Instance.Hide();
    }
    
    public GameOverPopup OnRetry(Action callback)
    {
        _onRetry = callback;
        return this;
    }
}