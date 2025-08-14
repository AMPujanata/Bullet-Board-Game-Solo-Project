using UnityEngine;

[System.Serializable]
public class PatternRow
{
    public PatternSpaceData[] PatternSpaces;
}

[CreateAssetMenu(fileName = "NewPatternCardData", menuName = "Pattern Card Data", order = 1)]
[System.Serializable]
public class PatternCardData : ScriptableObject
{
    public string PatternName;
    public string PatternDescription;
    public string PatternOwner;
    public PatternRow[] PatternGrid;
}
