using TMPro;
using UnityEngine;

public class SelectedBossPanelView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _passiveNameText;
    [SerializeField] private TMP_Text _passiveDescriptionText;
    [SerializeField] private Transform _shieldsContainerParent;
    [SerializeField] private GameObject _shieldSpacesPrefab;
    [SerializeField] private Transform _patternsContainerParent;
    [SerializeField] private GameObject _bossPatternCardPrefab;

    public void ChangeBoss(BossData bossData)
    {
        _nameText.text = bossData.BossName;
        _descriptionText.text = bossData.BossDescription;
        _passiveNameText.text = "Unique Ability: " + bossData.Passive.PassiveName;
        _passiveDescriptionText.text = bossData.Passive.PassiveDescription;


        foreach (Transform child in _shieldsContainerParent) // clear all leftover shields
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < bossData.Shields.Length; i++)
        {
            bool isFinalShield = (i == bossData.Shields.Length - 1);
            GameObject newShield = Instantiate(_shieldSpacesPrefab, _shieldsContainerParent);
            ShieldSpace newShieldSpace = newShield.GetComponent<ShieldSpace>();
            newShieldSpace.Initialize(bossData.Shields[i], isFinalShield);
        }

        foreach (Transform child in _patternsContainerParent) // clear all leftover patterns
        {
            Destroy(child.gameObject);
        }

        foreach (BossPatternCardData cardData in bossData.Patterns)
        {
            GameObject cardPrefab = Instantiate(_bossPatternCardPrefab, _patternsContainerParent);
            cardPrefab.GetComponent<BossPatternCard>().Initialize(cardData);
        }
    }
}
