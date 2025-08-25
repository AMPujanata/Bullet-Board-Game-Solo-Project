using UnityEngine;

[CreateAssetMenu(fileName = "FaceDownBulletAllColorPassiveSO", menuName = "BasePassive/FaceDownBulletAllColor")]
public class FaceDownBulletAllColorPassive : BasePassive
{
    public override void SetupPassive()
    {
        GameManager.Instance.ActivePlayer.SightController.CheckPropertyModifiersList.Add(BulletIsAllColor);
    }

    private BulletData BulletIsAllColor(BulletData bulletToChange, PatternSpaceData spaceRequirement)
    {
        if (bulletToChange.IsFacedown == true)
        {
            bulletToChange.Color = spaceRequirement.ColorRequired;
            bulletToChange.Number = 0;
            bulletToChange.IsStar = false;
        }

        return bulletToChange;
    }
}
