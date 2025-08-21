using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Slider _hpBar;
    [SerializeField] private TMP_Text _hpBarText;
    [SerializeField] private TMP_Text _passiveNameText;
    [SerializeField] private TMP_Text _passiveDescriptionText;

    public void Initialize(PlayerData playerData)
    {
        _nameText.text = playerData.PlayerName;

        ChangeHPValue(playerData.MaxHP, playerData.MaxHP);

        _passiveNameText.text = playerData.Passive.PassiveName;
        _passiveDescriptionText.text = playerData.Passive.PassiveDescription;
    }

    public void ChangeHPValue(int currentHP, int maxHP)
    {
        _hpBar.value = currentHP;
        _hpBar.maxValue = maxHP;
        _hpBarText.text = currentHP + " / " + maxHP;
    }
}
