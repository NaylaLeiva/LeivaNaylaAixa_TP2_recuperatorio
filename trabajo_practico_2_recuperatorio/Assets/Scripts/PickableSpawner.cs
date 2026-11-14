using UnityEngine;

public class PickableSpawner : MonoBehaviour
{
    [Header("Configuración de Generación")]
    [SerializeField] private GameObject pickablePrefab; // Arrastrá acá el Prefab Wall Light
    [SerializeField] private Transform[] spawnPoints;   // Puntos donde aparecerán

    private void Start()
    {
        SpawnAll();
    }

    public void SpawnAll()
    {
        if (pickablePrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[PickableSpawner] Falta asignar el Prefab o los Spawn Points.");
            return;
        }

        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;
            Instantiate(pickablePrefab, point.position, point.rotation);
        }
    }
}