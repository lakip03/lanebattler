using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit Database", menuName = "Battler/Unit Database")]
public class UnitDatabase : ScriptableObject
{
    [System.Serializable]
    public class UnitEntry
    {
        public UnitData unitData;
        public GameObject prefab;
    }
    
    public List<UnitEntry> units = new List<UnitEntry>();
    
    public GameObject GetPrefab(UnitData unitData)
    {
        return units.Find(entry => entry.unitData == unitData)?.prefab;
    }
    
    public GameObject GetPrefab(UnitType unitType)
    {
        return units.Find(entry => entry.unitData.unitType == unitType)?.prefab;
    }
}