using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternController : MonoBehaviour
{
    public int MaxHandSize { get; private set; } = 3;
    [SerializeField] private PatternView _patternView;
    private List<PatternCardData> _cardsInDeck = new List<PatternCardData>();
    private List<PatternCardData> _cardsInDiscard = new List<PatternCardData>();
    private List<PatternCard> _cardsInHand = new List<PatternCard>(); // hand cards have associated objects, cards in deck don't

    public void Initialize(PatternCardData[] patternCardDatas)
    {
        foreach(PatternCardData data in patternCardDatas)
        {
            _cardsInDeck.Add(data);
        }

        ShuffleDeck();
        UpdateDeckAndDiscardCount();
    }

    public void ShuffleDeck()
    {
        int deckCount = _cardsInDeck.Count;
        int loopUpperBound = deckCount - 1;
        for (var i = 0; i < loopUpperBound; i++)
        {
            int randomCardNumber = Random.Range(i, deckCount);
            PatternCardData tempCard = _cardsInDeck[i];
            _cardsInDeck[i] = _cardsInDeck[randomCardNumber];
            _cardsInDeck[randomCardNumber] = tempCard;
        }
    }

    public void ShuffleDiscardIntoDeck()
    {
        if(_cardsInDiscard.Count <= 0)
        {
            return;
        }

        foreach(PatternCardData data in _cardsInDiscard)
        {
            _cardsInDeck.Add(data);
        }

        Debug.Log("Cards in deck: " + _cardsInDeck.Count);
        _cardsInDiscard.Clear(); 
        UpdateDeckAndDiscardCount();
        ShuffleDeck();
    }

    public void UpdateDeckAndDiscardCount()
    {
        _patternView.UpdateDeckCount(_cardsInDeck.Count);
        _patternView.UpdateDiscardCount(_cardsInDiscard.Count);
    }

    public void DrawPatternFromDeck()
    {
        if (_cardsInDeck.Count <= 0) // Can't draw from an empty deck; try to shuffle discard back into deck
        {
            ShuffleDiscardIntoDeck();
            if(_cardsInDeck.Count <= 0) return; // If deck is still empty, return
        }

        PatternCardData drawnCard = _cardsInDeck[0];
        _cardsInDeck.RemoveAt(0);

        _cardsInHand.Add(_patternView.AddCardToHand(drawnCard));
        UpdateDeckAndDiscardCount();
    }

    public void DrawToMaxHandSize()
    {
        int previousHandSize = _cardsInHand.Count;
        while (_cardsInHand.Count < MaxHandSize)
        {
            DrawPatternFromDeck();
            if (_cardsInHand.Count == previousHandSize) // Drawing pattern failed, likely due to no cards remaining in deck and discard; terminate early
            {
                break;
            } 
        }
    }

    public void DiscardPatternFromHand(PatternCard removeCard)
    {
        if (_cardsInHand.Contains(removeCard))
        {
            _cardsInHand.Remove(removeCard);
            _cardsInDiscard.Add(removeCard.PatternCardDataProperties);
            _patternView.DiscardCardFromHand(removeCard);
            UpdateDeckAndDiscardCount();
        }
    }

    #region Getter Functions
    public int GetNumberOfCardsInHand()
    {
        return _cardsInHand.Count; 
    }

    public int GetNumberOfCardsInDeck()
    {
        return _cardsInDeck.Count;
    }
    #endregion

    #region Setter Functions
    public void SetMaxHandSize(int maxSize)
    {
        MaxHandSize = maxSize;
    }
    #endregion
}
