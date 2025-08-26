using UnityEngine;

[CreateAssetMenu(fileName = "FaceDownBulletsDamageEffectSO", menuName = "BaseBossEffect/FaceDownBulletsDamage")]
public class FaceDownBulletsDamageEffect : BaseBossEffect
{
    public override void ActivateEffect()
    {
        SightSpace[,] sightGrid = GameManager.Instance.ActivePlayer.SightController.GetSightGrid();
        int totalRows = sightGrid.GetLength(0);
        int totalColumns = sightGrid.GetLength(1);

        for (int x = 0; x < totalRows; x++)
        {
            for (int y = 0; y < totalColumns; y++)
            {
                if (sightGrid[x, y].BulletProperties == null) continue; // no point checking if a bullet isn't there
                if (sightGrid[x, y].BulletProperties.IsFacedown == true)
                {
                    GameManager.Instance.ActivePlayer.SightController.RemoveBulletFromSight(new Vector2Int(x, y), true);
                }
            }
        }
    }
}
