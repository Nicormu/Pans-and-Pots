using UnityEngine;

/// <summary>
/// Va en el prefab base de Pot/Pan. Aplica los datos del CookwareData:
/// sprite, tamano, material de fisica y sonido.
/// </summary>
public class CookwareBody : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // si esta vacio, lo busca

    public void Apply(CookwareData d)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null && d.WorldSprite != null)
            spriteRenderer.sprite = d.WorldSprite;

        transform.localScale *= d.scale;

        if (d.material != null)
        {
            foreach (var c in GetComponentsInChildren<Collider2D>())
                c.sharedMaterial = d.material;
        }

        var sound = GetComponent<CookwareSound>();
        if (sound != null) sound.Configure(d.clips, d.pitchRange, d.minImpact, d.maxImpact);
    }
}