using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Generación")]
    [SerializeField] private GameObject enemyPrefab; 
    [SerializeField] private Transform[] spawnPoints; 

    [Header("Configuración de Tiempo (InvokeRepeating)")]
    [SerializeField] private float initialDelay = 2f;  
    [SerializeField] private float spawnInterval = 10f; 

    private bool isSpawning = false;

    private void Start()
    {
        // Inicia la generación de enemigos al comenzar el juego
        StartSpawning();
    }

    private void Update()
    {
        // Monitorea el estado del juego mediante el GameManager
        if (GameManager.Instance != null)
        {
            // Si el juego NO está en estado Playing (ej. Murió, Game Over, Pausa), detenemos el spawner
            if (GameManager.Instance.CurrentState != GameState.Playing && isSpawning)
            {
                StopSpawning();
            }
            // Si el juego vuelve a estar en Playing (ej. el jugador revivió con la tecla 'R'), reactivamos el spawner
            else if (GameManager.Instance.CurrentState == GameState.Playing && !isSpawning)
            {
                StartSpawning();
            }
        }
    }

    // Inicia la generación de enemigos usando InvokeRepeating
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            InvokeRepeating(nameof(SpawnEnemies), initialDelay, spawnInterval);
            isSpawning = true;
            Debug.Log("[EnemySpawner] Generador de enemigos activado.");
        }
    }

    // Detiene la generación de enemigos cancelando InvokeRepeating
    public void StopSpawning()
    {
        if (isSpawning)
        {
            CancelInvoke(nameof(SpawnEnemies));
            isSpawning = false;
            Debug.Log("[EnemySpawner] Generador de enemigos pausado/detenido.");
        }
    }

    // Genera enemigos en los puntos de spawn definidos
    public void SpawnEnemies()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing)
            return;

        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[EnemySpawner] Falta asignar el Prefab del enemigo o los Spawn Points.");
            return;
        }

        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;

            // Valida y ajusta la posición al NavMesh más cercano
            Vector3 spawnPos = point.position;
            if (NavMesh.SamplePosition(point.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                spawnPos = hit.position;
            }

            Instantiate(enemyPrefab, spawnPos, point.rotation);
        }

        Debug.Log($"[EnemySpawner] Oleada generada ({spawnPoints.Length} unidades).");
    }

    // Asegura que la generación de enemigos se detenga si el objeto spawner se desactiva
    private void OnDisable()
    {
        StopSpawning();
    }
}