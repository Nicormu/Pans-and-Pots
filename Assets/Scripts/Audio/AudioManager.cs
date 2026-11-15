using System.Collections;
using UnityEngine;
using UnityEngine.Audio;


[DefaultExecutionOrder(-100)] // corre antes que los sliders y SceneMusic
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private string musicParam = "MusicVolume";
    [SerializeField] private string sfxParam = "SfxVolume";

    [SerializeField] private float musicFade = 1f;

    public AudioMixerGroup SfxGroup => sfxGroup;
    public float MusicVolume => musicVolume;
    public float SfxVolume => sfxVolume;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private Coroutine musicRoutine;
    private float musicVolume;
    private float sfxVolume;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.outputAudioMixerGroup = musicGroup;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.outputAudioMixerGroup = sfxGroup;

        musicVolume = PlayerPrefs.GetFloat(musicParam, 0.8f);
        sfxVolume = PlayerPrefs.GetFloat(sfxParam, 0.8f);
    }

    private void Start()
    {
        if (Instance != this) return; 
        ApplyVolume(musicParam, musicVolume);
        ApplyVolume(sfxParam, sfxVolume);
    }

    // ---------- Musica ----------
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return; // si es la misma cancion no se reinicia
        if (musicRoutine != null) StopCoroutine(musicRoutine);
        musicRoutine = StartCoroutine(CrossfadeTo(clip));
    }

    private IEnumerator CrossfadeTo(AudioClip clip)
    {
        float half = Mathf.Max(0.01f, musicFade * 0.5f);

        float startVol = musicSource.volume;
        for (float t = 0; t < half && musicSource.isPlaying; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / half);
            yield return null;
        }

        musicSource.clip = clip;
        musicSource.volume = 0f;
        musicSource.Play();

        for (float t = 0; t < half; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, 1f, t / half);
            yield return null;
        }
        musicSource.volume = 1f;
        musicRoutine = null;
    }

    // ---------- SFX (UI) ----------
    public void PlaySfx(AudioClip clip, float volume = 1f, float pitchVariation = 0.05f)
    {
        if (clip == null) return;
        sfxSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        sfxSource.PlayOneShot(clip, volume);
    }

    // ---------- Volumen (sliders: 0..1) ----------
    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        ApplyVolume(musicParam, musicVolume);
    }

    public void SetSfxVolume(float v)
    {
        sfxVolume = Mathf.Clamp01(v);
        ApplyVolume(sfxParam, sfxVolume);
    }

    private void ApplyVolume(string param, float v)
    {
        if (mixer != null) mixer.SetFloat(param, Mathf.Log10(Mathf.Max(v, 0.0001f)) * 20f);
        PlayerPrefs.SetFloat(param, v);
    }
}