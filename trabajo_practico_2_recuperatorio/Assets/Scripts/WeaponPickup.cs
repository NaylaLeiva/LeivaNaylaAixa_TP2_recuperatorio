using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    // Método que se llama cuando un objeto entra en el trigger del arma
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto que colisiona tiene la etiqueta "Player"
        if (!other.CompareTag("Player")) return;

        // Intenta obtener el componente PlayerAttack del objeto que colisiona o de sus padres
        PlayerAttack playerAttack = other.GetComponentInParent<PlayerAttack>();
        // Si se encuentra el componente PlayerAttack, equipa el arma y destruye el objeto del arma en el suelo
        if (playerAttack != null)
        {
            // Llama al método EquipWeapon del PlayerAttack para equipar el arma
            playerAttack.EquipWeapon();
            Destroy(gameObject); // Elimina el arma del suelo al agarrarla
        }
    }
}