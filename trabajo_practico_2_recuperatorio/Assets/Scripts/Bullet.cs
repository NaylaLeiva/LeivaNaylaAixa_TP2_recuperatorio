using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 80f; // Velocidad visual de la bala
    private Vector3 targetPoint;
    private bool hasTarget = false;

    public void SetTarget(Vector3 target)
    {
        targetPoint = target;
        hasTarget = true;
    }

    private void Update()
    {
        if (!hasTarget) return;

        // Mueve la bala en línea recta hacia el punto donde impactó el Raycast
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        // Cuando llega al punto de impacto, se destruye
        if (Vector3.Distance(transform.position, targetPoint) <= 0.05f)
        {
            Destroy(gameObject);
        }
    }
}