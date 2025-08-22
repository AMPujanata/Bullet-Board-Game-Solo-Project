using UnityEngine;

[CreateAssetMenu(fileName = "NewBossPatternCardData", menuName = "Pattern Card Data (Boss)", order = 1)]
[System.Serializable]
public class BossPatternCardData : ScriptableObject
{
    public string PatternName;
    public string PatternDescription;
    public string PatternOwner;
    public PatternRow[] PatternGrid;
    public BaseBossEffect OnFailEffect;
}
