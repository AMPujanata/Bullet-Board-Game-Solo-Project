using UnityEngine;

public abstract class BasePassive : ScriptableObject
{
    public string PassiveName;
    public string PassiveDescription;

    public abstract void SetupPassive();
}
