using UnityEngine;

public abstract class BaseBossPassive : ScriptableObject
{
    public string PassiveName;
    public string PassiveDescription;

    public abstract void SetupPassive();
}
