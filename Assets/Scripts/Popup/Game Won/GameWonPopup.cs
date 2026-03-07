using System;
using UnityEngine;
using UnityEngine.UI;

public class GameWonPopup : PopupBase
{
    [SerializeField] private Button retryButton;

    private Action _onRetry;

    private void Start()
    {
        retryButton.onClick.AddListener(OnRetryClicked);
        AnalyticsManager.Instance.TrackGameOver(true,
            WaveManager.Instance.currentWave.waveNumber,
            PlayerManager.Instance.Playtime,
            PlayerManager.Instance.TotalUnitsSpawned,
            PlayerManager.Instance.playerGold.TotalGoldEarned
        );
    }

    private void OnRetryClicked()
    {
        _onRetry?.Invoke();
        AnalyticsManager.Instance.TrackButtonClicked("retry", "win_popup");
        AnalyticsManager.Instance.TrackGameStart();
        PopupManager.Instance.Hide();
    }
    
    public GameWonPopup OnRetry(Action callback)
    {
        _onRetry = callback;
        return this;
    }
}