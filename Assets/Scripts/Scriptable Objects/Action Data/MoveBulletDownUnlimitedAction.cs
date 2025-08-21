using UnityEngine;

[CreateAssetMenu(fileName = "MoveBulletDownUnlimitedActionSO", menuName = "BaseAction/MoveBulletDownUnlimited")]
public class MoveBulletDownUnlimitedAction : BaseAction
{
    public override void OnActivated(System.Action<bool> callback)
    {
        int currentAP = GameManager.Instance.ActivePlayer.ActionController.CurrentAP;
        if (currentAP < ActionCost)
        {
            Vector2 warningPopupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
            PopupManager.Instance.DisplayPopup("Not enough AP to use action!", warningPopupLocation, "OK");
            callback.Invoke(false);
            return;
        }

        Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.8f, 0.5f));
        PopupManager.Instance.DisplayPopup("Choose a bullet to move.",  popupLocation, "Cancel", GameManager.Instance.ActivePlayer.SightController.CancelSpaceSelection);

        PatternSpaceData anyBullet = new PatternSpaceData()
        {
            NeedsBullet = true
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
            PopupManager.Instance.DisplayPopup("Choose the space to move into.", popupLocation, "Cancel", GameManager.Instance.ActivePlayer.SightController.CancelSpaceSelection);

            Direction[] allowedDirections = { Direction.Down };
            GameManager.Instance.ActivePlayer.SightController.CheckSpacesToMoveIntoOrthogonal(bulletCell, 7, allowedDirections, (bool isSuccessfulMove, Vector2Int finalCell, int distance) =>
            {
                if (!isSuccessfulMove)
                {
                    callback.Invoke(false);
                    return;
                }
                PopupManager.Instance.ClosePopup();

                GameManager.Instance.ActivePlayer.ActionController.ModifyCurrentAP(-ActionCost);
                GameManager.Instance.ActivePlayer.SightController.MoveBulletInSight(bulletCell, finalCell);
                callback.Invoke(true);
            });
        });
    }
}
