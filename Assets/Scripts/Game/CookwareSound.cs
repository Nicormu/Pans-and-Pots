using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CookwareSound : MonoBehaviour
{
    // --- Mute global ---
    private static readonly List<CookwareSound> all = new();
    public static bool Muted { get; private set; }

    public static void SetMuted(bool muted)
    {
        Muted = muted;
        if (!muted) return;

        // Corta lo que ya esté sonando
        foreach (var s in all)
            if (s != null && s.source != null) s.source.Stop();
    }

    [SerializeField] private AudioClip[] clips;
    [SerializeField] private Vector2 pitchRange = new(0.95f, 1.05f);
    [SerializeField] private float minImpact = 0.5f;
    [SerializeField] private float maxImpact = 10f;
    [SerializeField] private float cooldown = 0.06f;

    private AudioSource source;
    private float lastPlayTime;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // 2D game
    }

    private void OnEnable()  => all.Add(this);
    private void OnDisable() => all.Remove(this);

    public void Configure(AudioClip[] newClips, Vector2 pitch, float min, float max)
    {
        if (newClips != null && newClips.Length > 0) clips = newClips;
        pitchRange = pitch;
        minImpact = min;
        maxImpact = max;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (Muted) return;
        if (clips == null || clips.Length == 0) return;
        if (Time.time - lastPlayTime < cooldown) return;

        float impact = col.relativeVelocity.magnitude;
        if (impact < minImpact) return;

        float mass = col.rigidbody != null ? col.rigidbody.mass : 1f;
        float t = Mathf.InverseLerp(minImpact, maxImpact, impact * Mathf.Sqrt(mass));

        source.pitch = Random.Range(pitchRange.x, pitchRange.y) / Mathf.Clamp(mass, 0.5f, 2f);
        source.PlayOneShot(clips[Random.Range(0, clips.Length)], Mathf.Lerp(0.2f, 1f, t));
        lastPlayTime = Time.time;
    }
}