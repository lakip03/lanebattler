using UnityEngine;
using UnityEngine.UI;


public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    

    private void Awake()
    {
        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>();
    }

    public void Initialize(Transform unit, float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void UpdateHealth(float currentHealth)
    {
        healthSlider.value = currentHealth;
    }
    
}