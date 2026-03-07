using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Card Data")] 
    public UnitData unitData;

    [Header("UI References")] 
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;

    [Header("Lane Detection")] 
    [SerializeField] private LayerMask laneLayer;

    public static Action OnCardDragStarted;
    public static Action OnCardDragEnded;

    private Vector2 originalPosition;
    private LaneDetector laneDetector;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        laneDetector = new LaneDetector(Camera.main, laneLayer, 0f);
    }

    private void Start()
    {
        UpdateCardDisplay();
    }

    public void Init(UnitData data)
    {
        unitData = data;
        UpdateCardDisplay();
    }

    private void UpdateCardDisplay()
    {
        if (unitData == null) return;

        cardImage.sprite = unitData.Icon;
        costText.text = unitData.goldCost.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        
        SetDraggingVisuals(true);
        
        OnCardDragStarted?.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SetDraggingVisuals(false);

        Lane finalLane = laneDetector.DetectLane(eventData.position);

        if (finalLane == null)
        {
            ReturnToHand();
        }
        else
        {
            PlayCard(finalLane);
        }
        
        
        OnCardDragEnded?.Invoke();
    }

    private void SetDraggingVisuals(bool isDragging)
    {
        canvasGroup.alpha = isDragging ? 0.5f : 1f;
        canvasGroup.blocksRaycasts = !isDragging;
    }

    private void ReturnToHand()
    {
        Debug.Log("Card dropped outside valid lane - returning to hand");
        rectTransform.anchoredPosition = originalPosition;
    }

    private void PlayCard(Lane lane)
    {
        EconomyManager.Instance.PayForUnit(unitData, lane.laneIndex);
    }
}