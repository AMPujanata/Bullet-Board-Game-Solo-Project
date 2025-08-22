using UnityEngine;

[System.Serializable]
public class ShieldData
{
    public int ShieldIntensity;
    public int NextShieldBreak;
    public BaseBossEffect OnShieldBreak;
}

[CreateAssetMenu(fileName = "NewBossData", menuName = "Boss Data", order = 1)]
[System.Serializable]
public class BossData : ScriptableObject
{
    public string BossName;
    public BaseBossPassive Passive;
    public ShieldData[] Shields;
    public BossPatternCardData[] Patterns;
}
