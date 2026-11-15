using UnityEngine;

public class MudZone : MonoBehaviour
{
    [Header("Efecto de Ralentización")]
    [SerializeField] private float speedMultiplier = 0.5f;

    [Header("Efecto de Daño por Tics")]
    [SerializeField] private int damagePerTick = 3;
    [SerializeField] private float damageInterval = 1f;
    private float nextDamageTime = 0f;

    //Modifica la velocidad del jugador al entrar en la zona de lodo
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                //Aplica el multiplicador de velocidad al jugador
                player.SetSpeedMultiplier(speedMultiplier);
            }
        }
    }

    //Aplica daño al jugador mientras esté dentro de la zona de lodo
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Controla que el daño ocurra solo cada cierto intervalo de tiempo (cada 1 segundo)
            if (Time.time >= nextDamageTime)
            {
                HealthSystem playerHealth = other.GetComponent<HealthSystem>();
                // Aplica daño al jugador si tiene un sistema de salud y no está muerto
                if (playerHealth != null && !playerHealth.IsDead)
                {
                    playerHealth.TakeDamage(damagePerTick); // Aplica el daño al jugador
                    nextDamageTime = Time.time + damageInterval; // Renueva el tiempo para el siguiente tic
                }
            }
        }
    }

    // Restaura la velocidad normal del jugador al salir de la zona de lodo
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Restaura la velocidad normal del jugador al salir de la zona de lodo
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetSpeedMultiplier(1f); // Restaura la velocidad normal al salir
            }
        }
    }
}