using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Va en la Main Camera.
/// Building: la camara se mueve libre (WASD/flechas, clic central arrastrando, scroll = zoom).
/// Running:  la camara sigue a la bola.
/// Al volver a Building regresa suavemente al punto de caida.
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BallDropper dropper;
    [SerializeField] private CookwarePlacer placer;
    [SerializeField] private Camera cam;

    [Header("Seguir bola")]
    [SerializeField] private float followSmooth = 0.15f;

    [Header("Mover (Building)")]
    [SerializeField] private float panSpeed = 8f;

    [Header("Zoom (scroll, solo si no estas colocando una pieza)")]
    [SerializeField] private float zoomStep = 0.5f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 12f;

    [Header("Limites del mundo")]
    [SerializeField] private Rect limits = new Rect(-40f, -30f, 80f, 60f);

    private Vector3 velocity;
    private Vector3 dragOrigin;
    private bool dragging;
    private bool returning;
    private bool wasFollowing;

    private void Awake()
    {
        if (cam == null) cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        var state = gameManager.State;

        if (state == GameState.Running)
        {
            dragging = false;
            wasFollowing = true;
            Follow();
            return;
        }

        if (state != GameState.Building) return; // en pausa se queda quieta

        if (wasFollowing)
        {
            wasFollowing = false;
            returning = true;
            velocity = Vector3.zero;
        }

        bool moved = Pan();
        Zoom();
        if (moved) returning = false;
        if (returning) ReturnToDrop();
        Clamp();
    }

    private void Follow()
    {
        var ball = dropper.CurrentBall;
        if (ball == null) return;

        Vector3 target = new Vector3(ball.position.x, ball.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, followSmooth);
        Clamp();
    }

    private void ReturnToDrop()
    {
        Vector3 target = dropper.DropPosition;
        target.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, followSmooth * 2f);
        if ((transform.position - target).sqrMagnitude < 0.01f) returning = false;
    }

    private bool Pan()
    {
        bool moved = false;
        var kb = Keyboard.current;
        var mouse = Mouse.current;

        if (kb != null)
        {
            Vector2 dir = Vector2.zero;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  dir.x -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) dir.x += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  dir.y -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    dir.y += 1f;

            if (dir != Vector2.zero)
            {
                float speed = panSpeed * (cam.orthographicSize / 5f); // mas rapido con mas zoom out
                transform.position += (Vector3)(dir.normalized * speed * Time.unscaledDeltaTime);
                moved = true;
            }
        }

        if (mouse != null)
        {
            if (mouse.middleButton.wasPressedThisFrame)
            {
                dragOrigin = cam.ScreenToWorldPoint(mouse.position.ReadValue());
                dragging = true;
            }

            if (dragging && mouse.middleButton.isPressed)
            {
                Vector3 now = cam.ScreenToWorldPoint(mouse.position.ReadValue());
                Vector3 diff = dragOrigin - now;
                if (diff.sqrMagnitude > 0f)
                {
                    transform.position += diff;
                    moved = true;
                }
            }

            if (mouse.middleButton.wasReleasedThisFrame) dragging = false;
        }

        return moved;
    }

    private void Zoom()
    {
        if (placer != null && placer.IsPlacing) return; // con pieza en mano, el scroll la gira
        var mouse = Mouse.current;
        if (mouse == null) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        float scroll = mouse.scroll.ReadValue().y;
        if (scroll == 0f) return;

        cam.orthographicSize = Mathf.Clamp(
            cam.orthographicSize - Mathf.Sign(scroll) * zoomStep, minZoom, maxZoom);
    }

    private void Clamp()
    {
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, limits.xMin, limits.xMax);
        p.y = Mathf.Clamp(p.y, limits.yMin, limits.yMax);
        transform.position = p;
    }
}