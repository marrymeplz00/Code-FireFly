using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Footstep Sound")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip footstepClip;
    [SerializeField, Range(0f, 1f)] private float footstepVolume = 0.7f;

    [Header("Attack Sounds")]
    [SerializeField] private AudioSource attackAudioSource;
    [SerializeField] private AudioClip normalAttackSound;
    [SerializeField] private AudioClip skillESound;
    [SerializeField] private AudioClip skillRSound;
    [SerializeField] private AudioClip healWSound;
    [SerializeField, Range(0f, 1f)] private float attackSoundVolume = 0.8f;

    [Header("Normal Attack")]
    public float attackRange = 1.5f;
    public int damage = 20;
    public float attackCooldown = 0.8f;

    [Range(1, 3)] public int attackLevel = 1;
    public GameObject normalAttackEffectPrefab;
    public float normalAttackEffectSpeed = 20f;

    [Header("Magic Skill E")]
    [Range(1, 3)] public int eLevel = 1;
    public GameObject eSpellPrefab;
    public float eSpeed = 7f;
    public float eCooldown = 3f;
    public float eSpreadAngle = 15f;

    [Header("Skill R")]
    [Range(1, 3)] public int rLevel = 1;
    public float rAttackRange = 4f;
    public int rDamage = 40;
    public float rCooldown = 5f;
    public GameObject rAttackEffectPrefab;

    [Header("Heal W")]
    [Range(1, 3)] public int wLevel = 1;
    public int healAmount = 20;
    public float healCooldown = 8f;
    public GameObject healEffectPrefab;

    [Header("Stamina Cost")]
    public float wStaminaCost = 20f;
    public float eStaminaCost = 20f;
    public float rStaminaCost = 46f;

    [Header("Cursor")]
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;

    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public string speedParameter = "Speed";

    private PlayerHealth playerHealth;
    private PlayerStamina playerStamina;

    private Vector3 targetPosition;

    private float lastAttackTime;
    private float lastETime;
    private float lastRTime;
    private float lastHealTime;

    private bool isDead;
    private int normalAttackCombo;

    private void Start()
    {
        targetPosition = transform.position;

        playerHealth = GetComponent<PlayerHealth>();
        playerStamina = GetComponent<PlayerStamina>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // ตั้งค่า AudioSource เสียงเดิน
        if (footstepAudioSource == null)
        {
            footstepAudioSource = GetComponent<AudioSource>();
        }

        if (footstepAudioSource != null)
        {
            footstepAudioSource.playOnAwake = false;
            footstepAudioSource.loop = false;
            footstepAudioSource.Stop();
        }
        else
        {
            Debug.LogWarning(
                "PlayerControl: ไม่พบ AudioSource สำหรับเสียงเดิน"
            );
        }

        // สร้าง AudioSource แยกสำหรับเสียงโจมตี
        if (attackAudioSource == null || attackAudioSource == footstepAudioSource)
        {
            attackAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (attackAudioSource != null)
        {
            attackAudioSource.playOnAwake = false;
            attackAudioSource.loop = false;
            attackAudioSource.spatialBlend = 0f;
            attackAudioSource.volume = 1f;
            attackAudioSource.mute = false;
            attackAudioSource.enabled = true;
            attackAudioSource.Stop();
        }

        if (animator == null)
        {
            Debug.LogError("PlayerControl: ไม่พบ Animator ของตัวละคร");
        }
        else if (!HasParameter(
            speedParameter,
            AnimatorControllerParameterType.Float))
        {
            Debug.LogError(
                "PlayerControl: ไม่พบ Float Parameter ชื่อ '" + speedParameter + "' ใน Animator");
        }

        if (cursorTexture != null)
            Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }

    private void Update()
    {
        if (isDead) return;

        HandleMovement();
        HandleNormalAttack();
        HandleSkillE();
        HandleSkillR();
        HandleHeal();
    }

    // Movement + Blend Tree + Footstep Sound
    private void HandleMovement()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SetTargetPosition();
            UpdateFacingDirection();
        }

        bool isWalking = Vector2.Distance(transform.position, targetPosition) > 0.05f;

        if (isWalking)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPosition) <= 0.05f)
            {
                transform.position = targetPosition;
                isWalking = false;
            }
        }

        HandleFootstepSound(isWalking);

        if (animator != null &&
            HasParameter(speedParameter, AnimatorControllerParameterType.Float))
        {
            animator.SetFloat(speedParameter, isWalking ? 1f : 0f);
        }
    }

    private void HandleFootstepSound(bool isWalking)
    {
        if (footstepAudioSource == null)
            return;

        if (footstepClip == null)
        {
            if (footstepAudioSource.isPlaying)
                footstepAudioSource.Stop();

            return;
        }

        if (isWalking)
        {
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.clip = footstepClip;
                footstepAudioSource.volume = footstepVolume;
                footstepAudioSource.loop = true;
                footstepAudioSource.Play();
            }
        }
        else
        {
            if (footstepAudioSource.isPlaying)
                footstepAudioSource.Stop();

            footstepAudioSource.loop = false;
        }
    }

    // Attack Sound
    private void PlayAttackSound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning(
                "PlayerControl: ไม่มี AudioClip ของเสียงที่เรียกเล่น"
            );
            return;
        }

        if (attackAudioSource == null)
        {
            Debug.LogError(
                "PlayerControl: ไม่พบ AudioSource สำหรับเสียงโจมตี"
            );
            return;
        }

        // กันไม่ให้เสียงโจมตีถูกระบบเสียงเดินหยุด
        if (attackAudioSource == footstepAudioSource)
        {
            Debug.LogError(
                "PlayerControl: AudioSource เสียงโจมตีซ้ำกับเสียงเดิน"
            );
            return;
        }

        // ตั้งค่าเสียงโจมตีทุกครั้งก่อนเล่น
        attackAudioSource.enabled = true;
        attackAudioSource.mute = false;
        attackAudioSource.volume = 1f;
        attackAudioSource.loop = false;
        attackAudioSource.spatialBlend = 0f;

        Debug.Log(
            "PlayerControl: กำลังเล่นเสียง " + clip.name
        );

        // ใช้ PlayOneShot เพื่อเล่นเสียงโมตีโดยไม่ต้องเดิน
        attackAudioSource.PlayOneShot(clip, attackSoundVolume);
    }

    private void SetTargetPosition()
    {
        if (Camera.main == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePosition.z = 0f;
        targetPosition = mousePosition;
    }

    private void UpdateFacingDirection()
    {
        if (spriteRenderer == null) return;

        if (targetPosition.x > transform.position.x)
            spriteRenderer.flipX = false;
        else if (targetPosition.x < transform.position.x)
            spriteRenderer.flipX = true;
    }

    // Animation Helpers
    private bool HasParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (animator == null) return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == parameterType)
            {
                return true;
            }
        }

        return false;
    }

    private void PlayAnimation(string animationName)
    {
        if (animator == null) return;

        if (HasParameter(
            animationName, AnimatorControllerParameterType.Trigger))
        {
            animator.SetTrigger(animationName);
        }
        else
        {
            Debug.LogWarning( "ไม่พบ Trigger '" + animationName + "' ใน Animator");
        }
    }

    private void ResetAttackTriggers()
    {
        if (animator == null) return;

        string[] triggers = {"NormalAttack1", "NormalAttack2", "NormalAttack3"};

        foreach (string trigger in triggers)
        {
            if (HasParameter(trigger, AnimatorControllerParameterType.Trigger))
            {
                animator.ResetTrigger(trigger);
            }
        }
    }

    // การตาย
    public void PlayDeathAnimation()
    {
        if (isDead) return;

        isDead = true;
        targetPosition = transform.position;

        if (footstepAudioSource != null)
            footstepAudioSource.Stop();

        if (attackAudioSource != null)
            attackAudioSource.Stop();

        if (animator != null)
        {
            if (HasParameter(speedParameter, AnimatorControllerParameterType.Float))
            {
                animator.SetFloat(speedParameter, 0f);
            }

            ResetAttackTriggers();

            string[] triggers = {"Attack", "SkillE", "SkillR", "HealW"};

            foreach (string trigger in triggers)
            {
                if (HasParameter(trigger, AnimatorControllerParameterType.Trigger))
                {
                    animator.ResetTrigger(trigger);
                }
            }

            PlayAnimation("Dead");
        }
        else
        {
            OnDeathAnimationFinished();
        }
    }

    // Animation Event เฟรมสุดท้ายของท่าตาย
    public void OnDeathAnimationFinished()
    {
        if (!isDead) return;

        if (playerHealth != null)
            playerHealth.LoadGameOver();
    }

    private void HandleNormalAttack()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        normalAttackCombo++;

        if (normalAttackCombo > 3)
            normalAttackCombo = 1;

        PlayAnimation("NormalAttack" + normalAttackCombo);
        PlayAttackSound(normalAttackSound);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackRange);

        List<Collider2D> targets = GetEnemyTargets(colliders);

        SortTargetsByDistance(targets);

        int targetCount = GetNormalAttackTargetCount();

        for (int i = 0; i < Mathf.Min(targetCount, targets.Count); i++)
            AttackTarget(targets[i]);
    }

    private List<Collider2D> GetEnemyTargets(Collider2D[] colliders)
    {
        List<Collider2D> targets = new List<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy") &&
                collider.GetComponent<Enemy>() != null)
            {
                targets.Add(collider);
            }
        }

        return targets;
    }

    private void SortTargetsByDistance(List<Collider2D> targets)
    {
        targets.Sort((a, b) => Vector2.Distance(transform.position, a.transform.position) .CompareTo( Vector2.Distance(transform.position, b.transform.position)));
    }

    private int GetNormalAttackTargetCount()
    {
        return attackLevel switch
        {
            1 => 1, 
            2 => 3, 
            3 => 5, 
            _ => 1
        };
    }

    private void AttackTarget(Collider2D target)
    {
        SpawnNormalAttackEffect(target.transform.position);

        Enemy enemy = target.GetComponent<Enemy>();

        if (enemy != null)
            enemy.TakeDamage(damage);
    }

    private void SpawnNormalAttackEffect(Vector3 targetPosition)
    {
        if (normalAttackEffectPrefab == null) return;

        GameObject effect = Instantiate(normalAttackEffectPrefab, transform.position, Quaternion.identity);

        NormalAttackEffect attackEffect = effect.GetComponent<NormalAttackEffect>();

        if (attackEffect != null)
            attackEffect.SetTarget(targetPosition, normalAttackEffectSpeed);
    }

    // Skill E
    private void HandleSkillE()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        if (Time.time < lastETime + eCooldown) return;
        if (!TryUseStamina(eStaminaCost)) return;

        lastETime = Time.time;
        PlayAnimation("SkillE");
        PlayAttackSound(skillESound);

        Vector2 direction = GetMouseDirection();
        int bulletCount = GetEBulletCount();
        int skillDamage = GetEDamage();

        for (int i = 0; i < bulletCount; i++)
        {
            Vector2 bulletDirection = GetESpreadDirection(direction, i, bulletCount);

            CastMagic(eSpellPrefab, skillDamage, eSpeed, bulletDirection);
        }
    }

    private int GetEBulletCount()
    {
        return eLevel == 1 ? 1 : 3;
    }

    private int GetEDamage()
    {
        return eLevel == 3 ? 40 : 25;
    }

    private Vector2 GetESpreadDirection(Vector2 direction, int index, int bulletCount)
    {
        if (bulletCount == 1) return direction;

        float angle = (index - 1) * eSpreadAngle;

        return Quaternion.Euler(0f, 0f, angle) * direction;
    }

    private Vector2 GetMouseDirection()
    {
        if (Camera.main == null) return Vector2.right;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePosition.z = 0f;

        Vector2 direction = mousePosition - transform.position;

        return direction.sqrMagnitude > 0f ? direction.normalized : Vector2.right;
    }

    private void CastMagic(GameObject spellPrefab, int skillDamage, float skillSpeed, Vector2 direction)
    {
        if (spellPrefab == null) return;

        GameObject spell = Instantiate(spellPrefab, transform.position, Quaternion.identity);

        MagicSpell magicSpell = spell.GetComponent<MagicSpell>();

        if (magicSpell != null)
        {
            magicSpell.damage = skillDamage;
            magicSpell.speed = skillSpeed;
            magicSpell.Cast(direction);
        }
    }

    // Skill R
    private void HandleSkillR()
    {
        if (!Input.GetKeyDown(KeyCode.R)) return;
        if (Time.time < lastRTime + rCooldown) return;
        if (!TryUseStamina(rStaminaCost)) return;

        lastRTime = Time.time;
        PlayAnimation("SkillR");
        PlayAttackSound(skillRSound);

        SpawnRAttackEffect();
        DamageEnemiesInRange(GetRRange());
    }

    private float GetRRange()
    {
        return rLevel switch
        {
            1 => 4f, 
            2 => 6f, 
            3 => 12f, 
            _ => 4f
        };
    }

    private void SpawnRAttackEffect()
    {
        if (rAttackEffectPrefab == null) return;

        Instantiate(rAttackEffectPrefab, transform.position, Quaternion.identity);
    }

    private void DamageEnemiesInRange(float range)
    {
        Collider2D[] enemies =
            Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D collider in enemies)
        {
            if (!collider.CompareTag("Enemy")) continue;

            Enemy enemy = collider.GetComponent<Enemy>();

            if (enemy != null)
                enemy.TakeDamage(rDamage);
        }
    }

    // Heal W
    private void HandleHeal()
    {
        if (!Input.GetKeyDown(KeyCode.W)) return;
        if (Time.time < lastHealTime + healCooldown) return;
        if (!TryUseStamina(wStaminaCost)) return;

        lastHealTime = Time.time;
        PlayAnimation("HealW");
        PlayAttackSound(healWSound);

        int amount = GetHealAmount();

        if (playerHealth != null)
            playerHealth.Heal(amount);

        SpawnHealEffect();
    }

    private int GetHealAmount()
    {
        return wLevel switch
        {
            1 => 20,
            2 => 40,
            3 => 55,
            _ => 20
        };
    }

    private void SpawnHealEffect()
    {
        if (healEffectPrefab == null) return;

        GameObject effect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity, transform);

        Destroy(effect, 2f);
    }

    // Stamina
    private bool TryUseStamina(float amount)
    {
        if (playerStamina == null) return true;

        return playerStamina.UseStamina(amount);
    }

    // Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.DrawWireSphere(transform.position, GetGizmoRRange());
    }

    private float GetGizmoRRange()
    {
        return rLevel switch
        {
            1 => 4f,
            2 => 6f,
            3 => 12f,
            _ => 4f
        };
    }
}