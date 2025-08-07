using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Player Data", order = 1)]
[System.Serializable]
public class PlayerData : ScriptableObject
{ 
    public string PlayerName;
    public int MaxHP;
    public int MaxAP;
    public string PassiveName;
    public string PassiveDescription;
    public BaseAction[] Actions;
}
