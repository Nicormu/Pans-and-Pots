using UnityEngine;

/// <summary>
/// Put on each background/foreground layer (a GameObject with a SpriteRenderer).
/// strength: 0 = moves with the world (no parallax, like the gameplay)
///           1 = glued to the camera (looks infinitely far, like a sky)
/// Negative values make foreground layers that move faster than the world.
/// </summary>
[DefaultExecutionOrder(100)] // runs after CameraController's LateUpdate
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField, Range(-0.5f, 1f)] private float strengthX = 0.5f;
    [SerializeField, Range(-0.5f, 1f)] private float strengthY = 0.5f;

    private Vector3 startPos;
    private Vector3 camStartPos;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Start()
    {
        startPos = transform.position;
        camStartPos = cam.transform.position;
    }

    private void LateUpdate()
    {
        Vector3 delta = cam.transform.position - camStartPos;
        transform.position = new Vector3(
            startPos.x + delta.x * strengthX,
            startPos.y + delta.y * strengthY,
            startPos.z);
    }
}