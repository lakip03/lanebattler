using UnityEngine;

[System.Serializable]
public class PlayerGold
{
    [field: SerializeField]public int CurrentBalance {get; private set;}
    public int TotalGoldEarned;
    
    public void Initialize(int startingGold)
    {
        CurrentBalance = startingGold;
        EconomyManager.Instance.OnGoldChangedEvent?.Invoke();
    }
    
    public bool CanAfford(int amount) => amount < CurrentBalance;
    
    public void SpendGold(int amount, string reason)
    {
        AnalyticsManager.Instance.TrackGoldSpent(amount,
            reason,
            WaveManager.Instance.currentWave.waveNumber);
        CurrentBalance -= amount;
        EconomyManager.Instance.OnGoldChangedEvent?.Invoke();
    }

    public void AddGold(int amount)
    {
        CurrentBalance += amount;
        ++TotalGoldEarned;
        EconomyManager.Instance.OnGoldChangedEvent?.Invoke();
    }

    public void SetGold(int amount)
    {
        CurrentBalance = amount;
        EconomyManager.Instance.OnGoldChangedEvent?.Invoke();
    }
}