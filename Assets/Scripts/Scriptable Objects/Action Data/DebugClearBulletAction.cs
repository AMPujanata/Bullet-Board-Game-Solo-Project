using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebugClearBulletActionSO", menuName = "BaseAction/ClearBullet")]
public class DebugClearBulletAction : BaseAction
{
    public override void OnActivated()
    {
        Debug.Log("Action cost: " + ActionCost);
        throw new System.NotImplementedException();
    }
}
