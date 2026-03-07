using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePopup : PopupBase
{
    [Header("Container References")]
    [SerializeField] private Transform canvasPlayerInventory;
    [SerializeField] private Transform playerDeckContainer;
    
    [Header("Prefab References")]
    [SerializeField] private GameObject inventoryCardPrefab; 
    [SerializeField] private GameObject deckSlotPrefab; 

    [Header("Button References")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button unlockButton;
    [SerializeField] private Button claimButton;
    [SerializeField] private Button healButton;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI playerBaseLevel;

    private const int MAX_DECK_SIZE = 5;

    private int _rewardAmount;
    private Action _onClosed;
    private GameBase PlayerBase => PlayerManager.Instance.GetPlayerBase();

    protected override void Awake()
    {
        base.Awake();
        RegisterButtonListeners();
        ShowPlayerInventory();
        SetupPopupVisuals();
    }

    private void RegisterButtonListeners()
    {
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        unlockButton.onClick.AddListener(OnUnlockClicked);
        claimButton.onClick.AddListener(OnClaimClicked);
        healButton.onClick.AddListener(OnHealedClicked);
    }

    private void OnHealedClicked()
    {
        Upgrader.HealBase(PlayerBase);
        AnalyticsManager.Instance.TrackUpgradeChosen(
            "heal_base",
            $"",
            WaveManager.Instance.currentWave.waveNumber
        );
        LockUpgrades();
    }

    private void SetupPopupVisuals()
    {
        playerBaseLevel.text = PlayerBase.baseLevel.ToString();
    }

    private void ShowPlayerInventory()
    {
        foreach (var unitData in PlayerManager.Instance.playerInventory.unlockedUnits)
        {
            GameObject cardObject = Instantiate(inventoryCardPrefab, canvasPlayerInventory, false);
            InventoryCardUI inventoryCard = cardObject.GetComponent<InventoryCardUI>();
            
            if (inventoryCard != null)
            {
                inventoryCard.Init(unitData);
            }
        }
    }
    
    private void RestartPlayerInventory()
    {
        foreach (Transform child in canvasPlayerInventory)
        {
            Destroy(child.gameObject);
        }
        
        ShowPlayerInventory();
    }

    public UpgradePopup OnClose(Action callback)
    {
        _onClosed = callback;
        return this;
    }

    private void OnUpgradeClicked()
    {
        AnalyticsManager.Instance.TrackButtonClicked("upgrade_base", "upgrade_popup");
        Upgrader.UpgradeBase(PlayerBase);
        AnalyticsManager.Instance.TrackUpgradeChosen(
            "base_upgrade",
            $"player_base_to_level_{PlayerBase.baseLevel.ToString()}",
            WaveManager.Instance.currentWave.waveNumber
        );
        LockUpgrades();
        SetupPopupVisuals();
    }

    private void OnUnlockClicked()
    {
        AnalyticsManager.Instance.TrackButtonClicked("upgrade_unlocked", "upgrade_popup");
        UnitData unlocked = Upgrader.UnlockNewUnit();
        AnalyticsManager.Instance.TrackUpgradeChosen(
            "unlock_new_card",
            $"unlocked_{unlocked.unitType}",
            WaveManager.Instance.currentWave.waveNumber
        );
        UpdateCurrentPlayerDeck();
        LockUpgrades();
        RestartPlayerInventory();
    }

    private void LockUpgrades()
    {
        unlockButton.interactable = false;
        upgradeButton.interactable = false;
        healButton.interactable = false;
    }

    private void OnClaimClicked()
    {
        AnalyticsManager.Instance.TrackButtonClicked("claimed_upgrade", "upgrade_popup");
        ClosePopup();
    }

    private void ClosePopup()
    {
        _onClosed?.Invoke();
        PopupManager.Instance.Hide();
    }

    private static void UpdateCurrentPlayerDeck()
    {
        PlayerManager.Instance.playerDeck.currentDeck = new List<UnitData>();
        foreach (var unit in PlayerManager.Instance.playerInventory.unlockedUnits)
        {
            PlayerManager.Instance.playerDeck.AddUnitToDeck(unit);
        }
    }
}