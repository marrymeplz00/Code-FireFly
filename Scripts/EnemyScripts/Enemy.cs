using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Attack")]
    public int damage = 10;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float detectRange = 6f;

    [Header("Drop")]
    public GameObject dropItemPrefab;

    [Header("Upgrade Point")]
    public int upgradePointReward = 2;

    [Header("Animation")]
    public Animator animator;

    [Header("Shockwave")]
    public GameObject shockwavePrefab;
    public float shockwaveSpawnDistance = 1f;

    private Transform player;
    private float lastAttackTime;

    private bool isAttacking = false;
    private bool isDead = false;

    private GolemHealthUI golemHealthUI;
    private SkillUpgradeManager upgradeManager;

    private void Start()
    {
        currentHealth = maxHealth;

        // หา Player จาก Tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        // หา Animator อัตโนมัติ
        if (animator == null)
            animator = GetComponent<Animator>();

        // หา GolemHealthUI
        golemHealthUI = GetComponent<GolemHealthUI>();

        // หา SkillUpgradeManager
        upgradeManager = FindFirstObjectByType<SkillUpgradeManager>();
    }

    private void Update()
    {
        if (player == null || isDead)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        // หันหน้าหาผู้เล่น
        FacePlayer();

        // ถ้ากำลังโจมตีให้หยุดเดิน
        if (isAttacking)
            return;

        // ถ้าอยู่นอกระยะตรวจจับ
        if (distance > detectRange)
        {
            SetWalking(false);
            return;
        }

        // ถ้ายังไม่ถึงระยะโจมตี
        if (distance > attackRange)
        {
            SetWalking(true);
            ChasePlayer();
        }
        else
        {
            SetWalking(false);
            TryAttack();
        }
    }

    
    // การเดิน
    private void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position,  moveSpeed * Time.deltaTime);
    }

    private void FacePlayer()
    {
        Vector3 scale = transform.localScale;

        if (player.position.x > transform.position.x)
            scale.x = -Mathf.Abs(scale.x);

        else if (player.position.x < transform.position.x)
            scale.x = Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    
    // การโจมตี
    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        isAttacking = true;

        SetWalking(false);

        if (animator != null)
            animator.SetTrigger("attack");
    }

    private void SetWalking(bool walking)
    {
        if (animator != null)
            animator.SetBool("isWalking", walking);
    }

    public void DealDamage()
    {
        if (player == null || isDead)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;
    }

   
    // พลังชีวิต

    public void TakeDamage(int damageAmount)
    {
        if (isDead)
            return;

        currentHealth -= damageAmount;

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

        // ซ่อนหลอดเลือด Golem
        if (golemHealthUI != null)
            golemHealthUI.HideUI();

        SetWalking(false);

        if (animator != null)
            animator.SetTrigger("dead");

        // รอ Animation ตาย
        Invoke(nameof(DestroyEnemy), 1f);
    }

    private void DestroyEnemy()
    {
        
        // ให้ Point เพื่อเอาไปใช้ Upgrade 
        if (upgradeManager != null)
        {
            upgradeManager.AddUpgradePoints(upgradePointReward);
        }

       
        // Drop Item
        if (dropItemPrefab != null)
        {
            Instantiate(
                dropItemPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        // ลบ gameObject โกเลม
        Destroy(gameObject);
    }

    
    // Shockwave ของสกิลโกเลม
    public void FireShockwave()
    {
        if (player == null || shockwavePrefab == null)
            return;

        Vector2 direction = (player.position - transform.position).normalized;

        Vector3 spawnPosition = transform.position + (Vector3)(direction * shockwaveSpawnDistance);

        GameObject shockwave = Instantiate(shockwavePrefab, spawnPosition, Quaternion.identity);

        Shockwave shockwaveScript = shockwave.GetComponent<Shockwave>();

        if (shockwaveScript != null)
            shockwaveScript.SetDirection(direction);
    }
}