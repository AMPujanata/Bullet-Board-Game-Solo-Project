using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PatternView : MonoBehaviour
{
    [SerializeField] private GameObject _patternCardPrefab;
    [SerializeField] private Transform _handZone;

    [SerializeField] private TMP_Text _cardsInDeckText;
    [SerializeField] private Image _deckImage;
    [SerializeField] private TMP_Text _cardsInDiscardText;
    [SerializeField] private Image _discardImage;

    public void UpdateDeckCount(int deckCount)
    {
        _cardsInDeckText.text = string.Join(" ", deckCount, "cards");
        _deckImage.gameObject.SetActive(deckCount > 0);
    }

    public void UpdateDiscardCount(int discardCount)
    {
        _cardsInDiscardText.text = string.Join(" ", discardCount, "cards");
        _discardImage.gameObject.SetActive(discardCount > 0);
    }

    public PatternCard AddCardToHand(PatternCardData addedCard)
    {
        GameObject cardPrefab = Instantiate(_patternCardPrefab, _handZone);
        cardPrefab.GetComponent<PatternCard>().Initialize(addedCard.PatternName, addedCard.PatternDescription, addedCard.PatternOwner, addedCard);
        return cardPrefab.GetComponent<PatternCard>();
    }

    public void DiscardCardFromHand(PatternCard discardedCard)
    {
        Destroy(discardedCard.gameObject);
    }
}
