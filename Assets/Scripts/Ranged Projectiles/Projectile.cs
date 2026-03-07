using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float speed;
    private GameUnit owner;
    private bool isInitialized = false;

    [Header("Projectile Settings")] public float maxLifetime = 5f;

    private float lifetimeTimer = 0f;


    public void Initialize(Transform targetTransform, float projectileSpeed, int projectileDamage,
        GameUnit ownerGameUnit)
    {
        target = targetTransform;
        speed = projectileSpeed;
        owner = ownerGameUnit;
        isInitialized = true;
        
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        lifetimeTimer += Time.deltaTime;

        if (lifetimeTimer >= maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        MoveTowardsTarget();
    }
    
    bool hasHit = false;
    private void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * (speed * Time.deltaTime);

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(transform.position, target.position) < 0.5f && !hasHit)
        {
            hasHit = true;
            HitTarget();
        }
    }
    

    private void HitTarget()
    {
        var attackable = target.GetComponent<IAttackable>();
        attackable.TakeDamage(CalculateDamage(attackable));
        Destroy(this.gameObject);
    }

    private int CalculateDamage(IAttackable target)
    {
        if (target is GameUnit targetUnit)
        {
            return CombatResolver.ResolveCombat(targetUnit, owner);
        }

        return owner.UnitData.damage;
    }
}