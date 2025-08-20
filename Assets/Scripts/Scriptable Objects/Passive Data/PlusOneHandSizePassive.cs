using UnityEngine;

[CreateAssetMenu(fileName = "PlusOneHandSizePassiveSO", menuName = "BasePassive/PlusOneHandSize")]
public class PlusOneHandSizePassive : BasePassive
{
    public override void SetupPassive()
    {
        GameManager.Instance.ActivePlayer.PatternController.SetMaxHandSize(4);
    }
}
