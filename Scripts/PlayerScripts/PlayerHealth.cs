
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Image healthBar;

    [Header("Game Over")]
    public string gameOverSceneName = "GameLose";

    private bool isDead = false;
    private PlayerControl playerControl;

    void Start()
    {
        currentHealth = maxHealth;

        playerControl = GetComponent<PlayerControl>();

        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (playerControl != null)
        {
            // เล่นแอนิเมชันตายก่อน
            playerControl.PlayDeathAnimation();
        }
        else
        {
            // หากไม่มี PlayerControl ให้โหลดซีนทันที
            LoadGameOver();
        }
    }

    // เรียกหลังแอนิเมชันตายเล่นจบ
    public void LoadGameOver()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }

    public void Heal(int amount)
    {
        if (isDead)
            return;

        currentHealth += amount;

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();
    }
}
