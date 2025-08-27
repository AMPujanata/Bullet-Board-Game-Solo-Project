using TMPro;
using UnityEngine;

public class SelectedCharacterPanelView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _passiveNameText;
    [SerializeField] private TMP_Text _passiveDescriptionText;
    [SerializeField] private Transform _patternsContainerParent;
    [SerializeField] private GameObject _patternCardPrefab;

    public void ChangeCharacter(PlayerData playerData)
    {
        _nameText.text = playerData.PlayerName;
        _descriptionText.text = playerData.PlayerDescription;
        _passiveNameText.text = "Unique Ability: " + playerData.Passive.PassiveName;
        _passiveDescriptionText.text = playerData.Passive.PassiveDescription;

        foreach (Transform child in _patternsContainerParent) // clear all leftover patterns
        {
            Destroy(child.gameObject);
        }

        foreach (PatternCardData cardData in playerData.Patterns)
        {
            GameObject cardPrefab = Instantiate(_patternCardPrefab, _patternsContainerParent);
            cardPrefab.GetComponent<PatternCard>().Initialize(cardData, true);
        }
    }
}
