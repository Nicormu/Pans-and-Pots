using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class BallBody : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // si esta vacio, lo busca

    public void Apply(BallData d)
    {
        // Sprite + collider que coincide con el sprite
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        var circle = GetComponent<CircleCollider2D>();
        if (spriteRenderer != null && d.WorldSprite != null)
        {
            spriteRenderer.sprite = d.WorldSprite;
            circle.radius = d.WorldSprite.bounds.extents.x; // el tamano lo define el sprite (Pixels Per Unit)
        }

        // Fisica
        var rb = GetComponent<Rigidbody2D>();
        rb.mass = d.mass;
        rb.gravityScale = d.gravityScale;
        rb.linearDamping = d.linearDrag;    // Unity 6. In older versions: rb.drag
        rb.angularDamping = d.angularDrag;  // Unity 6. In older versions: rb.angularDrag
        circle.sharedMaterial = d.material;
    }
}