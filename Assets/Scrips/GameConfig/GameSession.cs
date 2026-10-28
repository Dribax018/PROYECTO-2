using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla el estado de la partida: puntuación (filas avanzadas), récord guardado,
/// fin de partida y nueva partida. Nueva partida = recargar la escena, así el mundo,
/// los pools, la cámara y el jugador vuelven siempre a su estado inicial.
/// Colócalo en un GameObject de la escena (por ejemplo el GameManager).
/// </summary>
public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    [SerializeField] private LaneGenerator laneGenerator;

    private const string BestScoreKey = "BestScore";

    public int Score { get; private set; }
    public int BestScore { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }

    public event Action<int> OnScoreChanged;
    public event Action<bool> OnPauseChanged;
    /// <summary>Parámetros: puntuación, récord, si es un récord nuevo.</summary>
    public event Action<int, int, bool> OnGameOver;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
    }

    private void OnEnable()
    {
        if (laneGenerator != null) laneGenerator.OnProgressChanged += HandleProgress;
    }

    private void OnDisable()
    {
        if (laneGenerator != null) laneGenerator.OnProgressChanged -= HandleProgress;
    }

    private void Update()
    {
        if (Keyboard.current == null) return;
        if (IsGameOver && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Restart();
        }

        // Pausar / reanudar con Esc o P
        if (!IsGameOver &&
            (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame))
        {
            TogglePause();
        }
    }

    private void HandleProgress(int progress)
    {
        if (IsGameOver) return;

        Score = progress;
        OnScoreChanged?.Invoke(Score);
    }
    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        bool isNewBest = Score > BestScore;
        if (isNewBest)
        {
            BestScore = Score;
            PlayerPrefs.SetInt(BestScoreKey, BestScore);
            PlayerPrefs.Save();
        }

        OnGameOver?.Invoke(Score, BestScore, isNewBest);
    }
    public void SetPaused(bool paused)
    {
        if (IsGameOver || paused == IsPaused) return;

        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        OnPauseChanged?.Invoke(IsPaused);
    }

    public void TogglePause()
    {
        SetPaused(!IsPaused);
    }

    /// <summary>Empieza una partida nueva recargando la escena actual.</summary>
    public void Restart()
    {
        Time.timeScale = 1f; 
        Scene scene = SceneManager.GetActiveScene();

        if (scene.buildIndex < 0)
        {
            Debug.LogError("La escena no está en la lista de escenas de la build. " +
                           "Añádela en File > Build Profiles > Scene List para poder reiniciarla.");
            return;
        }

        SceneManager.LoadScene(scene.buildIndex);
    }
}