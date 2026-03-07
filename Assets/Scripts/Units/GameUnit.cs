using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GameUnit : MonoBehaviour, IAttackable
{
    private UnitData unitData;
    [SerializeField] private UnitType unitType;

    [Header("Runtime Stats")] [SerializeField]
    private float currentHP;

    [SerializeField] private bool isAlive = true;

    private IAttackable _currentTarget;
    private float _attackCooldown;
    private bool _isBeingDestroyed = false;

    [Header("Team")] public bool isPlayerUnit = true;
    public bool isUnitGettingAttacked = false;
    public int laneIndex = 0;

    [SerializeField] private MovementBehavior movementBehavior;
    [SerializeField] private CombatBehavior combatBehavior;
    //public AbilityBehavior abilityBehavior;

    [Header("UI")] [SerializeField] private GameObject healthBarPrefab;
    private GameObject healthBarObj;
    private HealthBarUI healthBarInstance;

    public bool IsAlive() => currentHP > 0;
    public GameObject GetHealthBarObject() => healthBarInstance.gameObject;

    public UnitData UnitData => unitData;

    public void SetUnitData(UnitData unitData) => this.unitData = unitData;
    
    public void Initialize()
    {
        if (unitData == null)
        {
            Debug.LogError("UnitData is null on " + gameObject.name);
            return;
        }

        currentHP = unitData.maxHp;
        isAlive = true;
        _attackCooldown = 0f;

        movementBehavior.OnInitialize(this);
        combatBehavior.OnInitialize(this);

        CreateHealthBar();
    }

    private void CreateHealthBar()
    {
        healthBarObj = Instantiate(healthBarPrefab, this.transform,false);
        healthBarObj.transform.localPosition = new Vector3(0, 1f, 0);
        healthBarInstance = healthBarObj.GetComponent<HealthBarUI>();
        healthBarInstance.Initialize(transform, unitData.maxHp);
    }


    private void Update()
    {
        if (!isAlive) return;
        

        if (_attackCooldown > 0)
        {
            _attackCooldown -= Time.deltaTime;
        }

        movementBehavior.OnUpdate();
        combatBehavior.OnUpdate();
    }

    public void TakeDamage(float damage)
    {
        
        isUnitGettingAttacked = true;
        if (!isAlive) return;

        currentHP -= damage;

        healthBarInstance.UpdateHealth(currentHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (IsAlive()) return;
        
        AnalyticsManager.Instance.TrackUnitKilled(UnitData.unitType.ToString(), 
            null,
            laneIndex);
        
        Destroy(gameObject);
    }

    private void KilledUnitHandle()
    {
        if (!isPlayerUnit)
        {
            --GameManager.Instance.currentAliveEnemies;
            Debug.unityLogger.Log("Killed enemy unit" + unitData.name);
            EconomyManager.Instance.OnEnemyKilled();
        }
        else
        {
            --GameManager.Instance.currentAlivePlayers;
        }
        WaveManager.Instance.TryCompleteWave();
    }

    public void ForceDestroy()
    {
        _isBeingDestroyed = true;
        Destroy(gameObject);
        
    }
    
    private void OnDestroy()
    {
        if (!_isBeingDestroyed)
        {
            KilledUnitHandle();
        }
        
        if (healthBarInstance != null)
        {
            Destroy(healthBarObj);
        }
    }
}