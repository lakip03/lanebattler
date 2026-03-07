using UnityEngine;
using System.Linq;

public static class GameUtils
{
    public static UnitDatabase UnitDatabase => Resources.Load<UnitDatabase>("Unit Database");
    
    public static GameObject GetPrefabByUnitData(UnitData unitData) => UnitDatabase.GetPrefab(unitData);

    public static GameObject GetPrefabByUnitType(UnitType unitType) => UnitDatabase.GetPrefab(unitType);

    public static UnitData UnlockRandomNewUnit(PlayerInventory inventory)
    {
        var lockedUnits = UnitDatabase.units
            .Where(entry => !inventory.IsUnlocked(entry.unitData))
            .Select(entry => entry.unitData)
            .ToArray();

        if (lockedUnits.Length == 0)
        {
            Debug.LogWarning("No locked units available to unlock!");
            return null;
        }

        var randomUnit = lockedUnits[Random.Range(0, lockedUnits.Length)];
        inventory.UnlockUnit(randomUnit);
        
        Debug.Log($"Unlocked new unit: {randomUnit.unitType}");
        return randomUnit;
    }
}