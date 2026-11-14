using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class BallBody : MonoBehaviour
{
    public void Apply(BallData d)
    {
        var rb = GetComponent<Rigidbody2D>();
        rb.mass = d.mass;
        rb.gravityScale = d.gravityScale;
        rb.linearDamping = d.linearDrag; 
        rb.angularDamping = d.angularDrag; 
        GetComponent<CircleCollider2D>().sharedMaterial = d.material;
    }
}