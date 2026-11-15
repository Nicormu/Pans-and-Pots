using System;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { Building, Countdown, Running, Paused }

public class GameManager : MonoBehaviour
{
    [SerializeField] private BallDropper dropper;
    [SerializeField] private Transform placedRoot;   // padre de todo el cookware colocado
    [SerializeField] private FadePanel pausePanel;   // opcional
    [SerializeField] private DropCountdown countdown; // opcional: si esta vacio, suelta al instante

    [Header("Bloqueo de edicion (al soltar la bola)")]
    [SerializeField] private Selectable[] editControls; // botones que abren paneles, Clear, etc.
    [SerializeField] private SlidePanel[] editPanels;   // paneles que se cierran al bloquear
    [SerializeField] private Selectable dropButton;     // solo activo en Building

    public GameState State { get; private set; }
    public Transform PlacedRoot => placedRoot;
    public event Action<GameState> StateChanged;

    public bool IsEditing =>
        State == GameState.Building ||
        (State == GameState.Paused && stateBeforePause == GameState.Building);

    private GameState stateBeforePause;

    private void OnEnable()  => dropper.Respawned += OnBallRespawned;
    private void OnDisable() => dropper.Respawned -= OnBallRespawned;

    private void Start() => SetState(GameState.Building);

    private void OnBallRespawned()
    {
        if (countdown != null) countdown.Cancel(); 
        SetState(GameState.Building);
    }

    // --- Botones de la UI ---

    public void Drop()
    {
        if (State != GameState.Building) return;

        if (countdown == null)
        {
            StartRun();
            return;
        }

        SetState(GameState.Countdown);
        countdown.Begin(StartRun);
    }

    private void StartRun()
    {
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

    private void OnDestroy() => Time.timeScale = 1f;
}