using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BossView))]
public class BossController : MonoBehaviour
{
    [SerializeField] private BossView _bossView;
    private BossData _bossData;
    public int BrokenShieldsCount { get; private set; }

    private List<BulletData> _bulletsInBossIncoming = new List<BulletData>();
    public void Initialize(BossData bossData)
    {
        _bossData = bossData;
        // shuffle patterns
        // perform boss passive setup
        _bossView.Initialize(_bossData, SwapToPlayerPanel);

        BrokenShieldsCount = 0;
        _bossView.SetNewActiveShield(BrokenShieldsCount);
        _bossView.UpdateBulletIncomingText(_bulletsInBossIncoming.Count);
    }

    private void SwapToPlayerPanel()
    {
        _bossView.gameObject.SetActive(false);
        GameManager.Instance.ActivePlayer.gameObject.SetActive(true);
    }

    public void AddBulletToBossIncoming(BulletData data)
    {
        _bulletsInBossIncoming.Add(data);
        _bossView.UpdateBulletIncomingText(_bulletsInBossIncoming.Count);
    }

    public void CheckShieldBreak()
    {
        bool checkMoreShields = true;
        bool shieldBroken = false;
        int bulletsAlreadyUsedCount = 0;

        while (checkMoreShields)
        {
            if (BrokenShieldsCount + 1 >= _bossData.Shields.Length) break; // no more shields to check
            ShieldData activeShield = _bossData.Shields[BrokenShieldsCount];
            if (_bulletsInBossIncoming.Count >= activeShield.NextShieldBreak + bulletsAlreadyUsedCount)
            {
                BrokenShieldsCount++;
                BaseBossEffect nextOnShieldBreak = _bossView.GetAllShieldSpaces()[BrokenShieldsCount].ShieldProperties.OnShieldBreak;
                if (nextOnShieldBreak != null) nextOnShieldBreak.ActivateEffect();
                shieldBroken = true;
                bulletsAlreadyUsedCount += activeShield.NextShieldBreak;
            }
            else
            {
                checkMoreShields = false;
            }
        }

        if (shieldBroken) // only clear incoming if a shield was broken
        {
            _bossView.SetNewActiveShield(BrokenShieldsCount);
            foreach (BulletData data in _bulletsInBossIncoming)
            {
                CenterManager.Instance.AddBulletToCenter(data);
            }
            _bulletsInBossIncoming.Clear();
            _bossView.UpdateBulletIncomingText(_bulletsInBossIncoming.Count);
        }

        if (BrokenShieldsCount + 1 >= _bossData.Shields.Length) // boss is defeated
        {
            GameManager.Instance.TriggerVictory();
        }
    }

    public int GetCurrentBossIntensity()
    {
        return _bossData.Shields[BrokenShieldsCount].ShieldIntensity;
    }
}
