using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StarGain1APActionSO", menuName = "BaseAction/StarGain1AP")]
public class StarGain1APAction : BaseAction
{
    public override void OnActivated()
    {
        GameManager.Instance.ActivePlayer.ActionController.ModifyCurrentAP(1);
    }
}
