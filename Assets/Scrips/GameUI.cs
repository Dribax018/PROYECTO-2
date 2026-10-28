using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra el marcador durante la partida y el panel de fin de partida
/// con la puntuación, el récord y el botón de nueva partida.
/// </summary>
public class GameUI : MonoBehaviour
{
    [SerializeField] private GameSession session;

    [Header("Durante la partida")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Fin de partida")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text bestScoreText;
    [Tooltip("Opcional: objeto que se activa solo si se ha batido el récord")]
    [SerializeField] private GameObject newBestLabel;
    [SerializeField] private Button restartButton;

    private void OnEnable()
    {
        session.OnScoreChanged += ShowScore;
        session.OnGameOver += ShowGameOver;
        restartButton.onClick.AddListener(session.Restart);
    }

    private void OnDisable()
    {
        session.OnScoreChanged -= ShowScore;
        session.OnGameOver -= ShowGameOver;
        restartButton.onClick.RemoveListener(session.Restart);
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
        ShowScore(0);
    }

    private void ShowScore(int score)
    {
        scoreText.text = score.ToString();
    }

    private void ShowGameOver(int score, int best, bool isNewBest)
    {
        scoreText.gameObject.SetActive(false);
        gameOverPanel.SetActive(true);

        finalScoreText.text = $"Puntuación: {score}";
        bestScoreText.text = $"Récord: {best}";
        if (newBestLabel != null) newBestLabel.SetActive(isNewBest);
    }
}