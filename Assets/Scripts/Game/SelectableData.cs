using UnityEngine;

public abstract class SelectableData : ScriptableObject
{
    public string id;
    public string displayName;

    [Header("Visual")]
    public Sprite icon;          // icono del boton en el panel
    public Sprite worldSprite;   // si esta vacio, usa icon
    public GameObject worldPrefab; 

    public Sprite WorldSprite => worldSprite != null ? worldSprite : icon;
}