using System.Collections;
using UnityEngine;

/// <summary>
/// Mueve un panel entre una posición cerrada y una abierta
/// Lo voy a usar para el panel lateral de las ollas/sartenes
/// Y tmb el panel de las pelotas
/// </summary>
public class SlidePanel : MonoBehaviour
{
    [Header("Panel a mover")]
    [SerializeField] private RectTransform panel;

    [Header("Posiciones (anchoredPosition)")]
    [SerializeField] private Vector2 closedPosition;
    [SerializeField] private Vector2 openPosition;

    [Header("Animación")]
    [SerializeField] private float duration = 0.35f;
    [SerializeField] private float delay = 0.2f;
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool startOpen = false;

    private Coroutine currentRoutine;
    private bool isOpen;
    private float unlockTime; // tiempo en el que se puede volver a abrir/cerrar el panel

    private bool IsLocked => Time.unscaledTime < unlockTime;

    private void Awake()
    {
        if (panel == null) panel = GetComponent<RectTransform>();
        isOpen = startOpen;
        panel.anchoredPosition = startOpen ? openPosition : closedPosition;
    }

    public void Open()
    {
        if (!isOpen && !IsLocked)
        {
            isOpen = true;
            MoveTo(openPosition);
        }
    }

    public void Close()
    {
        if (isOpen && !IsLocked)
        {
            isOpen = false;
            MoveTo(closedPosition);
        }
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }

    private void MoveTo(Vector2 target)
    {
        unlockTime = Time.unscaledTime + delay + duration;
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(Move(target));
    }

    private IEnumerator Move(Vector2 target)
    {
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);

        Vector2 start = panel.anchoredPosition;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float k = curve.Evaluate(Mathf.Clamp01(t / duration));
            panel.anchoredPosition = Vector2.LerpUnclamped(start, target, k);
            yield return null;
        }

        panel.anchoredPosition = target;
        currentRoutine = null;
    }
}