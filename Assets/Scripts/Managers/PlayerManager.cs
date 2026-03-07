using System;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public UnitData startingUnit;
    public int startingGold = 400;
    [SerializeField] private GameBase playerBase;
    
    public float Playtime { get; private set; }
    public int TotalUnitsSpawned;
    
    public PlayerDeck playerDeck = new PlayerDeck();
    public PlayerInventory playerInventory = new PlayerInventory();
    public PlayerGold playerGold = new PlayerGold();
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
        Initialize();
    }

    private void Update()
    {
        Playtime += Time.deltaTime;
    }

    private void Initialize()
    {
        playerDeck.Initialize(startingUnit);
        playerInventory.Initialize(startingUnit);
        playerGold.Initialize(startingGold);
    }


    public void RestartPlayerProgress()
    {
        playerDeck = new PlayerDeck();
        playerInventory = new PlayerInventory();
        playerGold = new PlayerGold();
        
        Initialize();
    }
    
    public GameBase GetPlayerBase() => playerBase;}