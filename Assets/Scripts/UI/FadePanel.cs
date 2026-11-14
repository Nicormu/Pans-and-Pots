using System.Collections;
using UnityEngine;

/// <summary>
/// Abre/cierra un panel con fade in / fade out :D
/// </summary>

[RequireComponent(typeof(CanvasGroup))]
public class FadePanel : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private bool startOpen = false;

    private CanvasGroup canvasGroup;
    private Coroutine currentRoutine;
    private bool isOpen;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        isOpen = startOpen;
        SetState(startOpen);
    }

    public void Open()
    {
        isOpen = true;
        StartFade(1f);
    }

    public void Close()
    {
        isOpen = false;
        StartFade(0f);
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }

    private void StartFade(float target)
    {
        if (currentRoutine != null) StopCoroutine(currentRoutine);

        if (!gameObject.activeSelf) gameObject.SetActive(true);

        currentRoutine = StartCoroutine(Fade(target));
    }

    private IEnumerator Fade(float target)
    {
        float start = canvasGroup.alpha;
        float t = 0f;

        // Mientras se anima, bloquea clics
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
        bool visible = target > 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
        currentRoutine = null;
    }

    private void SetState(bool open)
    {
        canvasGroup.alpha = open ? 1f : 0f;
        canvasGroup.interactable = open;
        canvasGroup.blocksRaycasts = open;
    }
}