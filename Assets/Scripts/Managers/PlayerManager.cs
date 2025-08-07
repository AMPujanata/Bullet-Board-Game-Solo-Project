using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CurrentManager currentManager;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private PlayerView playerView;
    [SerializeField] private ActionManager actionManager;

    private int currentHP;
    public int CurrentHP { get; private set; }
    private int currentAP;
    public int CurrentAP { get; private set; }


    private void Start()
    {
        currentHP = playerData.MaxHP;
        currentAP = playerData.MaxAP;
        playerView.Initialize(playerData);
        actionManager.Initialize(playerData.MaxAP, playerData.Actions);
    }

    public void ModifyCurrentHP(int value) // increases or decreases current HP by the value's amount
    {
        currentHP += value;
        Mathf.Clamp(currentHP, 0, playerData.MaxHP);
        playerView.ChangeHPValue(currentHP);
    }

    public void ModifyCurrentAP(int value) // increases or decreases current HP by the value's amount
    {
        currentAP += value;
        Mathf.Clamp(currentAP, 0, playerData.MaxAP);
        actionManager.ChangeAPValue(currentAP);
    }
}
