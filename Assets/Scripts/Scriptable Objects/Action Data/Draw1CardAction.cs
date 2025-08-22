using UnityEngine;

[CreateAssetMenu(fileName = "Draw1CardActionSO", menuName = "BaseAction/Draw1Card")]
public class Draw1CardAction : BaseAction
{
    public override void ActivateAction(System.Action<bool> callback)
    {
        int currentAP = GameManager.Instance.ActivePlayer.ActionController.CurrentAP;
        if (currentAP < ActionCost)
        {
            Vector2 warningPopupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
            PopupManager.Instance.DisplayPopup("Not enough AP to use action!", warningPopupLocation, "OK");
            callback.Invoke(false);
            return;
        }

        int deckCount = GameManager.Instance.ActivePlayer.PatternController.GetNumberOfCardsInDeck();
        if (deckCount <= 0)
        {
            GameManager.Instance.ActivePlayer.PatternController.ShuffleDiscardIntoDeck();
            deckCount = GameManager.Instance.ActivePlayer.PatternController.GetNumberOfCardsInDeck();
            if(deckCount <= 0)
            {
                Vector2 warningPopupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
                PopupManager.Instance.DisplayPopup("No cards in deck to draw!", warningPopupLocation, "OK");
                callback.Invoke(false);
                return;
            }
        }

        GameManager.Instance.ActivePlayer.ActionController.ModifyCurrentAP(-ActionCost);
        GameManager.Instance.ActivePlayer.PatternController.DrawPatternFromDeck();
        callback.Invoke(true);
    }
}
