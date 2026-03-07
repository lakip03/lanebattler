using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCardUI : MonoBehaviour
{
    [Header("Card Data")] 
    public UnitData unitData;

    [Header("UI References")] 
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;

    private Transform _originalParent;
    private int _originalSiblingIndex;
    private Vector3 _originalPosition;


    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Init(UnitData data, bool isFromInventory = true)
    {
        unitData = data;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (unitData == null) return;

        if (cardImage != null && unitData.Icon != null)
        {
            cardImage.sprite = unitData.Icon;
        }
    }
    
    
}