using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveBulletSideOrDownActionSO", menuName = "BaseAction/MoveBulletSideOrDown")]
public class MoveBulletSideOrDownAction : BaseAction
{
    public override void OnActivated()
    {
        if (ActionCost == 0)
        {
            Debug.LogError("Action cost is 0! This will cause a problem when trying to make the radius later. Please change the value!");
            return;
        }

        int currentAP = GameManager.Instance.ActivePlayer.ActionController.CurrentAP;
        if (currentAP < ActionCost)
        {
            Vector2 warningPopupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
            PopupManager.Instance.DisplayPopup("Not enough AP to use action!", "OK", warningPopupLocation);
            return;
        }

        Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.8f, 0.5f));
        PopupManager.Instance.DisplayPopup("Choose a bullet to move.", "Cancel", popupLocation, GameManager.Instance.ActivePlayer.CurrentController.CancelSpaceSelection);

        PatternSpaceData anyBullet = new PatternSpaceData { NeedsBullet = true };
        PatternSpaceData[,] patternData = new PatternSpaceData[1, 1] { { anyBullet } };

        GameManager.Instance.ActivePlayer.CurrentController.CheckValidSpacesOnHover(patternData, (bool isSuccessfulSelect, Vector2Int bulletCell) =>
        {
            if (!isSuccessfulSelect) return;
            PopupManager.Instance.ClosePopup();
            PopupManager.Instance.DisplayPopup("Choose the space to move into.", "Cancel", popupLocation, GameManager.Instance.ActivePlayer.CurrentController.CancelSpaceSelection);
            
            Direction[] allowedDirections = { Direction.Down, Direction.Left, Direction.Right };
            GameManager.Instance.ActivePlayer.CurrentController.CheckSpacesToMoveIntoOrthogonal(bulletCell, Mathf.FloorToInt(currentAP / ActionCost), allowedDirections, (bool isSuccessfulMove, Vector2Int finalCell, int distance) =>
            {
                if (!isSuccessfulMove) return;
                PopupManager.Instance.ClosePopup();

                GameManager.Instance.ActivePlayer.ActionController.ModifyCurrentAP(-distance * ActionCost);
                GameManager.Instance.ActivePlayer.CurrentController.MoveBulletInCurrent(bulletCell, finalCell);
            });
        });
    }
}
