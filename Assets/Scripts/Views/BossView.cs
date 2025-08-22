using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _passiveNameText;
    [SerializeField] private TMP_Text _passiveDescriptionText;
    [SerializeField] private TMP_Text _bossIncomingBulletText;
    [SerializeField] private Transform _bossShieldsContainer;
    [SerializeField] private Transform _bossActivePatternContainer;
    [SerializeField] private Button _swapToPlayerButton;

    [SerializeField] private GameObject _bossShieldPrefab;
    [SerializeField] private GameObject _bossPatternCardPrefab;

    private ShieldSpace[] _shieldSpaces;
    public void Initialize(BossData bossData, Action swapToPlayerAction)
    {
        _nameText.text = bossData.BossName;
        _passiveNameText.text = bossData.Passive.PassiveName;
        _passiveDescriptionText.text = bossData.Passive.PassiveDescription;
        _swapToPlayerButton.onClick.AddListener(() => swapToPlayerAction());

        // boss shields spawn here
        Debug.Log("Spawning Shields");
        _shieldSpaces = new ShieldSpace[bossData.Shields.Length];
        for(int i = 0; i < bossData.Shields.Length; i++)
        {
            bool isFinalShield = (i == bossData.Shields.Length - 1);
            GameObject newShield = Instantiate(_bossShieldPrefab, _bossShieldsContainer);
            ShieldSpace newShieldSpace = newShield.GetComponent<ShieldSpace>();
            newShieldSpace.Initialize(bossData.Shields[i], isFinalShield);
            _shieldSpaces[i] = newShieldSpace;
        }

        // boss active pattern can be spawned later
    }

    public void UpdateBulletIncomingText(int newCount)
    {
        _bossIncomingBulletText.text = "Incoming Bullets: " + newCount + " bullets";
    }

    public void SetNewActiveShield(int activeShieldIndex)
    {
        for(int i = 0; i < _shieldSpaces.Length; i++)
        {
            if(i < activeShieldIndex) // shield is already cleared and irrelevant
            {
                _shieldSpaces[i].SetAsClearedInactiveShield();
            }
            if(i == activeShieldIndex) // this is the current active shield
            {
                _shieldSpaces[i].SetAsCurrentActiveShield();
            }
            if(i > activeShieldIndex)
            {
                _shieldSpaces[i].SetAsUnclearedInactiveShield();
            }
        }
    }

    public ShieldSpace[] GetAllShieldSpaces()
    {
        return _shieldSpaces;
    }
}
