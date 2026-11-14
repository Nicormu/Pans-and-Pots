using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Hover (mouse encima)")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float scaleSpeed = 12f;

    [Header("Flotar (idle)")]
    [SerializeField] private bool floatIdle = false;
    [SerializeField] private float floatAmplitude = 6f;   // píxeles del Canvas
    [SerializeField] private float floatSpeed = 2f;

    private RectTransform rect;
    private Vector3 baseScale;
    private Vector2 basePosition;
    private bool hovered;
    private float phaseOffset;

    private void Awake()
    {
        rect = (RectTransform)transform;
        baseScale = rect.localScale;
        basePosition = rect.anchoredPosition;
        phaseOffset = Random.value * Mathf.PI * 2f; // cada botón flota desfasado
    }

    private void OnEnable()
    {
        hovered = false;
        rect.localScale = baseScale;
    }

    private void Update()
    {
        // Escala suave hacia el objetivo
        Vector3 targetScale = baseScale * (hovered ? hoverScale : 1f);
        rect.localScale = Vector3.Lerp(rect.localScale, targetScale, scaleSpeed * Time.unscaledDeltaTime);

        if (floatIdle)
        {
            float offset = Mathf.Sin(Time.unscaledTime * floatSpeed + phaseOffset) * floatAmplitude;
            rect.anchoredPosition = basePosition + Vector2.up * offset;
        }
    }

    public void OnPointerEnter(PointerEventData e) => hovered = true;
    public void OnPointerExit(PointerEventData e) => hovered = false;
    public void OnSelect(BaseEventData e) => hovered = true;
    public void OnDeselect(BaseEventData e) => hovered = false;
}