using UnityEngine;

public class MeleeCombat : CombatBehavior
{
    private void Start()
    {
        Debug.Log("Melee Combat Initialized");
        MovementBehavior = GetComponentInParent<MovementBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Entered {other.gameObject.name}");
        MovementBehavior?.Stop();
        
        if (other.GetComponent<IAttackable>() != null)
        {
            HandleAttackable(other);
        }
    }

    private void HandleAttackable(Collider attackable)
    {
        var unitComponent = attackable.GetComponent<GameUnit>();
        if (unitComponent != null)
        {
            HandleUnitCombat(unitComponent); 
        }
        else
        {
            HandleBaseCombat(attackable);
        }
    }

    private void HandleBaseCombat(Collider attackable)
    {
        Debug.Log($"Attacking Base");
        var gameBase = attackable.GetComponent<GameBase>();
        
        OnAttackOnce(gameBase);
        Destroy(gameObject);
    }

    private void HandleUnitCombat(GameUnit gameUnitComponent)
    {
        if (gameUnitComponent.isPlayerUnit == this.GameUnit.isPlayerUnit)
        {
            Debug.Log($"Frednly Fired");
            return;
        }
        OnAttack(gameUnitComponent);
    }

    protected override void PerformAttack()
    {
        if ((UnityEngine.Object)Target != null)
        {
            Debug.Log($"{GameUnit.name} melees {Target}");
            Target.TakeDamage(CalculateDamage(Target));
        }
    }
}