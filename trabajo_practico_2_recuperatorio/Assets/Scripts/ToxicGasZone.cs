using UnityEngine;

public class ToxicGasZone : MonoBehaviour
{
    [Header("Efecto de Daño por Tics")]
    [SerializeField] private int damagePerTick = 3;
    [SerializeField] private float damageInterval = 1f;
    private float nextDamageTime = 0f;

    //Aplica daño al jugador mientras esté dentro de la zona de gas tóxico
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time >= nextDamageTime)
            {
                // Aplica daño al jugador si tiene un sistema de salud y no está muerto
                HealthSystem playerHealth = other.GetComponent<HealthSystem>();
                if (playerHealth != null && !playerHealth.IsDead)
                {
                    playerHealth.TakeDamage(damagePerTick); // Aplica el daño al jugador
                    nextDamageTime = Time.time + damageInterval; // Renueva el tiempo para el siguiente tic
                }
            }
        }
    }
}