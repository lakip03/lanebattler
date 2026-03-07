using System;
using System.Collections;
using UnityEngine;

public class RangedCombat : CombatBehavior
{
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;

    [Header("Detection Settings")] [SerializeField]
    private int raycastCount = 5;

    [SerializeField] private float detectionArc = 60f;

    private const int NumberOfBufferedArraysPerRaycastHit = 10;
    private int _numberOfBufferedRays;
    private RaycastHit[] hitBuffer;
    private Coroutine spawnCoroutine;
    private bool hasAttackedThisFrame = false;

    public override void OnInitialize(GameUnit owner)
    {
        base.OnInitialize(owner);
        _numberOfBufferedRays = raycastCount * NumberOfBufferedArraysPerRaycastHit;
        hitBuffer = new RaycastHit[_numberOfBufferedRays];
        MovementBehavior = GetComponentInParent<MovementBehavior>();
    }

    public override void OnUpdate()
    {
        combatTimer += Time.deltaTime;
        TryAttack();
    }

    private void LateUpdate()
    {
        hasAttackedThisFrame = false;
    }

    private bool CanAttack()
    {
        if (GameUnit is null)
            return false;
        return combatTimer >= GameUnit.UnitData.attackSpeed;
    }

    private void TryAttack()
    {
        Transform target = FindClosestTarget();

        if (!CanAttack())
        {
            return;
        }

        if (target == null)
        {
            MovementBehavior?.Resume();
            return;
        }

        PerformAttack(target);
    }

    private Transform FindClosestTarget()
    {
        Transform closestTarget = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < raycastCount; i++)
        {
            Vector3 rayDirection = GetRayDirection(i);
            int hitCount = PerformRaycast(rayDirection);

            Transform target = GetClosestValidTargetFromHits(hitCount, out float distance);

            if (target != null && distance < closestDistance)
            {
                closestTarget = target;
                closestDistance = distance;
            }
        }

        return closestTarget;
    }

    private Vector3 GetRayDirection(int rayIndex)
    {
        Vector3 baseDirection = transform.forward;

        if (raycastCount == 1)
        {
            return baseDirection;
        }
        
        float startAngle = -detectionArc / 2f;
        float angleStep = detectionArc / (raycastCount - 1);
        float currentAngle = startAngle + (angleStep * rayIndex);

        Quaternion rotation = Quaternion.AngleAxis(currentAngle, transform.right);
        return rotation * baseDirection;
    }

    private int PerformRaycast(Vector3 direction)
    {
        Vector3 origin = firePoint.position;
        int hitCount = Physics.RaycastNonAlloc(origin, direction, hitBuffer, attackRange);

        Debug.DrawRay(origin, direction * attackRange, 
            hitCount > 0 ? Color.green : Color.red, 0.1f);

        return hitCount;
    }

    private Transform GetClosestValidTargetFromHits(int hitCount, out float distance)
    {
        Transform closestTarget = null;
        distance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            if (!IsValidTarget(hitBuffer[i], out float hitDistance)) continue;
            if (hitDistance >= distance) continue;

            closestTarget = hitBuffer[i].collider.transform;
            distance = hitDistance;
        }

        return closestTarget;
    }

    private bool IsValidTarget(RaycastHit hit, out float distance)
    {
        distance = hit.distance;

        GameBase gameBase = hit.collider.gameObject.GetComponent<GameBase>();
        if (gameBase != null)
        {
            if (!hasAttackedThisFrame)
            {
                hasAttackedThisFrame = true;
                PerformBaseAttack(hit.collider.gameObject.transform);
            }
            return false;
        }

        GameUnit attackable = hit.collider.gameObject.GetComponent<GameUnit>();

        if (attackable == null || attackable.isPlayerUnit == GameUnit.isPlayerUnit)
        {
            return false;
        }

        return true;
    }

    private void PerformBaseAttack(Transform target)
    {
        MovementBehavior?.Stop();
        spawnCoroutine = StartCoroutine(SpawnProjectileCoroutine(target));
        StopCoroutine(spawnCoroutine);
        Destroy(this.gameObject);
    }

    private void PerformAttack(Transform target)
    {
        MovementBehavior?.Stop();

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        combatTimer = 0f;
        spawnCoroutine = StartCoroutine(SpawnProjectileCoroutine(target));
    }

    private IEnumerator SpawnProjectileCoroutine(Transform target)
    {
        GameObject projectileObj = CreateProjectile();
        InitializeProjectile(projectileObj, target);

        yield return null;
    }

    private GameObject CreateProjectile()
    {
        Vector3 spawnPosition = firePoint.position;
        return Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
    }

    private void InitializeProjectile(GameObject projectileObj, Transform target)
    {
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.Initialize(target, projectileSpeed, damage, GameUnit);
        Debug.Log($"{gameObject.name} fired projectile at {target.name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        TryAttack();
    }

    private void OnDestroy()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint == null) return;

        Vector3 origin = firePoint.position;

        for (int i = 0; i < raycastCount; i++)
        {
            Vector3 direction = GetRayDirection(i);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(origin, direction * attackRange);
        }

        if (raycastCount > 1)
        {
            Gizmos.color = Color.yellow;
            Vector3 topBound = GetRayDirection(raycastCount - 1);
            Vector3 bottomBound = GetRayDirection(0);

            Gizmos.DrawRay(origin, topBound * attackRange);
            Gizmos.DrawRay(origin, bottomBound * attackRange);
        }
    }
}