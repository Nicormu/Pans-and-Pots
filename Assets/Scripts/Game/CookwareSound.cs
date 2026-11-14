using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CookwareSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private Vector2 pitchRange = new(0.95f, 1.05f);
    [SerializeField] private float minImpact = 0.5f;   // below this, no sound
    [SerializeField] private float maxImpact = 10f;    // at or above this, full volume

    private float lastPlayTime;
    [SerializeField] private float cooldown = 0.06f;

    private AudioSource source;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // 2D game
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (clips.Length == 0) return;
        if (Time.time - lastPlayTime < cooldown) return;

        float impact = col.relativeVelocity.magnitude;
        if (impact < minImpact) return;

        // Heavier balls hit louder and lower
        float mass = col.rigidbody != null ? col.rigidbody.mass : 1f;
        float t = Mathf.InverseLerp(minImpact, maxImpact, impact * Mathf.Sqrt(mass));

        source.pitch = Random.Range(pitchRange.x, pitchRange.y) / Mathf.Clamp(mass, 0.5f, 2f);
        source.PlayOneShot(clips[Random.Range(0, clips.Length)], Mathf.Lerp(0.2f, 1f, t));
        lastPlayTime = Time.time;
    }
}