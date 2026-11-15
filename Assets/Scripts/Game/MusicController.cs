using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BallDropper dropper;
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource specialSource;
    [SerializeField] private float fadeTime = 0.4f;
    [SerializeField, Range(0f, 1f)] private float runningVolume = 0f;

    private float baseVolume;
    private float specialBase;
    private float musicTarget;
    private float specialTarget;

    private void Awake()
    {
        baseVolume = music != null ? music.volume : 1f;
        specialBase = specialSource.volume > 0f ? specialSource.volume : 1f;
        musicTarget = baseVolume;

        specialSource.volume = 0f;
        specialSource.playOnAwake = false;
        specialSource.loop = true;
    }

    private void OnEnable() => gameManager.StateChanged += OnState;

    private void OnDisable()
    {
        gameManager.StateChanged -= OnState;
        CookwareSound.SetMuted(false);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetEggMode(false);
            AudioManager.Instance.SetMusicSilenced(false);
        }
    }

    private void OnState(GameState s)
    {
        var am = AudioManager.Instance;

        if (s == GameState.Running)
        {
            if (am != null) am.SetMusicSilenced(true);

            var data = dropper.CurrentData;
            bool egg = data != null && data.specialSong != null;

            if (egg)
            {
                musicTarget = 0f;
                specialTarget = specialBase;
                CookwareSound.SetMuted(true);
                if (am != null) am.SetEggMode(true);

                if (!(specialSource.isPlaying && specialSource.clip == data.specialSong))
                {
                    specialSource.clip = data.specialSong;
                    specialSource.volume = 0f;
                    specialSource.Play();
                }
            }
            else
            {
                musicTarget = runningVolume * baseVolume;
                if (am != null) am.SetEggMode(false);
            }
        }
        else if (s == GameState.Building || s == GameState.Countdown)
        {
            musicTarget = baseVolume;
            specialTarget = 0f;
            CookwareSound.SetMuted(false);
            if (am != null)
            {
                am.SetEggMode(false);
                am.SetMusicSilenced(false);
            }
        }
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;
        if (music != null)
            music.volume = Mathf.MoveTowards(music.volume, musicTarget, baseVolume / fadeTime * dt);

        specialSource.volume = Mathf.MoveTowards(
            specialSource.volume, specialTarget, Mathf.Max(specialBase, 0.01f) / fadeTime * dt);

        if (specialTarget == 0f && specialSource.volume <= 0f && specialSource.isPlaying)
            specialSource.Stop();
    }
}