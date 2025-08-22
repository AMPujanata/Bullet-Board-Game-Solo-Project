using UnityEngine;

public abstract class BaseBossEffect : ScriptableObject
{
    public string EffectDescription;

    public abstract void ActivateEffect();
}
