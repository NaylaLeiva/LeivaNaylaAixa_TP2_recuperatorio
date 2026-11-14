using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Paneles de Pantallas")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Textos del HUD")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text scoreText;

    [Header("Referencias")]
    [SerializeField] private HealthSystem playerHealth;

    private void Start()
    {
        if (GameManager.Instance != null) // Verifica si la instancia de GameManager existe antes de suscribirse a los eventos
        {
            GameManager.Instance.OnGameStateChanged += UpdatePanels;
            GameManager.Instance.OnScoreChanged += UpdateScoreText;
            // Actualiza los paneles y el texto de puntuación al iniciar
            UpdatePanels(GameManager.Instance.CurrentState);
            UpdateScoreText(GameManager.Instance.CurrentScore);
        }
    }

    // Desuscribirse de los eventos al destruir el objeto para evitar errores
    private void OnDestroy()
    {
        // Verifica si la instancia de GameManager existe antes de desuscribirse de los eventos
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= UpdatePanels;
            GameManager.Instance.OnScoreChanged -= UpdateScoreText;
        }
    }

    // Actualiza la barra de vida del jugador en cada frame
    private void Update()
    {
        if (playerHealth != null && healthText != null)
        {
            healthText.text = $"Vida: {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
        }
    }

    // Actualiza la visibilidad de los paneles según el estado del juego
    private void UpdatePanels(GameState state)
    {
        if (hudPanel != null) hudPanel.SetActive(state == GameState.Playing);
        if (pausePanel != null) pausePanel.SetActive(state == GameState.Paused);
        if (victoryPanel != null) victoryPanel.SetActive(state == GameState.Victory);
        if (gameOverPanel != null) gameOverPanel.SetActive(state == GameState.GameOver);
    }

    // Actualiza el texto de puntuación en el HUD
    private void UpdateScoreText(int currentScore)
    {
        if (scoreText != null && GameManager.Instance != null)
        {
            scoreText.text = $"Puntos: {currentScore}/{GameManager.Instance.ScoreToWin}";
        }
    }
}