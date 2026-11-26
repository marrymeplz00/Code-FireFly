using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Golem : MonoBehaviour
{
    [Header("Golem")]
    public int maxHealth = 500;
    private int currentHealth;

    [Header("Reward")]
    public int upgradePointReward = 2;

    [Header("Attack Sound")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField, Range(0f, 1f)] private float attackVolume = 0.8f;

    private AudioSource audioSource;
    private bool isDead = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // แยกเสียงของโกเลมออกจากเสียงตัวละครผู้เล่น
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
        audioSource.mute = false;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Attack Sound
    // เรียกใช้ตอนโกเลมออกท่าโจมตี
    public void PlayAttackSound()
    {
        if (isDead)
            return;

        if (attackSound == null)
        {
            Debug.LogWarning("Golem: ยังไม่ได้กำหนด Attack Sound ใน Inspector", gameObject);
            return;
        }

        audioSource.PlayOneShot(attackSound, attackVolume);
    }

    // พลังชีวิต
    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

   
    // การตาย
    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        GiveUpgradePoints();

        Destroy(gameObject);
    }

   
    // ให้ Point เพื่อเอาไปใช้ Upgrade 
    private void GiveUpgradePoints()
    {
        SkillUpgradeManager manager = FindFirstObjectByType<SkillUpgradeManager>();

        if (manager != null)
        {
            manager.AddUpgradePoints(upgradePointReward);
        }
    }
}