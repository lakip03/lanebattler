using System;
using Unity.Mathematics.Geometry;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class CombatBehavior : MonoBehaviour
{
    [SerializeField] protected GameUnit GameUnit;
    [SerializeField] protected IAttackable Target;
    [SerializeField] public float combatTimer = 0f;
    [SerializeField] protected bool isInCombat = false;
    [SerializeField] public float varianceCoefficient = .2f;
    [SerializeField] protected MovementBehavior MovementBehavior;

    private void Awake()
    {
        MovementBehavior = GetComponent<MovementBehavior>();
    }

    public virtual void OnInitialize(GameUnit owner)
    {
        GameUnit = owner;
    }

    public virtual void OnAttack(IAttackable target)
    {
        Target = target;
        isInCombat = true;
        var variance = Random.Range(1 - varianceCoefficient, 1 + varianceCoefficient);
        combatTimer = GameUnit.UnitData.attackSpeed * variance;
    }

    protected virtual void OnAttackOnce(IAttackable target)
    {
        target.TakeDamage(GameUnit.UnitData.damage);
    }

    private void UpdateCombat()
    {
        if (!isInCombat)
        {
            MovementBehavior.Resume();
            return;
        }

        if ((UnityEngine.Object)Target == null)
        {
            StopCombat();
        }


        combatTimer += Time.deltaTime;

        TryAttack();
    }

    private void TryAttack()
    {
        if (combatTimer >= GameUnit.UnitData.attackSpeed)
        {
            PerformAttack();
            combatTimer = 0f;
        }
    }

    protected int CalculateDamage(IAttackable target)
    {
        if (target is GameUnit targetUnit)
        {
            return CombatResolver.ResolveCombat(targetUnit, GameUnit);
        }

        return GameUnit.UnitData.damage;
    }

    protected virtual void PerformAttack()
    {
       
    }

    public virtual void OnUpdate()
    {
        UpdateCombat();
    }

    public void StopCombat()
    {
        isInCombat = false;
        MovementBehavior.Resume();
    }
}