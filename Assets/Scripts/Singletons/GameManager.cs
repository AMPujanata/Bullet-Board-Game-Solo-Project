using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameMode CurrentMode = GameMode.ScoreAttack;

    public static GameManager Instance { get; private set; }
    public int CurrentIntensity { get; private set; } = 4;
    public int CurrentRound { get; private set; } = 1;
    private int _startingBullets = 10;
    public int TotalClearedBullets { get; private set; } = 1; // used for end of match statistics
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this); // Make sure there's only ever one 
            return; // do NOT run any other code
        }

        DontDestroyOnLoad(gameObject);

        Player1 = FindObjectOfType<PlayerController>(); // move to a "when starting gameplay" method later
        ActivePlayer = Player1;
        _activePlayerBoardCanvasGroup = ActivePlayer.GetComponentInParent<CanvasGroup>();
        StartCoroutine(BeginSetup());
    }

    private IEnumerator BeginSetup()
    {
        _activePlayerBoardCanvasGroup.interactable = false;
        yield return new WaitForSeconds(0.5f);
        ActivePlayer.SightController.DrawBulletsFromCenter(_startingBullets);
        ActivePlayer.PatternController.DrawToMaxHandSize();
        _activePlayerBoardCanvasGroup.interactable = true;
        ActivePlayer.SightController.UpdateCurrentIntensity(CurrentIntensity, CenterManager.Instance.GetNumberOfBulletsInIntensity());
        CurrentRound = 1;
        yield break;
    }

    public void BeginEndPhase()
    {
        _activePlayerBoardCanvasGroup.interactable = false;
        ActivePlayer.PatternController.DrawToMaxHandSize();
        if(CurrentMode == GameMode.ScoreAttack)
        {
            int extraBullets = CenterManager.Instance.GetNumberOfBulletsInIntensity();
            ActivePlayer.SightController.DrawBulletsFromCenter(CurrentIntensity + extraBullets);
            CenterManager.Instance.ReturnAllBulletsFromIntensityToCenter();
        }
        else
        {
            ActivePlayer.SightController.DrawBulletsFromCenter(CurrentIntensity);
        }
        // and then start Cleanup Phase after "all" players are done
        BeginCleanupPhase();
    }

    private void BeginCleanupPhase()
    {
        CurrentIntensity += 1;
        ActivePlayer.SightController.UpdateCurrentIntensity(CurrentIntensity, CenterManager.Instance.GetNumberOfBulletsInIntensity());
        // take all bullets in incoming and place in current
        ActivePlayer.ActionController.RefreshAP();
        CurrentRound++;
        _activePlayerBoardCanvasGroup.interactable = true;
    }

    public void TriggerGameOver()
    {
        if(CurrentMode == GameMode.ScoreAttack)
        {
            Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
            PopupManager.Instance.DisplayPopup("GAME OVER\nRounds Survived: " + CurrentRound + "\nBullets Cleared: " + TotalClearedBullets, popupLocation, "OK");
        }
    }

    public void AddBulletToTotalClear()
    {
        TotalClearedBullets++;
    }

    public PlayerController ActivePlayer { get; private set; }
    private CanvasGroup _activePlayerBoardCanvasGroup;
    public PlayerController Player1 { get; private set; }
}
