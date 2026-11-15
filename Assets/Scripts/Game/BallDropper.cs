using System;
using UnityEngine;

public class BallDropper : MonoBehaviour
{
    [SerializeField] private SelectionPanel ballPanel;
    [SerializeField] private Transform dropPoint;

    public event Action Respawned;

    private BallData current;
    public BallData CurrentData => current;
    private GameObject ball;
    private Rigidbody2D rb;

    private Vector2 launchDir = Vector2.down;
    private float launchSpeed = 0f;

    public Transform CurrentBall => ball != null ? ball.transform : null;
    public Rigidbody2D CurrentBody => rb;
    public Vector3 DropPosition => dropPoint.position;

    private void OnEnable()  => ballPanel.Selected += OnSelected;
    private void OnDisable() => ballPanel.Selected -= OnSelected;

    private void OnSelected(SelectableData data)
    {
        current = data as BallData;
        Respawn(); 
    }

    public void SetLaunch(Vector2 dir, float speed)
    {
        launchDir = dir.normalized;
        launchSpeed = speed;
    }

    public void Respawn()
    {
        if (current == null) return;
        if (ball != null) Destroy(ball);

        ball = Instantiate(current.worldPrefab, dropPoint.position, Quaternion.identity);
        ball.GetComponent<BallBody>().Apply(current);

        rb = ball.GetComponent<Rigidbody2D>();
        rb.simulated = false; 
        Respawned?.Invoke();
    }

    public void Drop()
    {
        if (rb == null) return;
        rb.simulated = true;
        rb.linearVelocity = launchDir * launchSpeed; 
    }
}