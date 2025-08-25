using UnityEngine;

[CreateAssetMenu(fileName = "BossMultipleActivePatternsPassiveSO", menuName = "BaseBossPassive/BossMultipleActivePatterns")]
public class BossMultipleActivePatternsPassive : BaseBossPassive
{
    public override void SetupPassive()
    {
        // This passive doesn't do anything by itself. All logic is handled by the actual boss effects this boss comes with.
    }
}
