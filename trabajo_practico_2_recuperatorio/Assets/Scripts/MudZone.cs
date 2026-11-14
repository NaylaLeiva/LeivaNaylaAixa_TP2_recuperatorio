using UnityEngine;

public class MudZone : MonoBehaviour
{
    [Header("Efecto de Ralentización")]
    [SerializeField] private float speedMultiplier = 0.5f; 

    [Header("Efecto de Daño")]
    [SerializeField] private int damagePerTick = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetSpeedMultiplier(speedMultiplier);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            if (playerHealth != null && !playerHealth.IsDead)
            {
                playerHealth.TakeDamage(damagePerTick);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetSpeedMultiplier(1f); // Restaura la velocidad normal
            }
        }
    }
}