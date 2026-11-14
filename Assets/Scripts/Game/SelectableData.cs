using UnityEngine;

public abstract class SelectableData : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject worldPrefab;
}