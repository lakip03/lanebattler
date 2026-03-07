using System.Collections.Generic;
using Unity.VisualScripting;

[System.Serializable]
public class PlayerInventory
{
    public List<UnitData> unlockedUnits;

    public void Initialize(UnitData startingUnit)
    {
        unlockedUnits = new List<UnitData>();
        UnlockUnit(startingUnit);
    }
    
    public void UnlockUnit(UnitData unit) {
        if(IsUnlocked(unit)) return;
        unlockedUnits.Add(unit);
    }
    
    public bool IsUnlocked(UnitData unit) => unlockedUnits.Contains(unit);
}