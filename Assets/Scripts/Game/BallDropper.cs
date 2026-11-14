using System;
using UnityEngine;

public class BallDropper : MonoBehaviour
{
    [SerializeField] private SelectionPanel ballPanel;
    [SerializeField] private Transform dropPoint;

    public event Action Respawned;

    private BallData current;
    private GameObject ball;
    private Rigidbody2D rb;

    private void OnEnable()  => ballPanel.Selected += OnSelected;
    private void OnDisable() => ballPanel.Selected -= OnSelected;

    private void OnSelected(SelectableData data)
    {
        current = data as BallData;
        Respawn(); // choosing a different ball always resets to the drop point
    }

    // Hook to the Reset button's OnClick
    public void Respawn()
    {
        if (current == null) return;
        if (ball != null) Destroy(ball);

        ball = Instantiate(current.worldPrefab, dropPoint.position, Quaternion.identity);
        ball.GetComponent<BallBody>().Apply(current);

        rb = ball.GetComponent<Rigidbody2D>();
        rb.simulated = false; // frozen at the drop point until Drop()
        Respawned?.Invoke();
    }

    // Hook to the Drop button's OnClick
    public void Drop()
    {
        if (rb == null) return;
        rb.simulated = true;
    }
}