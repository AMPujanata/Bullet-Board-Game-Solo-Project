using UnityEngine;

[CreateAssetMenu(fileName = "DebugBossEffectSO", menuName = "BaseBossEffect/DebugEffect")]
public class DEBUGBossEffect : BaseBossEffect
{

    public override void ActivateEffect()
    {
        Debug.Log("This is a blank effect.");
    }
}
