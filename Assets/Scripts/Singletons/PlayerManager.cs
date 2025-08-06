using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this); // Make sure there's only ever one 
            return; // do NOT run any other code
        }
    }

    [SerializeField] private PlayerData playerData;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Slider hpBar;
    [SerializeField] private TMP_Text hpBarText;
    [SerializeField] private Slider apBar;
    [SerializeField] private TMP_Text apBarText;
    [SerializeField] private TMP_Text passiveNameText;
    [SerializeField] private TMP_Text passiveDescriptionText;

    private int currentHP;
    private int currentAP;

    private void Start()
    {
        nameText.text = playerData.PlayerName;

        currentHP = playerData.MaxHP;
        hpBar.maxValue = playerData.MaxHP;
        hpBar.value = hpBar.maxValue;
        hpBarText.text = playerData.MaxHP.ToString() + " / " + playerData.MaxHP.ToString();

        currentAP = playerData.MaxAP;
        apBar.maxValue = playerData.MaxAP;
        apBar.value = apBar.maxValue;
        apBarText.text = playerData.MaxAP.ToString() + " / " + playerData.MaxAP.ToString();

        passiveNameText.text = playerData.PassiveName;
        passiveDescriptionText.text = playerData.PassiveDescription;
    }

    public void ModifyCurrentHP(int value) // increases or decreases current HP by the value's amount
    {
        currentHP += value;
        hpBar.value = currentHP;
        hpBarText.text = currentHP.ToString() + " / " + playerData.MaxHP.ToString();
        if (currentHP > playerData.MaxHP) currentHP = playerData.MaxHP;
        if (currentHP <= 0)
        {
            Vector3 centerOfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            PopupManager.Instance.DisplayPopup("Game Over!", "OK", centerOfScreen);
            Debug.Log("Game over!");
        }
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }
}
