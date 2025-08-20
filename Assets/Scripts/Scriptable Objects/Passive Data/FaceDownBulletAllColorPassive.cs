using UnityEngine;

[CreateAssetMenu(fileName = "FaceDownBulletAllColorPassiveSO", menuName = "BasePassive/FaceDownBulletAllColor")]
public class FaceDownBulletAllColorPassive : BasePassive
{
    public override void SetupPassive()
    {
        // Currently facedown bullets are just handled in the space validity checker instead. If another character uses facedown bulelts in a different way, will be moved here 
    }
}
