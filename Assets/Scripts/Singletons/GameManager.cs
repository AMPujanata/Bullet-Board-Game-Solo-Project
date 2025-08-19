using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
        ActivePlayer.CurrentController.DrawBulletsFromCenter(_startingBullets);
        ActivePlayer.PatternController.DrawToMaxHandSize();
        // also add powerups setup later
        _activePlayerBoardCanvasGroup.interactable = true;
        yield break;
    }

    public void BeginEndPhase()
    {
        // take a powerup
        ActivePlayer.PatternController.DrawToMaxHandSize();
        ActivePlayer.CurrentController.DrawBulletsFromCenter(_currentIntensity);
        // and then start Cleanup Phase after "all" players are done
        BeginCleanupPhase();
    }

    private void BeginCleanupPhase()
    {
        // draw more powerups
        _currentIntensity += 1; // and +1 more for each lost Heroine
        // take all bullets in incoming and place in current
        ActivePlayer.ActionController.RefreshAP();
        // start a new round (option phase)
    }

    public PlayerController ActivePlayer { get; private set; }
    private CanvasGroup _activePlayerBoardCanvasGroup;
    public PlayerController Player1 { get; private set; }
}
