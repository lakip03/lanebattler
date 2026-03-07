using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPopup : PopupBase
{
    [SerializeField] Button playButton;

    private Action _onClosed;

    private void Start()
    {
        playButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        AnalyticsManager.Instance.TrackButtonClicked("game_start", "main_menu_popup");
      ClosePopup();
    }
    
    public MainMenuPopup OnClose(Action callback)
    {
        _onClosed = callback;
        return this;
    }
    
    private void ClosePopup()
    {
        _onClosed?.Invoke();
        AnalyticsManager.Instance.TrackGameStart();
        PopupManager.Instance.Hide();
    }
}