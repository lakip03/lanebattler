using System;
using Script;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameBase : MonoBehaviour, IAttackable
{
    public bool isPlayer;
    public float currentHP;
    public float maxHP = 10;
    public int baseLevel = 1;
    
    [Header("UI References")]
    [SerializeField]private TextMeshProUGUI hpText;
    [SerializeField]private Slider hpSlider;

    public void Start()
    {
        currentHP = maxHP;
        SetupUI();
    }

    private void Update()
    {
        if (currentHP <= 0)
        {
            Die();
        }

        SetupUI();
    }

    public void SetupUI()
    {
        hpText.text = "HP: " + currentHP + "/" + maxHP;
        hpSlider.value = Mathf.Clamp01(currentHP / maxHP);
    }

    public void TakeDamage(float damage)
    {

        damage = 1;
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
            
    }

    public void Die()
    {
        if (isPlayer)
        {
            GameManager.Instance.ChangeState(GameState.GameOver);
            return;
        }
        GameManager.Instance.ChangeState(GameState.GameWon);
        
    }
}