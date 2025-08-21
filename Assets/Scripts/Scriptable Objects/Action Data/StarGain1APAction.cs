using UnityEngine;

[CreateAssetMenu(fileName = "StarGain1APActionSO", menuName = "BaseAction/StarGain1AP")]
public class StarGain1APAction : BaseAction
{
    public override void OnActivated(System.Action<bool> callback)
    {
        GameManager.Instance.ActivePlayer.ActionController.ModifyCurrentAP(1);
        callback.Invoke(true);
    }
}
