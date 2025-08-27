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

    [Tooltip("Width should be adjusted based on the max width of 1920x1080.")]
    [SerializeField] private int _handMaxWidth;

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

    private void UpdateHandSize()
    {
        RectTransform handRect = _handZone.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(handRect);
        if(handRect.rect.width > _handMaxWidth)
        {
            float scaleToFit = _handMaxWidth / handRect.rect.width;
            handRect.transform.localScale = new Vector3(scaleToFit, scaleToFit, scaleToFit);
        }
        else
        {
            handRect.transform.localScale = Vector3.one;
        }
    }

    public PatternCard AddCardToHand(PatternCardData addedCard)
    {
        GameObject cardPrefab = Instantiate(_patternCardPrefab, _handZone);
        cardPrefab.GetComponent<PatternCard>().Initialize(addedCard);
        UpdateHandSize();
        return cardPrefab.GetComponent<PatternCard>();
    }

    public void DiscardCardFromHand(PatternCard discardedCard)
    {
        Destroy(discardedCard.gameObject);
        UpdateHandSize();
    }
}
