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
    [SerializeField] private Transform _activeBossPatternContainer;
    [SerializeField] private Button _swapToPlayerButton;
    [SerializeField] private Button _quitGameButton;

    [SerializeField] private GameObject _bossShieldPrefab;
    [SerializeField] private GameObject _bossPatternCardPrefab;

    [SerializeField] private TMP_Text _cardsInBossDeckText;
    [SerializeField] private TMP_Text _cardsInBossDiscardText;

    private ShieldSpace[] _shieldSpaces;
    public void Initialize(BossData bossData, Action swapToPlayerAction, Action quitGameAction)
    {
        _nameText.text = bossData.BossName;
        _passiveNameText.text = bossData.Passive.PassiveName;
        _passiveDescriptionText.text = bossData.Passive.PassiveDescription;
        _swapToPlayerButton.onClick.AddListener(() => swapToPlayerAction());
        _quitGameButton.onClick.AddListener(() => quitGameAction());

        // boss shields spawn here
        _shieldSpaces = new ShieldSpace[bossData.Shields.Length];
        for(int i = 0; i < bossData.Shields.Length; i++)
        {
            bool isFinalShield = (i == bossData.Shields.Length - 1);
            GameObject newShield = Instantiate(_bossShieldPrefab, _bossShieldsContainer);
            ShieldSpace newShieldSpace = newShield.GetComponent<ShieldSpace>();
            newShieldSpace.Initialize(bossData.Shields[i], isFinalShield);
            _shieldSpaces[i] = newShieldSpace;
        }

        SetNewActiveShield(0);
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

    public void UpdateDeckCount(int deckCount)
    {
        _cardsInBossDeckText.text = string.Join(" ", deckCount, "cards in Deck");
    }

    public void UpdateDiscardCount(int discardCount)
    {
        _cardsInBossDiscardText.text = string.Join(" ", discardCount, "cards in Discard");
    }

    public BossPatternCard AddCardToActiveBossPattern(BossPatternCardData addedCard)
    {
        GameObject cardPrefab = Instantiate(_bossPatternCardPrefab, _activeBossPatternContainer);
        cardPrefab.GetComponent<BossPatternCard>().Initialize(addedCard);
        return cardPrefab.GetComponent<BossPatternCard>();
    }

    public void DiscardActivatedCard(BossPatternCard discardedCard)
    {
        Destroy(discardedCard.gameObject);
    }
}
