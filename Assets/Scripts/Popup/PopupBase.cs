using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopupBase : MonoBehaviour
{
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected Button closeButton;

    protected virtual void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(() =>
            {
                AnalyticsManager.Instance.TrackButtonClicked("closed_popup", "popup");
                PopupManager.Instance.Hide();
            });
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        OnShow();
    }

    public virtual void Hide(Action onComplete = null)
    {
        OnHide();
        onComplete?.Invoke();
    }

    protected virtual void OnShow()
    {
    }

    protected virtual void OnHide()
    {
    }
}