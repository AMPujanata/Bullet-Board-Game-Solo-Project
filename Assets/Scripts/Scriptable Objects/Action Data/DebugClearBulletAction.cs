using UnityEngine;

[CreateAssetMenu(fileName = "DebugClearBulletActionSO", menuName = "BaseAction/ClearBullet")]
public class DebugClearBulletAction : BaseAction
{
    public override void OnActivated()
    {
        Debug.Log("Action cost: " + ActionCost);
    }
}
