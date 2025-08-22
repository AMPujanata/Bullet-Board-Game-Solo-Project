using UnityEngine;

[RequireComponent(typeof(BossView))]
public class BossController : MonoBehaviour
{
    [SerializeField] private BossView _bossView;
    private BossData _bossData;
    public void Initialize(BossData bossData)
    {
        _bossData = bossData;
        // shuffle patterns
        // place shields
        // perform boss passive setup
        _bossView.Initialize(_bossData);
    }

}
