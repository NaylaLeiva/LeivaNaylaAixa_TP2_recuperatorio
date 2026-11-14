using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    [SerializeField] private float mouseSensitivity = 8f; // Sensibilidad del ratón
    [SerializeField] private float minPitch = -60f; // Límite inferior de inclinación vertical
    [SerializeField] private float maxPitch = 60f;  // Límite superior de inclinación vertical

    private float cameraXRotation = 0f; // Ángulo acumulado en el eje X (mirar arriba/abajo)

    private void Update()
    {
        // No mueve la cámara si el juego está pausado, en victoria o derrota
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        // Lee el movimiento vertical del mouse y calcula la rotación
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        cameraXRotation -= mouseY;
        cameraXRotation = Mathf.Clamp(cameraXRotation, minPitch, maxPitch);

        // Aplica la rotación vertical suavemente al CameraHolder
        transform.localRotation = Quaternion.Euler(cameraXRotation, 0f, 0f);
    }
}