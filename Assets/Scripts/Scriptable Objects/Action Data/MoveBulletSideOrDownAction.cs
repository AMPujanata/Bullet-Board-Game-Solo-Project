using UnityEngine;

[CreateAssetMenu(fileName = "MoveBulletSideOrDownActionSO", menuName = "BaseAction/MoveBulletSideOrDown")]
public class MoveBulletSideOrDownAction : BaseAction
{
    public override void ActivateAction(System.Action<bool> callback)
    {
        if (ActionCost == 0)
        {
            Debug.LogError("Action cost is 0! This will cause a problem when trying to make the radius later. Please change the value!");
            callback.Invoke(false);
            return;
        }

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
            
            Direction[] allowedDirections = { Direction.Down, Direction.Left, Direction.Right };
            GameManager.Instance.ActivePlayer.SightController.CheckSpacesToMoveIntoOrthogonal(bulletCell, Mathf.FloorToInt(currentAP / ActionCost), allowedDirections, (bool isSuccessfulMove, Vector2Int finalCell, int distance) =>
            {
                if (!isSuccessfulMove)
                {
                    callback.Invoke(false);
                    return;
                }
                PopupManager.Instance.ClosePopup();

                GameManager.Instance.ActivePlayer.ActionController.ModifyCurrentAP(-distance * ActionCost);
                GameManager.Instance.ActivePlayer.SightController.MoveBulletInSight(bulletCell, finalCell);
                callback.Invoke(true);
            });
        });
    }
}
