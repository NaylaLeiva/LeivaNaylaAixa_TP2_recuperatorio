using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Generación")]
    [SerializeField] private GameObject enemyPrefab; // Prefab del enemigo a generar
    [SerializeField] private Transform[] spawnPoints; // Puntos donde aparecerán (PointA, PointB, PointC, PointD)
    [SerializeField] private bool spawnOnStart = true; // Generar al iniciar la escena

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnEnemies();
        }
    }

    public void SpawnEnemies()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[EnemySpawner] Falta asignar el Prefab del enemigo o los Spawn Points.");
            return;
        }

        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;

            // Ajusta la posición exacta al NavMesh más cercano para que nunca tire error
            Vector3 spawnPos = point.position;
            if (NavMesh.SamplePosition(point.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                spawnPos = hit.position;
            }

            Instantiate(enemyPrefab, spawnPos, point.rotation);
        }

        Debug.Log($"[EnemySpawner] Se generaron {spawnPoints.Length} enemigos en los puntos asignados.");
    }
}