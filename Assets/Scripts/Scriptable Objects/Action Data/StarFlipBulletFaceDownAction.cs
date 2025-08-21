using UnityEngine;

[CreateAssetMenu(fileName = "StarFlipBulletFaceDownActionSO", menuName = "BaseAction/StarFlipBulletFaceDown")]
public class StarFlipBulletFaceDownAction : BaseAction
{
    public override void OnActivated(System.Action<bool> callback)
    {
        Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.8f, 0.5f));
        PopupManager.Instance.DisplayPopup("Choose a bullet to flip face-down.", popupLocation, "Cancel", GameManager.Instance.ActivePlayer.SightController.CancelSpaceSelection);

        PatternSpaceData anyBullet = new PatternSpaceData()
        {
            NeedsBullet = true,
            NeedsFaceUp = true
        };

        PatternSpaceData[,] patternData = new PatternSpaceData[1, 1] { { anyBullet } };

        GameManager.Instance.ActivePlayer.SightController.CheckValidSpacesOnHover(patternData, (bool isSuccessfulSelect, Vector2Int bulletCell) =>
        {
            if (!isSuccessfulSelect)
            {
                callback.Invoke(false);
                return;
            }

            PopupManager.Instance.ClosePopup();

            GameManager.Instance.ActivePlayer.SightController.FlipBulletFaceDown(bulletCell);
            callback.Invoke(true);
        });
    }
}
