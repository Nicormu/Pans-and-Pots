using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CookwarePlacer : MonoBehaviour
{
    [SerializeField] private SelectionPanel cookwarePanel;
    [SerializeField] private Camera cam;
    [SerializeField] private float rotateStep = 15f;
    [SerializeField] private LayerMask blockMask; // Cookware + Ball
    [SerializeField] private GameManager gameManager; // para colgar lo colocado de PlacedRoot

    private CookwareData current;
    private GameObject ghost;
    private bool canPlace;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void OnEnable()  => cookwarePanel.Selected += OnSelected;
    private void OnDisable() => cookwarePanel.Selected -= OnSelected;

    private void OnSelected(SelectableData data)
    {
        Cancel();
        current = data as CookwareData;
        if (current == null) return;

        ghost = Instantiate(current.worldPrefab);
        foreach (var c in ghost.GetComponentsInChildren<Collider2D>()) c.enabled = false;
        foreach (var s in ghost.GetComponentsInChildren<CookwareSound>()) s.enabled = false;
        SetGhostColor(new Color(1f, 1f, 1f, 0.5f));
    }

    private void Update()
    {
        if (ghost == null) return;

        var mouse = Mouse.current;
        Vector2 world = cam.ScreenToWorldPoint(mouse.position.ReadValue());
        ghost.transform.position = world;

        float scroll = mouse.scroll.ReadValue().y;
        if (scroll != 0f)
            ghost.transform.Rotate(0, 0, Mathf.Sign(scroll) * rotateStep);

        UpdateCanPlace();

        bool overUI = EventSystem.current.IsPointerOverGameObject();
        if (mouse.leftButton.wasPressedThisFrame && !overUI && canPlace) Place();

        if (mouse.rightButton.wasPressedThisFrame ||
            Keyboard.current.escapeKey.wasPressedThisFrame) Cancel();
    }

    // Uses the sprite bounds because the ghost's colliders are disabled
    // (a disabled collider reports empty bounds).
    private void UpdateCanPlace()
    {
        var renderers = ghost.GetComponentsInChildren<SpriteRenderer>();
        if (renderers.Length == 0) { canPlace = true; return; }

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);

        canPlace = Physics2D.OverlapBox(b.center, b.size * 0.9f, 0f, blockMask) == null;
        SetGhostColor(canPlace ? new Color(1f, 1f, 1f, 0.5f) : new Color(1f, 0.3f, 0.3f, 0.5f));
    }

    private void SetGhostColor(Color c)
    {
        foreach (var r in ghost.GetComponentsInChildren<SpriteRenderer>()) r.color = c;
    }

    private void Place()
    {
        Instantiate(current.worldPrefab, ghost.transform.position, ghost.transform.rotation,
            gameManager != null ? gameManager.PlacedRoot : null);
    }

    private void Cancel()
    {
        if (ghost != null) Destroy(ghost);
        ghost = null;
        current = null;
    }
}