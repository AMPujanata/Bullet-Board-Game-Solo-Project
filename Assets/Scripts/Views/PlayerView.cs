using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Slider _hpBar;
    [SerializeField] private TMP_Text _hpBarText;
    [SerializeField] private TMP_Text _passiveNameText;
    [SerializeField] private TMP_Text _passiveDescriptionText;
    [SerializeField] private Button _swapToBossButton;
    [SerializeField] private Button _quitGameButton;

    public void Initialize(PlayerData playerData, Action swapToBossAction, Action quitGameAction)
    {
        _nameText.text = playerData.PlayerName;

        ChangeHPValue(playerData.MaxHP, playerData.MaxHP);

        _passiveNameText.text = playerData.Passive.PassiveName;
        _passiveDescriptionText.text = playerData.Passive.PassiveDescription;
        _swapToBossButton.onClick.AddListener(() => swapToBossAction());

        if (GameManager.Instance.CurrentMode == GameMode.ScoreAttack) _swapToBossButton.gameObject.SetActive(false);

        _quitGameButton.onClick.AddListener(() => quitGameAction());
    }

    public void ChangeHPValue(int currentHP, int maxHP)
    {
        _hpBar.value = currentHP;
        _hpBar.maxValue = maxHP;
        _hpBarText.text = currentHP + " / " + maxHP;
    }
}
