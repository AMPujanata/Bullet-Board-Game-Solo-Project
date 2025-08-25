using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Activate1MorePatternEffectSO", menuName = "BaseBossEffect/Activate1MorePattern")]
public class Activate1MorePatternEffect : BaseBossEffect
{
    public override void ActivateEffect()
    {
        GameManager.Instance.ActiveBoss.MaxActivePatternsSize += 1;

        UnityAction reduceMaxSizeOnce = null;
        reduceMaxSizeOnce = () =>
        {
            GameManager.Instance.ActiveBoss.MaxActivePatternsSize -= 1;
            GameManager.Instance.BossPhaseStart.RemoveListener(reduceMaxSizeOnce);
        };

        GameManager.Instance.BossPhaseStart.AddListener(reduceMaxSizeOnce);
    }
}
