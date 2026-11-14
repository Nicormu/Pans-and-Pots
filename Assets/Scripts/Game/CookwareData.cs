using UnityEngine;

public enum CookwareType { Pot, Pan }

[CreateAssetMenu(menuName = "PansAndPots/Cookware", fileName = "NewCookware")]
public class CookwareData : SelectableData
{
    public CookwareType type;

    [Header("Forma")]
    public float scale = 1f; // multiplica el tamano del prefab (sprite + colliders)

    [Header("Fisica (rebote / friccion propios)")]
    public PhysicsMaterial2D material;

    [Header("Sonido propio")]
    public AudioClip[] clips;
    public Vector2 pitchRange = new Vector2(0.95f, 1.05f);
    public float minImpact = 0.5f;
    public float maxImpact = 10f;
}