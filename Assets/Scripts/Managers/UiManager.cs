using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public UiManager Instance {get; private set;}
    
    [SerializeField] private GameObject cardSelectionSpace;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject lockedCardPrefab;
    [SerializeField] private TextMeshProUGUI currentGoldText;
    
    [SerializeField] private EconomyManager economyManager;
    [SerializeField] private PlayerManager playerManager;

    
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
        SubscribeToEvents();
    }
    
    private void SubscribeToEvents()
    {
        economyManager.OnGoldChangedEvent += ChangeGoldTextUi;
        CardManager.OnDeckInitialized += InitializeDeck;
        CardManager.OnCardAddedEvent += InitializeDeck;
        CardManager.OnCardReplacedEvent += InitializeDeck;
    }

    private void InitializeDeck()
    {
        ClearDeck();
        FillWithCurrentPlayerDeck();
        FillRemainingWithLockedCards();
    }

    private void FillRemainingWithLockedCards()
    {
        int currentCardCount = PlayerManager.Instance.playerDeck.currentDeck.Count;
        int maxDeckSize = PlayerManager.Instance.playerDeck.maxDeckSize;
        int lockedCardsNeeded = maxDeckSize - currentCardCount;

        for (int i = 0; i < lockedCardsNeeded; i++)
        {
            Instantiate(lockedCardPrefab, cardSelectionSpace.transform, false);
        }
    }

    private void FillWithCurrentPlayerDeck()
    {
        foreach (var playerCardData in PlayerManager.Instance.playerDeck.currentDeck)
        {
            Debug.Log("PlayerCardData: " + playerCardData.name);
            var cardObject = Instantiate(cardPrefab, cardSelectionSpace.transform, false).GetComponent<Card>();
            cardObject.Init(playerCardData);
        }
    }

    private void ClearDeck()
    {
        foreach (Transform child in cardSelectionSpace.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ChangeGoldTextUi()
    {
        currentGoldText.text = playerManager.playerGold.CurrentBalance.ToString();
        
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
        economyManager.OnGoldChangedEvent -= ChangeGoldTextUi;
        CardManager.OnDeckInitialized -= InitializeDeck;
        CardManager.OnCardAddedEvent -= InitializeDeck;
        CardManager.OnCardReplacedEvent -= InitializeDeck;
    }
}