using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Configuración Jugador (Tag: Player)")]
    [SerializeField] private int playerMaxHealth = 100;
    [SerializeField] private float playerImmunityCooldown = 1.2f;
    [SerializeField] private Transform healingStationSpawn;

    [Header("Configuración Enemigo (Tag: Enemy)")]
    [SerializeField] private int enemyMaxHealth = 50;
    [SerializeField] private float enemyImmunityCooldown = 0.1f;

    [Header("Estado Actual (Solo lectura)")]
    [SerializeField] private int currentHealth;
    [SerializeField] private int activeMaxHealth;

    [Header("Referencias")]
    [SerializeField] private Animator animator;

    private float activeImmunityCooldown;
    private float lastDamageTime = -10f;
    private bool isDead = false;
    private bool isPlayer = false;
    private Vector3 respawnPoint;
    private Rigidbody rb;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => activeMaxHealth;
    public bool IsDead => isDead;

    private void Start()
    {
        isPlayer = CompareTag("Player");

        // Asigna automáticamente la vida y el cooldown según sea Player o Enemy
        if (isPlayer)
        {
            activeMaxHealth = playerMaxHealth;
            activeImmunityCooldown = playerImmunityCooldown;
        }
        else
        {
            activeMaxHealth = enemyMaxHealth;
            activeImmunityCooldown = enemyImmunityCooldown;
        }

        currentHealth = activeMaxHealth;
        respawnPoint = transform.position;
        rb = GetComponent<Rigidbody>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        Debug.Log($"[HealthSystem] {gameObject.name} ({(isPlayer ? "Player" : "Enemy")}) iniciado | Salud: {currentHealth}/{activeMaxHealth}");
    }

    private void Update()
    {
        if (isDead && isPlayer && Input.GetKeyDown(KeyCode.R))
        {
            RespawnAtStation();
        }
    }

    public void TakeDamage(int amount, bool ignoreCooldown = false)
    {
        if (isDead) return;
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        if (!ignoreCooldown && Time.time < lastDamageTime + activeImmunityCooldown)
            return;

        if (!ignoreCooldown)
            lastDamageTime = Time.time;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, activeMaxHealth);
        Debug.Log($"[HealthSystem] {gameObject.name} recibió -{amount} daño | Salud: {currentHealth}/{activeMaxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        int previous = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, activeMaxHealth);

        if (currentHealth > previous)
        {
            Debug.Log($"[HealthSystem] {gameObject.name} curado: +{currentHealth - previous} | Salud: {currentHealth}/{activeMaxHealth}");
        }
    }

    private void Die()
    {
        isDead = true;

        if (isPlayer)
        {
            Debug.Log("[HealthSystem] Jugador derrotado. Presioná 'R' para revivir en la Estación de Curación.");

            if (animator != null)
                animator.SetTrigger("Die");

            if (GameManager.Instance != null)
                GameManager.Instance.ChangeState(GameState.GameOver);
        }
        else
        {
            Debug.Log($"[HealthSystem] Enemigo {gameObject.name} eliminado.");
            gameObject.SetActive(false);
        }
    }

    public void RespawnAtStation()
    {
        isDead = false;
        currentHealth = activeMaxHealth;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Vector3 targetPos = healingStationSpawn != null ? healingStationSpawn.position : respawnPoint;
        transform.position = targetPos + Vector3.up * 1f;

        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.Play("Idle");
        }

        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameState.Playing);

        Debug.Log($"[HealthSystem] Jugador revivido en la Estación de Curación | Salud: {currentHealth}/{activeMaxHealth}");
    }
}