using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BossView))]
public class BossController : MonoBehaviour
{
    [SerializeField] private BossView _bossView;
    private BossData _bossData;
    public int BrokenShieldsCount { get; private set; }

    private List<BulletData> _bulletsInBossIncoming = new List<BulletData>();

    public int MaxActivePatternsSize = 1;
    private List<BossPatternCardData> _cardsInDeck = new List<BossPatternCardData>();
    private List<BossPatternCardData> _cardsInDiscard = new List<BossPatternCardData>();
    private List<BossPatternCard> _activeBossPatternCards = new List<BossPatternCard>(); // hand cards have associated objects, cards in deck don't

    public void Initialize(BossData bossData)
    {
        _bossData = bossData;
        _bossView.Initialize(_bossData, SwapToPlayerPanel);

        foreach (BossPatternCardData data in _bossData.Patterns)
        {
            _cardsInDeck.Add(data);
        }
        ShuffleDeck();
        UpdateDeckAndDiscardCount();

        BrokenShieldsCount = 0;
        _bossView.SetNewActiveShield(BrokenShieldsCount);
        _bossView.UpdateBulletIncomingText(_bulletsInBossIncoming.Count);

        bossData.Passive.SetupPassive();
        DrawToMaxActivePatternsSize();
    }

    public void SwapToPlayerPanel()
    {
        _bossView.gameObject.SetActive(false);
        GameManager.Instance.ActivePlayer.gameObject.SetActive(true);
    }

    public void AddBulletToBossIncoming(BulletData data)
    {
        _bulletsInBossIncoming.Add(data);
        _bossView.UpdateBulletIncomingText(_bulletsInBossIncoming.Count);
    }

    public void CheckShieldBreak()
    {
        bool checkMoreShields = true;
        bool shieldBroken = false;
        int bulletsAlreadyUsedCount = 0;

        while (checkMoreShields)
        {
            if (BrokenShieldsCount + 1 >= _bossData.Shields.Length) break; // no more shields to check
            ShieldData activeShield = _bossData.Shields[BrokenShieldsCount];
            if (_bulletsInBossIncoming.Count >= activeShield.NextShieldBreak + bulletsAlreadyUsedCount)
            {
                BrokenShieldsCount++;
                BaseBossEffect nextOnShieldBreak = _bossView.GetAllShieldSpaces()[BrokenShieldsCount].ShieldProperties.OnShieldBreak;
                if (nextOnShieldBreak != null) nextOnShieldBreak.ActivateEffect();
                shieldBroken = true;
                bulletsAlreadyUsedCount += activeShield.NextShieldBreak;
            }
            else
            {
                checkMoreShields = false;
            }
        }

        if (shieldBroken) // only clear incoming if a shield was broken
        {
            _bossView.SetNewActiveShield(BrokenShieldsCount);
            foreach (BulletData data in _bulletsInBossIncoming)
            {
                CenterManager.Instance.AddBulletToCenter(data);
            }
            _bulletsInBossIncoming.Clear();
            _bossView.UpdateBulletIncomingText(_bulletsInBossIncoming.Count);
        }

        if (BrokenShieldsCount + 1 >= _bossData.Shields.Length) // boss is defeated
        {
            GameManager.Instance.TriggerVictory();
        }
    }

    public int GetCurrentBossIntensity()
    {
        return _bossData.Shields[BrokenShieldsCount].ShieldIntensity;
    }

    #region Pattern Functions
    public void ShuffleDeck()
    {
        int deckCount = _cardsInDeck.Count;
        int loopUpperBound = deckCount - 1;
        for (var i = 0; i < loopUpperBound; i++)
        {
            int randomCardNumber = UnityEngine.Random.Range(i, deckCount);
            BossPatternCardData tempCard = _cardsInDeck[i];
            _cardsInDeck[i] = _cardsInDeck[randomCardNumber];
            _cardsInDeck[randomCardNumber] = tempCard;
        }
    }

    public void ShuffleDiscardIntoDeck()
    {
        if (_cardsInDiscard.Count <= 0)
        {
            return;
        }

        foreach (BossPatternCardData data in _cardsInDiscard)
        {
            _cardsInDeck.Add(data);
        }

        _cardsInDiscard.Clear();
        UpdateDeckAndDiscardCount();
        ShuffleDeck();
    }

    public void UpdateDeckAndDiscardCount()
    {
        _bossView.UpdateDeckCount(_cardsInDeck.Count);
        _bossView.UpdateDiscardCount(_cardsInDiscard.Count);
    }

    public void DrawPatternFromDeck()
    {
        if (_cardsInDeck.Count <= 0) // Can't draw from an empty deck; try to shuffle discard back into deck
        {
            ShuffleDiscardIntoDeck();
            if (_cardsInDeck.Count <= 0) return; // If deck is still empty, return
        }

        BossPatternCardData drawnCard = _cardsInDeck[0];
        _cardsInDeck.RemoveAt(0);

        _activeBossPatternCards.Add(_bossView.AddCardToActiveBossPattern(drawnCard));
        UpdateDeckAndDiscardCount();
    }

    public void DrawToMaxActivePatternsSize()
    {
        int previousHandSize = _activeBossPatternCards.Count;
        while (_activeBossPatternCards.Count < MaxActivePatternsSize)
        {
            DrawPatternFromDeck();
            if (_activeBossPatternCards.Count == previousHandSize) // Drawing pattern failed, likely due to no cards remaining in deck and discard; terminate early
            {
                break;
            }
        }
    }

    private Queue<BossPatternCard> _bossPatternsQueue = new Queue<BossPatternCard>();
    private bool _currentlyResolvingBossPatterns = false;
    public void ActivateAllBossPatterns(Action<bool> finalCallback)
    {
        foreach(BossPatternCard card in _activeBossPatternCards)
        {
            _bossPatternsQueue.Enqueue(card);
        }

        NextBossPatternInQueue(finalCallback);
    }

    private void NextBossPatternInQueue(Action<bool> finalCallback)
    {
        if (_bossPatternsQueue.Count <= 0 || _currentlyResolvingBossPatterns) // no patterns left to check, and don't check patterns while another is happening
        {
            finalCallback.Invoke(true);
            return;
        }

        BossPatternCard activatedCard = _bossPatternsQueue.Dequeue();
        _currentlyResolvingBossPatterns = true;

        activatedCard.ActivateBossPattern((isSucessful) =>
        {
            if (_activeBossPatternCards.Contains(activatedCard))
            {
                _activeBossPatternCards.Remove(activatedCard);
                _cardsInDiscard.Add(activatedCard.BossPatternCardProperties);
                _bossView.DiscardActivatedCard(activatedCard);
                UpdateDeckAndDiscardCount();
            }

            _currentlyResolvingBossPatterns = false;
            NextBossPatternInQueue(finalCallback);
        });
    }
    #endregion
}
