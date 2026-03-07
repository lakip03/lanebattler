using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    private static PopupManager _instance;
    public static PopupManager Instance => _instance;
    
    [SerializeField] private Canvas popupCanvas;
    [SerializeField] private Transform popupContainer;
    [SerializeField] private float queueProcessDelay = 0.1f;
    
    private Dictionary<Type, PopupBase> _popupPrefabs = new();
    private Stack<PopupRequest> _popupStack = new();
    private PopupBase _currentPopup;
    private bool _isProcessingQueue;
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        RegisterPopups();
    }
    
    private void RegisterPopups()
    {
        var popups = Resources.LoadAll<PopupBase>("Popups");
        foreach (var popup in popups)
        {
            _popupPrefabs[popup.GetType()] = popup;
        }
    }
    
    public void Show<T>(Action<T> onConfigured = null) where T : PopupBase
    {
        if (!_popupPrefabs.TryGetValue(typeof(T), out var prefab))
        {
            Debug.LogError($"Popup {typeof(T).Name} not registered!");
            return;
        }
        
        var request = new PopupRequest
        {
            PopupType = typeof(T),
            Prefab = prefab,
            OnConfigured = popup => onConfigured?.Invoke(popup as T)
        };
        
        _popupStack.Push(request);
        
        if (!_isProcessingQueue)
            ProcessQueue();
    }
    
    private void ProcessQueue()
    {
        if (_currentPopup != null)
        {
            _isProcessingQueue = true;
            return;
        }
        
        if (_popupStack.Count == 0)
        {
            _isProcessingQueue = false;
            return;
        }
        
        _isProcessingQueue = true;
        var request = _popupStack.Pop();
        ShowPopup(request);
    }
    
    private void ShowPopup(PopupRequest request)
    {
        var popup = Instantiate(request.Prefab, popupContainer);
        request.OnConfigured?.Invoke(popup);
        
        _currentPopup = popup;
        popup.Show();
        
        Invoke(nameof(ProcessQueue), queueProcessDelay);
    }
    
    public void Hide()
    {
        if (_currentPopup == null) return;
        
        _currentPopup.Hide(() =>
        {
            Destroy(_currentPopup.gameObject);
            _currentPopup = null;
            ProcessQueue();
        });
    }
    
    public void ClearQueue() => _popupStack.Clear();
    
    public int GetQueueCount() => _popupStack.Count;
    
    public bool IsPopupActive() => _currentPopup != null;
}

public class PopupRequest
{
    public Type PopupType { get; set; }
    public PopupBase Prefab { get; set; }
    public Action<PopupBase> OnConfigured { get; set; }
}