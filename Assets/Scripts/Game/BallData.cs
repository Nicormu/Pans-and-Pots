using UnityEngine;

[CreateAssetMenu(menuName = "PansAndPots/Ball", fileName = "NewBall")]
public class BallData : SelectableData
{
    [Header("Physics")]
    public float mass = 1f;
    public float linearDrag = 0.05f;
    public float angularDrag = 0.2f;
    public float gravityScale = 1f;
    public PhysicsMaterial2D material; // friction + bounciness

    [Header("Easter egg (Verity xD)")]
    public AudioClip specialSong; // si tiene clip: al soltar, suena esto y se mutean los SFX
}