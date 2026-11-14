using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Estado del Arma y Munición")]
    [SerializeField] private bool hasWeapon = false;
    [SerializeField] private GameObject equippedWeaponVisual;
    [SerializeField] private int currentAmmo = 150; // Balas iniciales con las que viene cargada el arma
    [SerializeField] private int maxAmmo = 150; // Máximo de balas que puede llevar

    [Header("Configuración de Disparo")]
    [SerializeField] private GameObject bulletPrefab; // Prefab visual de la bala
    [SerializeField] private Transform firePoint; // Punta del cañón del arma
    [SerializeField] private Camera playerCamera; // Cámara principal
    [SerializeField] private int damage = 35;
    [SerializeField] private float range = 60f;
    [SerializeField] private float fireRate = 0.35f;
    [SerializeField] private LayerMask hitLayers = ~0;

    private float nextFireTime = 0f;

    public bool HasWeapon => hasWeapon;
    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (equippedWeaponVisual != null)
            equippedWeaponVisual.SetActive(hasWeapon);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!hasWeapon)
            {
                Debug.LogWarning("[PlayerAttack] No dispara: Has Weapon está en FALSE (todavía no agarraste el arma).");
                return;
            }
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            {
                Debug.LogWarning($"[PlayerAttack] No dispara: El juego está en estado {GameManager.Instance.CurrentState}, no en Playing.");
                return;
            }
            if (currentAmmo <= 0)
            {
                Debug.LogWarning("[PlayerAttack] No dispara: Current Ammo es 0.");
                return;
            }

            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate;
                currentAmmo--;
                Shoot();
                Debug.Log($"[PlayerAttack] ¡DISPARÓ! Balas restantes: {currentAmmo}/{maxAmmo}");
            }
        }
    }

    public void EquipWeapon()
    {
        hasWeapon = true;

        if (equippedWeaponVisual != null)
            equippedWeaponVisual.SetActive(true);

        Debug.Log($"[PlayerAttack] ¡Arma equipada! Balas iniciales: {currentAmmo}/{maxAmmo}");
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        Debug.Log($"[PlayerAttack] +{amount} balas recogidas | Total: {currentAmmo}/{maxAmmo}");
    }

    private void Shoot()
    {
        if (playerCamera == null) return;

        // 1. Rayo invisible desde el centro exacto de la cámara
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 2f);
        Vector3 targetPoint = ray.GetPoint(range);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayers, QueryTriggerInteraction.Ignore))
        {
            targetPoint = hit.point;

            HealthSystem targetHealth = hit.collider.GetComponentInParent<HealthSystem>();
            if (targetHealth != null && targetHealth.gameObject != gameObject)
            {
                targetHealth.TakeDamage(damage, ignoreCooldown: true);
                Debug.Log($"[PlayerAttack] Impacto en {targetHealth.gameObject.name} (-{damage} HP)");
            }
        }

        // 2. Instancia la bala visual desde el cañón hacia el punto de impacto del rayo
        if (bulletPrefab != null && firePoint != null)
        {
            Vector3 direction = (targetPoint - firePoint.position).normalized;
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));

            if (bulletObj.TryGetComponent(out Bullet bullet))
            {
                bullet.SetTarget(targetPoint);
            }
        }
        else
        {
            Debug.LogWarning("[PlayerAttack] Falta asignar Bullet Prefab o Fire Point en el Inspector.");
        }
    }
}