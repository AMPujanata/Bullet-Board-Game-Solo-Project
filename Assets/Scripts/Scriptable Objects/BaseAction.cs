using UnityEngine;

public abstract class BaseAction : ScriptableObject
{
    public int ActionCost;
    public string ActionText;
    public Sprite ActionIcon;
    public bool IsStar;

    public abstract void OnActivated();
}
