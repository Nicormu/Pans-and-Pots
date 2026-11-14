using UnityEngine;

public enum CookwareType { Pot, Pan }

[CreateAssetMenu(menuName = "PansAndPots/Cookware", fileName = "NewCookware")]
public class CookwareData : SelectableData
{
    public CookwareType type;
}