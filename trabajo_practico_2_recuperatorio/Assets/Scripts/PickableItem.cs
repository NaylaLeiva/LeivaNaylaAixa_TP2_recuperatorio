using UnityEngine;

public class PickableItem : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int pointsValue = 10;
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private float rotateSpeed = 90f;

    private bool playerInRange = false;

    private void Update()
    {
        // Rotación visual opcional
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        // Si el jugador está dentro del área y presiona F
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            Collect();
        }
    }

    private void Collect()
    {
        // Suma puntos en tu GameManager (verificá que el método exista en tu GameManager)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(pointsValue);
        }

        Debug.Log($"[Pickable] Recogido con F (+{pointsValue} puntos).");

        // Desaparece del escenario
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Presioná 'F' para recoger.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}