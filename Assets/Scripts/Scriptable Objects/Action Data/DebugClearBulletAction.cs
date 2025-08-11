using UnityEngine;

[CreateAssetMenu(fileName = "DebugClearBulletActionSO", menuName = "BaseAction/ClearBullet")]
public class DebugClearBulletAction : BaseAction
{
    public override void OnActivated()
    {
        Debug.Log("Action cost: " + ActionCost);
        Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.8f, 0.5f));
        PopupManager.Instance.DisplayPopup("Choose a bullet to clear.", "Cancel", popupLocation, 0, GameManager.Instance.ActivePlayer.CurrentController.CancelCheckValidSpacesOnHover);
        PatternSpaceData anyBullet = new PatternSpaceData { NeedsBullet = true, WillClearBullet = true };
        PatternSpaceData[,] patternData = new PatternSpaceData[1, 1] { { anyBullet } };
        GameManager.Instance.ActivePlayer.CurrentController.CheckValidSpacesOnHover(patternData,  (bool isSuccessful, Vector2Int finalCell)=>
        {
            if (!isSuccessful) return;
            PopupManager.Instance.ClosePopup();
            GameManager.Instance.ActivePlayer.CurrentController.RemoveBulletFromCurrent(finalCell);
            GameManager.Instance.ActivePlayer.ActionController.ModifyCurrentAP(-ActionCost);
        });
    }
}
