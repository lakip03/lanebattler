using System;

public static class Upgrader
{
    public static Action NewUnitUnlocked;
    public static void UpgradeBase(GameBase gameBase)
    {
        ++gameBase.currentHP;
        ++gameBase.maxHP;
        ++gameBase.baseLevel;
    }

    public static void HealBase(GameBase gameBase)
    {
        gameBase.currentHP = gameBase.maxHP;
    }

    public static UnitData UnlockNewUnit()
    {
        UnitData unit = GameUtils.UnlockRandomNewUnit(PlayerManager.Instance.playerInventory);
        NewUnitUnlocked?.Invoke();
        return unit;
    }
}