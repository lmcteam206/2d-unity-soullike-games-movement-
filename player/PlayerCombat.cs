using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public LayerMask enemyLayers;
    public float comboResetTime = 0.5f;

    [Header("Charged Attack Settings")]
    public float chargeTime = 1.5f;
    public int chargedAttackDamage = 3;

    [Header("Ranged Attack Settings")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileCooldown = 1f;

    private Animator anim;
    private int comboStep = 0;
    private float lastAttackTime = 0f;
    private bool isCharging = false;
    private float chargeStartTime;
    private float lastProjectileTime = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleComboAttack();
        HandleChargedAttack();
        HandleRangedAttack();
    }

    // ------------------------- COMBO ATTACK -------------------------
    void HandleComboAttack()
    {
        if (Time.time - lastAttackTime > comboResetTime) comboStep = 0;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (comboStep < 3) comboStep++;
            Attack(comboStep);
            lastAttackTime = Time.time;
        }
    }

    void Attack(int comboIndex)
    {
        anim.SetTrigger("Attack" + comboIndex); // Animator triggers: Attack1, Attack2, Attack3

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    // ------------------------- CHARGED ATTACK -------------------------
    void HandleChargedAttack()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            isCharging = true;
            chargeStartTime = Time.time;
            anim.SetBool("Charging", true);
        }
        if (Input.GetKeyUp(KeyCode.X) && isCharging)
        {
            isCharging = false;
            anim.SetBool("Charging", false);

            if (Time.time - chargeStartTime >= chargeTime)
            {
                ChargedAttack();
            }
        }
    }

    void ChargedAttack()
    {
        anim.SetTrigger("ChargedAttack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange * 1.5f, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(chargedAttackDamage);
        }
    }

    // ------------------------- RANGED ATTACK -------------------------
    void HandleRangedAttack()
    {
        if (Input.GetKeyDown(KeyCode.C) && Time.time - lastProjectileTime >= projectileCooldown)
        {
            FireProjectile();
            lastProjectileTime = Time.time;
        }
    }

    void FireProjectile()
    {
        anim.SetTrigger("RangedAttack");
        Instantiate(projectilePrefab, projectileSpawnPoint.position, transform.rotation);
    }

    // ------------------------- DEBUG & VISUALS -------------------------
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
