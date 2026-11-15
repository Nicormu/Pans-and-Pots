using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ponlo en cada Slider de volumen. Al aparecer toma el valor guardado
/// y despues actualiza el AudioManager al moverlo. Slider: Min 0, Max 1.
/// </summary>
[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    public enum Channel { Music, Sfx }

    [SerializeField] private Channel channel;

    private Slider slider;

    private void Awake() => slider = GetComponent<Slider>();

    private void OnEnable()
    {
        var am = AudioManager.Instance;
        if (am != null)
            slider.SetValueWithoutNotify(channel == Channel.Music ? am.MusicVolume : am.SfxVolume);

        slider.onValueChanged.AddListener(OnChanged);
    }

    private void OnDisable() => slider.onValueChanged.RemoveListener(OnChanged);

    private void OnChanged(float v)
    {
        var am = AudioManager.Instance;
        if (am == null) return;

        if (channel == Channel.Music) am.SetMusicVolume(v);
        else am.SetSfxVolume(v);
    }
}