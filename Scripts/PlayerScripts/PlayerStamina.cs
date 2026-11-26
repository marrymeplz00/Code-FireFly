using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;

    [Header("Stamina Regeneration")]
    public float staminaRegenRate = 4f;

    [Header("Stamina Bar")]
    public Image staminaFill;

    void Start()
    {
        currentStamina = maxStamina;

        UpdateStaminaBar();
    }

    void Update()
    {
        RegenerateStamina();
        UpdateStaminaBar();
    }

    void RegenerateStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;

            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina < amount)
            return false;

        currentStamina -= amount;

        UpdateStaminaBar();

        return true;
    }

    public bool HasEnoughStamina(float amount)
    {
        return currentStamina >= amount;
    }

    void UpdateStaminaBar()
    {
        if (staminaFill != null)
        {
            staminaFill.fillAmount = currentStamina / maxStamina;
        }
    }
}