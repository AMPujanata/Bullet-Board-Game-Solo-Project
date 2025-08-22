using UnityEngine;

[CreateAssetMenu(fileName = "DebugBossPassiveSO", menuName = "BaseBossPassive/DebugPassive")]
public class DEBUGBossPassive : BaseBossPassive
{
    public override void SetupPassive()
    {
        Debug.Log("This is a blank effect.");
    }
}
