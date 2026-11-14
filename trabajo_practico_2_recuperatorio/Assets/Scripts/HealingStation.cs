using UnityEngine;

public class HealingStation : MonoBehaviour
{
    [Header("Curación Progresiva")]
    [SerializeField] private int healPerTick = 10;          // +10 HP por pulso
    [SerializeField] private float healInterval = 1.5f;     // Cada 1.5 segundos

    [Header("Límite de Carga y Cooldown")]
    [SerializeField] private int maxChargeCapacity = 50;    // Curación máxima antes de agotarse
    [SerializeField] private float rechargeCooldown = 8f;   // Segundos deshabilitada

    [Header("Retroalimentación Visual")]
    [SerializeField] private Light stationLight;
    [SerializeField] private ParticleSystem stationParticles;

    private int currentCharge;
    private float nextHealTime;
    private bool isRecharging;

    private void Start()
    {
        // Inicializa la estación con carga completa y activa la retroalimentación visual
        currentCharge = maxChargeCapacity;
        SetVisualsActive(true);
    }

    // Método que se llama cuando un objeto entra en el trigger de la estación de curación
    private void OnTriggerStay(Collider other)
    {
        if (isRecharging) return;
        if (!other.CompareTag("Player")) return;

        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health == null || health.CurrentHealth >= health.MaxHealth) return;

        if (Time.time >= nextHealTime)
        {
            int amountToHeal = Mathf.Min(healPerTick, currentCharge);
            health.Heal(amountToHeal);
            currentCharge -= amountToHeal;
            nextHealTime = Time.time + healInterval;

            Debug.Log($"[HealingStation] Curando +{amountToHeal} HP | Carga restante: {currentCharge}/{maxChargeCapacity}");

            if (currentCharge <= 0)
            {
                StartCoroutine(RechargeRoutine());
            }
        }
    }

    // Rutina de recarga de la estación después de agotarse
    private System.Collections.IEnumerator RechargeRoutine()
    {
        isRecharging = true;
        SetVisualsActive(false);
        Debug.Log($"[HealingStation] Estación en Recarga ({rechargeCooldown}s)...");

        yield return new WaitForSeconds(rechargeCooldown);

        currentCharge = maxChargeCapacity;
        isRecharging = false;
        SetVisualsActive(true);
        Debug.Log("[HealingStation] Estación recargada y lista.");
    }

    // Método para activar o desactivar la retroalimentación visual de la estación
    private void SetVisualsActive(bool active)
    {
        if (stationLight != null) stationLight.enabled = active;
        if (stationParticles != null)
        {
            if (active) stationParticles.Play();
            else stationParticles.Stop();
        }
    }
}