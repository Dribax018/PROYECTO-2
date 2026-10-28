
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra el panel de pausa y conecta sus botones con GameSession.
/// Ponlo en el Canvas (UI), NO en el panel de pausa: si estuviera en el panel,
/// al desactivarlo dejaría de escuchar los cambios de pausa.
/// </summary>
public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameSession session;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [Tooltip("Opcional: reinicia la partida desde la pausa")]
    [SerializeField] private Button restartButton;

    private void OnEnable()
    {
        session.OnPauseChanged += ShowPause;
        resumeButton.onClick.AddListener(Resume);
        if (restartButton != null) restartButton.onClick.AddListener(session.Restart);
    }

    private void OnDisable()
    {
        session.OnPauseChanged -= ShowPause;
        resumeButton.onClick.RemoveListener(Resume);
        if (restartButton != null) restartButton.onClick.RemoveListener(session.Restart);
    }

    private void Start()
    {
        pausePanel.SetActive(false);
    }

    private void Resume()
    {
        session.SetPaused(false);
    }

    private void ShowPause(bool paused)
    {
        pausePanel.SetActive(paused);
    }
}
