using System;
using UnityEngine;


public enum GameState // Enum para representar los diferentes estados del juego
{
    Start,
    Playing,
    Paused,
    Victory,
    GameOver
}


public class GameManager : MonoBehaviour
{
    // Patrón Singleton para acceder desde cualquier script sin acoplar referencias
    public static GameManager Instance { get; private set; }

    [Header("Estado del Juego")]
    [SerializeField] private GameState currentState = GameState.Start; //Estado inicial del juego

    [Header("Puntuación y Condición de Victoria")]
    [SerializeField] private int currentScore = 0;
    [SerializeField] private int scoreToWin = 50; 

    [Header("Referencias")]
    [SerializeField] private HealthSystem playerHealth; // Referencia al HealthSystem del jugador

    // Propiedades públicas para acceder a los valores privados
    public GameState CurrentState => currentState;
    public int CurrentScore => currentScore;
    public int ScoreToWin => scoreToWin;

    // Eventos desacoplados para UI o sistemas externos
    public event Action<GameState> OnGameStateChanged; // Evento para notificar cambios de estado del juego
    public event Action<int> OnScoreChanged; // Evento para notificar cambios en el puntaje

    //Implementación del patrón Singleton en Awake para asegurar que solo haya una instancia de GameManager
    private void Awake()
    {
        // Configuración del Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruye la instancia duplicada si ya existe otra
            return;
        }
        Instance = this; // Asigna la instancia actual como la única instancia del GameManager
    }

    private void Start()
    {
        // Si no se asignó en el Inspector, busca automáticamente el HealthSystem del jugador
        if (playerHealth == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player"); // Busca el objeto del jugador por su etiqueta "Player"
            if (playerObj != null) // Si se encuentra el objeto del jugador
                playerHealth = playerObj.GetComponent<HealthSystem>();
        }

        // Pasa por Start y comienza la partida en Playing notificando estado, vida y puntaje
        ChangeState(GameState.Start); 
        ChangeState(GameState.Playing);
        NotifyStatusInConsole("Partida iniciada");
    }

    private void Update()
    {
        // Pausar o reanudar el juego con Escape o P
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (currentState == GameState.Playing) // Si el juego está en estado Playing, cambia a Paused
            {
                ChangeState(GameState.Paused);
            }
            else if (currentState == GameState.Paused) // Si el juego está en estado Paused, cambia a Playing
            {
                ChangeState(GameState.Playing);
            }
        }
    }

    // Cambia el estado del juego y aplica las reglas correspondientes
    public void ChangeState(GameState newState)
    {
        if (currentState == newState) return; // Evita cambiar al mismo estado

        currentState = newState;

        // Aplica las reglas según el estado actual del juego
        switch (currentState)
        {
            // Ajusta el tiempo y el cursor según el estado del juego
            case GameState.Start: // Estado inicial, no hace nada especial
                Time.timeScale = 1f;
                break;

            case GameState.Playing: // Estado de juego activo, reanuda el tiempo y bloquea el cursor
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;

            case GameState.Paused: // Estado de juego pausado, congela el tiempo y desbloquea el cursor
                Time.timeScale = 0f; // Congela el tiempo del juego
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;

            case GameState.Victory: // Estado de victoria, mantiene el tiempo activo para que corra la animación Victory y desbloquea el cursor
                Time.timeScale = 1f; // Mantiene el tiempo activo para que corra la animación Victory
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.GameOver: // Estado de derrota, mantiene el tiempo activo para que corra la animación Die y desbloquea el cursor
                Time.timeScale = 1f; // Mantiene el tiempo activo para que corra la animación Die
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }

        NotifyStatusInConsole($"Cambio de estado -> {currentState}"); // Notifica el cambio de estado en la consola
        OnGameStateChanged?.Invoke(currentState);
    }

    // Suma puntos al recolectar ítems o activar terminales y evalúa Victoria
    public void AddScore(int points)
    {
        // Solo suma puntaje si el juego está en estado Playing
        if (currentState != GameState.Playing) return;

        currentScore += points;
        NotifyStatusInConsole($"+{points} puntos recolectados");
        OnScoreChanged?.Invoke(currentScore); // Notifica el cambio de puntaje a los suscriptores

        // Evalúa si se alcanzó la condición de victoria
        if (currentScore >= scoreToWin)
        {
            Debug.Log("[GameManager] ¡Objetivo de puntos alcanzado! VICTORIA.");
            ChangeState(GameState.Victory); // Cambia el estado a Victory si se alcanza la condición de victoria
        }
    }

    // Reinicia el puntaje
    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore); // Notifica el cambio de puntaje
    }

    // Notifica estado, vida actual y puntaje en consola
    public void NotifyStatusInConsole(string reason = "")
    {
        // Obtiene la vida actual y máxima si existe la referencia a HealthSystem; caso contrario muestra "N/A"
        string healthInfo = playerHealth != null
            ? $"{playerHealth.CurrentHealth}/{playerHealth.MaxHealth}"
            : "N/A";

        // Si se envió un motivo, lo formatea entre paréntesis como prefijo; si no, lo deja vacío
        string prefix = string.IsNullOrEmpty(reason) ? "" : $"({reason}) ";

        // Imprime el resumen completo del estado del juego en la consola de Unity
        Debug.Log($"[GameManager] {prefix}Estado: {currentState} | Vida actual: {healthInfo} | Puntaje: {currentScore}/{scoreToWin}");
    }
}