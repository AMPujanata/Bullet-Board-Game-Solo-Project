using UnityEngine;

[CreateAssetMenu(fileName = "MoveAllBulletsDown2EffectSO", menuName = "BaseBossEffect/MoveAllBulletsDown2")]
public class MoveAllBulletsDown2Effect : BaseBossEffect
{

    public override void ActivateEffect()
    {
        int totalRows = GameManager.Instance.ActivePlayer.SightController.GetSightGrid().GetLength(0);
        int totalColumns = GameManager.Instance.ActivePlayer.SightController.GetSightGrid().GetLength(1);

        for(int x = totalRows - 1; x >= 0; x--)
        {
            for(int y = totalColumns - 1; y >= 0; y--)
            {
                Vector2Int oldCell = new Vector2Int(x, y);
                Vector2Int newCell = new Vector2Int(x + 2, y);
                GameManager.Instance.ActivePlayer.SightController.MoveBulletInSight(oldCell, newCell);
            }
        }
    }
}
