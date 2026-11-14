using System;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { Building, Running, Paused }

/// <summary>
/// Coordinador de la escena Game. NO tiene fisica ni sonido:
/// solo conoce el estado, bloquea la edicion al soltar la bola y conecta botones.
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private BallDropper dropper;
    [SerializeField] private Transform placedRoot;   // padre de todo el cookware colocado
    [SerializeField] private FadePanel pausePanel;   // opcional

    [Header("Bloqueo de edicion (al soltar la bola)")]
    [SerializeField] private Selectable[] editControls; // botones que abren paneles, Clear, etc.
    [SerializeField] private SlidePanel[] editPanels;   // paneles que se cierran al bloquear
    [SerializeField] private Selectable dropButton;     // solo activo en Building

    public GameState State { get; private set; }
    public Transform PlacedRoot => placedRoot;
    public event Action<GameState> StateChanged;

    // true si se puede colocar/borrar cookware y elegir cosas
    public bool IsEditing =>
        State == GameState.Building ||
        (State == GameState.Paused && stateBeforePause == GameState.Building);

    private GameState stateBeforePause;

    private void OnEnable()  => dropper.Respawned += OnBallRespawned;
    private void OnDisable() => dropper.Respawned -= OnBallRespawned;

    private void Start() => SetState(GameState.Building);

    // Cada vez que aparece una bola (cambio de bola, Reset, kill zone) volvemos a Building
    private void OnBallRespawned() => SetState(GameState.Building);

    // --- Botones de la UI ---

    public void Drop()
    {
        if (State != GameState.Building) return;
        dropper.Drop();
        SetState(GameState.Running);
    }

    public void ResetBall() => dropper.Respawn();

    public void ClearCookware()
    {
        if (!IsEditing) return;
        for (int i = placedRoot.childCount - 1; i >= 0; i--)
            Destroy(placedRoot.GetChild(i).gameObject);
    }

    public void TogglePause()
    {
        if (State == GameState.Paused)
        {
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.Close();
            SetState(stateBeforePause);
        }
        else
        {
            stateBeforePause = State;
            Time.timeScale = 0f;
            if (pausePanel != null) pausePanel.Open();
            SetState(GameState.Paused);
        }
    }

    private void SetState(GameState s)
    {
        State = s;
        ApplyEditLock();
        StateChanged?.Invoke(s);
    }

    private void ApplyEditLock()
    {
        bool editing = IsEditing;

        foreach (var c in editControls)
            if (c != null) c.interactable = editing;

        if (dropButton != null) dropButton.interactable = State == GameState.Building;

        if (!editing)
            foreach (var p in editPanels)
                if (p != null) p.Close();
    }

    // Por si sales de la escena estando en pausa
    private void OnDestroy() => Time.timeScale = 1f;
}