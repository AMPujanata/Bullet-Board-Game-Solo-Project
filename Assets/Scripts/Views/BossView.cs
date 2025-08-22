using TMPro;
using UnityEngine;

public class BossView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _passiveNameText;
    [SerializeField] private TMP_Text _passiveDescriptionText;
    [SerializeField] private Transform _bossShieldsContainer;
    [SerializeField] private Transform _bossActivePatternContainer;

    [SerializeField] private GameObject _bossShieldPrefab;
    [SerializeField] private GameObject _bossPatternCardPrefab;

    public void Initialize(BossData bossData)
    {
        _nameText.text = bossData.BossName;
        _passiveNameText.text = bossData.Passive.PassiveName;
        _passiveDescriptionText.text = bossData.Passive.PassiveDescription;
        // boss shields spawn here
        // boss active pattern can be spawned later
    }
}
