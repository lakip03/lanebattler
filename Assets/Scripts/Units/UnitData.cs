using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Battler/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("Basic Info")] 
    public UnitType unitType;
    public Sprite Icon;

    [Header("Stats")] 
    public int goldCost = 50;
    public int maxHp = 100;
    public int damage = 25;
    public float moveSpeed = 2f;
    public float attackSpeed = 1f;
    
    
    [Header("Combat Relationships")]
    public List<UnitType> strongAgainst;
    public float damageMultiplierStrong = 2f;
}