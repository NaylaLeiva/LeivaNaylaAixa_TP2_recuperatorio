using UnityEngine;

public class MudZone : MonoBehaviour
{
    [Header("Efecto de Ralentización")]
    [SerializeField] private float speedMultiplier = 0.5f; 

    [Header("Efecto de Daño")]
    [SerializeField] private int damagePerTick = 3;

    // Método para establecer el multiplicador de velocidad en PlayerController
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que entra en la zona de lodo es el jugador
        if (other.CompareTag("Player"))
        {
            // Obtiene el componente PlayerController del jugador
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Aplica el efecto de ralentización al jugador
                player.SetSpeedMultiplier(speedMultiplier);
            }
        }
    }

    // Método para aplicar daño al jugador mientras esté dentro de la zona de lodo
    private void OnTriggerStay(Collider other)
    {
        // Verifica si el objeto que está dentro de la zona de lodo es el jugador
        if (other.CompareTag("Player"))
        {
            // Obtiene el componente HealthSystem del jugador
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();
            // Aplica daño al jugador si tiene un HealthSystem y no está muerto
            if (playerHealth != null && !playerHealth.IsDead)
            {
                // Aplica daño al jugador
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