using UnityEngine;

[CreateAssetMenu(fileName = "DebugClearBulletActionSO", menuName = "BaseAction/ClearBullet")]
public class DebugClearBulletAction : BaseAction
{
    public override void OnActivated()
    {
        int currentAP = GameManager.Instance.ActivePlayer.ActionController.CurrentAP;
        if(currentAP < ActionCost)
        {
            Vector2 warningPopupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
            PopupManager.Instance.DisplayPopup("Not enough AP to use action!", "OK", warningPopupLocation);
            return;
        }

        Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.8f, 0.5f));
        PopupManager.Instance.DisplayPopup("Choose a bullet to clear.", "Cancel", popupLocation, GameManager.Instance.ActivePlayer.SightController.CancelSpaceSelection);

        PatternSpaceData anyBullet = new PatternSpaceData(true, false, BulletColor.Any, 0, false, false, true);
        PatternSpaceData[,] patternData = new PatternSpaceData[1, 1] { { anyBullet } };

        GameManager.Instance.ActivePlayer.SightController.CheckValidSpacesOnHover(patternData,  (bool isSuccessful, Vector2Int finalCell)=>
        {
            if (!isSuccessful) return;
            PopupManager.Instance.ClosePopup();

            GameManager.Instance.ActivePlayer.ActionController.ModifyCurrentAP(-ActionCost);
            GameManager.Instance.ActivePlayer.SightController.RemoveBulletFromSight(finalCell);
        });
    }
}
