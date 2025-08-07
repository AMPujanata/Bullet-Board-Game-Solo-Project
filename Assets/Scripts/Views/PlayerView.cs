using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Slider hpBar;
    [SerializeField] private TMP_Text hpBarText;
    [SerializeField] private TMP_Text passiveNameText;
    [SerializeField] private TMP_Text passiveDescriptionText;

    private int maxHP;
    public void Initialize(PlayerData playerData)
    {
        nameText.text = playerData.PlayerName;
        maxHP = playerData.MaxHP;

        hpBar.maxValue = maxHP;
        ChangeHPValue(maxHP);

        passiveNameText.text = playerData.PassiveName;
        passiveDescriptionText.text = playerData.PassiveDescription;
    }

    public void ChangeHPValue(int currentHP)
    {
        hpBar.value = currentHP;
        hpBarText.text = currentHP + " / " + maxHP;
        if (currentHP <= 0)
        {
            Vector3 centerOfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            PopupManager.Instance.DisplayPopup("Game Over!", "OK", centerOfScreen);
        }
    }
}
