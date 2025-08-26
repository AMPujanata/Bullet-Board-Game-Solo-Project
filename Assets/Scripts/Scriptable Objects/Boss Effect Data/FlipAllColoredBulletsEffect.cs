using UnityEngine;

[CreateAssetMenu(fileName = "FlipAllColoredBulletsEffectSO", menuName = "BaseBossEffect/FlipAllColoredBullets")]
public class FlipAllColoredBulletsEffect : BaseBossEffect
{
    [SerializeField] private BulletColor colorToFlip;
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
                if(sightGrid[x, y].BulletProperties.Color == colorToFlip)
                {
                    GameManager.Instance.ActivePlayer.SightController.FlipBulletFaceDown(new Vector2Int(x, y));
                }
            }
        }
    }
}
