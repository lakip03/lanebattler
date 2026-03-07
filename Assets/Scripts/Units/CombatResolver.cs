using UnityEngine;

public static class CombatResolver
{
    public static int ResolveCombat(GameUnit damagedUnit, GameUnit attackingUnit)
    {
        int baseDamage = attackingUnit.UnitData.damage;
        float damageMultiplier = 1f;

        var isStronger = attackingUnit.UnitData.strongAgainst != null &&
                         attackingUnit.UnitData.strongAgainst.Contains(damagedUnit.UnitData.unitType);
        if (isStronger)
        {
            damageMultiplier = attackingUnit.UnitData.damageMultiplierStrong;
        }

        int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);

        return finalDamage;
    }
}