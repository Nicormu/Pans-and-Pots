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
        baseVolume = music.volume;
        specialBase = specialSource.volume;
        musicTarget = baseVolume;

        specialSource.volume = 0f;
        specialSource.playOnAwake = false;
        specialSource.loop = true;
    }

    private void OnEnable()  => gameManager.StateChanged += OnState;

    private void OnDisable()
    {
        gameManager.StateChanged -= OnState;
        CookwareSound.Muted = false; 
    }

    private void OnState(GameState s)
    {
        if (s == GameState.Running)
        {
            var data = dropper.CurrentData;
            bool egg = data != null && data.specialSong != null;

            if (egg)
            {
                musicTarget = 0f;
                specialTarget = specialBase;
                CookwareSound.Muted = true;

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
            }
        }
        else if (s == GameState.Building || s == GameState.Countdown)
        {
            musicTarget = baseVolume;
            specialTarget = 0f;
            CookwareSound.Muted = false;
        }
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;
        music.volume = Mathf.MoveTowards(music.volume, musicTarget, baseVolume / fadeTime * dt);

        specialSource.volume = Mathf.MoveTowards(
            specialSource.volume, specialTarget, Mathf.Max(specialBase, 0.01f) / fadeTime * dt);

        
        if (specialTarget == 0f && specialSource.volume <= 0f && specialSource.isPlaying)
            specialSource.Stop();
    }
}