using UnityEngine;
using UnityEngine.InputSystem;

public class DropAimer : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BallDropper dropper;
    [SerializeField] private LineRenderer line;
    [SerializeField] private LayerMask hitMask;      

    [Header("Color")]
    [SerializeField] private Color startColor = new Color(1f, 0.92f, 0.1f, 0.9f); // amarillo
    [SerializeField] private Color endColor   = new Color(1f, 0.92f, 0.1f, 0.2f);

    [Header("Dirección")]
    [SerializeField] private float maxAngle = 80f;
    [SerializeField] private float rotateSpeed = 60f;
    [SerializeField] private float launchSpeed = 5f; 

    [Header("Predicción")]
    [SerializeField] private float previewTime = 1.2f;

    private float angle; // grados; + = derecha

    private void Awake()
    {
        line.startColor = startColor;
        line.endColor = endColor;
    }

    private void Update()
    {
        var body = dropper.CurrentBody;
        bool show = gameManager.State == GameState.Building && body != null;
        line.enabled = show;
        if (!show) return;

        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.qKey.isPressed) angle -= rotateSpeed * Time.unscaledDeltaTime;
            if (kb.eKey.isPressed) angle += rotateSpeed * Time.unscaledDeltaTime;
            angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
        }

        Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.down;
        dropper.SetLaunch(dir, launchSpeed);
        DrawPreview(body, dir * launchSpeed);
    }

    private void DrawPreview(Rigidbody2D body, Vector2 v)
    {
        var circle = body.GetComponent<CircleCollider2D>();
        Vector3 s = body.transform.lossyScale;
        float radius = circle.radius * Mathf.Max(Mathf.Abs(s.x), Mathf.Abs(s.y));

        float dt = Time.fixedDeltaTime;
        Vector2 gravity = Physics2D.gravity * body.gravityScale;
        float drag = body.linearDamping;

        Vector2 pos = body.transform.TransformPoint(circle.offset);

        var pts = new System.Collections.Generic.List<Vector3> { pos };

        for (float t = 0f; t < previewTime; t += dt)
        {
            v += gravity * dt;
            v *= 1f / (1f + dt * drag);

            Vector2 delta = v * dt;
            float dist = delta.magnitude;
            if (dist < 0.0001f) continue;

            var hit = Physics2D.CircleCast(pos, radius, delta / dist, dist, hitMask);
            if (hit.collider != null)
            {
                pts.Add(hit.centroid); 
                break;
            }

            pos += delta;
            pts.Add(pos);
        }

        line.positionCount = pts.Count;
        line.SetPositions(pts.ToArray());
    }
}