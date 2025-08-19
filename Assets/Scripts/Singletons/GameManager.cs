using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameMode CurrentMode = GameMode.ScoreAttack;

    public static GameManager Instance { get; private set; }
    private int _currentIntensity = 4;
    private int _startingBullets = 10;

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
        yield break;
    }

    public void BeginEndPhase()
    {
        _activePlayerBoardCanvasGroup.interactable = false;
        ActivePlayer.PatternController.DrawToMaxHandSize();
        if(CurrentMode == GameMode.ScoreAttack)
        {
            int extraBullets = CenterManager.Instance.GetNumberOfBulletsInIntensity();
            ActivePlayer.SightController.DrawBulletsFromCenter(_currentIntensity + extraBullets);
            CenterManager.Instance.ReturnAllBulletsFromIntensityToCenter();
        }
        else
        {
            ActivePlayer.SightController.DrawBulletsFromCenter(_currentIntensity);
        }
        // and then start Cleanup Phase after "all" players are done
        BeginCleanupPhase();
    }

    private void BeginCleanupPhase()
    {
        _currentIntensity += 1;
        // take all bullets in incoming and place in current
        ActivePlayer.ActionController.RefreshAP();
        _activePlayerBoardCanvasGroup.interactable = true;
    }

    public PlayerController ActivePlayer { get; private set; }
    private CanvasGroup _activePlayerBoardCanvasGroup;
    public PlayerController Player1 { get; private set; }
}
