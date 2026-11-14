using UnityEngine;

// Put on a wide BoxCollider2D (Is Trigger) below the play area.
public class BallKillZone : MonoBehaviour
{
    [SerializeField] private BallDropper dropper;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball")) dropper.Respawn();
    }
}