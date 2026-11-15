using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;
    [SerializeField, Range(0f, 1f)] private float hoverVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float clickVolume = 1f;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayClick);
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (!button.interactable) return;
        Play(hoverClip, hoverVolume);
    }

    private void PlayClick() => Play(clickClip, clickVolume);

    private void Play(AudioClip clip, float volume)
    {
        if (clip != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(clip, volume);
    }
}