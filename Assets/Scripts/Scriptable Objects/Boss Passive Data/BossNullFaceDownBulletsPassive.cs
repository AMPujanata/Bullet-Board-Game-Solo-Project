using UnityEngine;

[CreateAssetMenu(fileName = "BossNullFaceDownBulletsPassiveSO", menuName = "BaseBossPassive/BossNullFaceDownBullets")]
public class BossNullFaceDownBulletsPassive : BaseBossPassive
{
    public override void SetupPassive()
    {
        GameManager.Instance.ActivePlayer.SightController.CheckPropertyModifiersList.Add(NullBullet);
    }

    private BulletData NullBullet(BulletData bulletToChange, PatternSpaceData spaceRequirement)
    {
        if(bulletToChange.IsFacedown == true)
        {
            bulletToChange.Color = BulletColor.Any;
            bulletToChange.Number = 0;
            bulletToChange.IsStar = false;
        }

        return bulletToChange;
    }
}
