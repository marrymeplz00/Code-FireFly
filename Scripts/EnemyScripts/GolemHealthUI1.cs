using UnityEngine;
using UnityEngine.UI;

public class GolemHealthUI : MonoBehaviour
{
    [Header("Golem Health UI")]
    public GameObject golemHealthUI;
    public Image healthBar;

    [Header("Player Detection")]
    public Transform player;
    public float showDistance = 12f;

    private Enemy enemy;
    private bool isDead = false;

    private void Start()
    {
        // หา Enemy ที่อยู่บน Golem ตัวนี้
        enemy = GetComponent<Enemy>();

        // ซ่อน UI ตอนเริ่มเกม
        if (golemHealthUI != null)
        {
            golemHealthUI.SetActive(false);
        }

        // ตั้งหลอดเลือดให้เต็ม
        UpdateHealthBar();
    }

    private void Update()
    {
        if (isDead)
            return;

        if (player == null || golemHealthUI == null || enemy == null)
            return;

        // คำนวณระยะ Player กับ Golem
        float distance = Vector2.Distance(transform.position, player.position);

        // แสดง/ซ่อน UI
        if (distance <= showDistance)
        {
            golemHealthUI.SetActive(true);
        }
        else
        {
            golemHealthUI.SetActive(false);
        }

        // อัปเดตหลอดเลือด
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBar == null || enemy == null)
            return;

        healthBar.fillAmount = (float)enemy.currentHealth / enemy.maxHealth;
    }

    public void HideUI()
    {
        isDead = true;

        if (golemHealthUI != null)
        {
            golemHealthUI.SetActive(false);
        }
    }
}