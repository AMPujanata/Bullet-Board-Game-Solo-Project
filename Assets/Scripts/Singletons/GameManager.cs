using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameMode CurrentMode = GameMode.ScoreAttack;

    public static GameManager Instance { get; private set; }
    public int CurrentIntensity { get; private set; } = 4;
    public int CurrentRound { get; private set; } = 1;
    private const int _startingBullets = 10;
    public int TotalClearedBullets { get; private set; } = 1; // used for end of match statistics

    [SerializeField] private PlayerData[] _selectablePlayerDatas;
    [SerializeField] private PlayerData _activePlayerData;
    [SerializeField] private BossData[] _selectableBossDatas;
    [SerializeField] private BossData _activeBossData;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this); // Make sure there's only ever one 
            return; // do NOT run any other code
        }

        DontDestroyOnLoad(gameObject);

    #if UNITY_EDITOR
        if(SceneManager.GetActiveScene().name == "MainGameplayScene") // special code to allow quick testing of gameplay scene in editor
        {
            StartCoroutine(BeginGameSetup());
        }
    #endif
    }

    public PlayerData[] GetSelectablePlayers()
    {
        return _selectablePlayerDatas;
    }

    public void SetActivePlayerData(PlayerData activePlayer)
    {
        _activePlayerData = activePlayer;
    }

    public BossData[] GetSelectableBosses()
    {
        return _selectableBossDatas;
    }

    public void SetActiveBossData(BossData activeBoss)
    {
        _activeBossData = activeBoss;
    }

    public void StartGame()
    {
        StartCoroutine(LoadMainGameplayScene());
    }

    private IEnumerator LoadMainGameplayScene()
    {
        AsyncOperation asyncLoadGameplay = SceneManager.LoadSceneAsync("MainGameplayScene", LoadSceneMode.Single);
        while (!asyncLoadGameplay.isDone) yield return null;

        StartCoroutine(BeginGameSetup());
    }

    public void ShowReturnToTitleScenePopup()
    {
        Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
        OverlayManager.Instance.DisplayPopup("Are you sure you want to return to the title screen?", popupLocation, "Yes", EndGame, "No", null);
    }

    public void EndGame()
    {
        StartCoroutine(LoadTitleScene());
    }

    private IEnumerator LoadTitleScene()
    {
        AsyncOperation asyncLoadGameplay = SceneManager.LoadSceneAsync("TitleScreenScene", LoadSceneMode.Single);
        while (!asyncLoadGameplay.isDone) yield return null;
    }

    private IEnumerator BeginGameSetup()
    {
        yield return new WaitForEndOfFrame(); // let all objects on scene go through Awake and Start methods first
        Player1 = FindObjectOfType<PlayerController>(true);
        ActivePlayer = Player1;
        ActivePlayer.Initialize(_activePlayerData);
        _activePlayerBoardCanvasGroup = ActivePlayer.GetComponentInParent<CanvasGroup>();
        _activePlayerBoardCanvasGroup.interactable = false;
        ActivePlayer.SightController.DrawBulletsFromCenter(_startingBullets);

        if (CurrentMode == GameMode.BossBattle)
        {
            ActiveBoss = FindObjectOfType<BossController>(true);
            ActiveBoss.Initialize(_activeBossData);
        }

        ActivePlayer.PatternController.DrawToMaxHandSize();
        if (CurrentMode == GameMode.ScoreAttack)
        {
            ActivePlayer.SightController.UpdateCurrentIntensity(CurrentIntensity, CenterManager.Instance.GetNumberOfBulletsInIntensity()); // add boss support later
        }
        else if (CurrentMode == GameMode.BossBattle)
        {
            ActivePlayer.SightController.UpdateCurrentIntensity(ActiveBoss.GetCurrentBossIntensity());
        }

        _activePlayerBoardCanvasGroup.interactable = true;
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
        // boss battle doesnt make you draw from intensity

        // and then start Cleanup Phase after "all" players are done
        BeginCleanupPhase();
    }

    private void BeginCleanupPhase()
    {
        if (CurrentMode == GameMode.ScoreAttack) // intensity doesnt update during boss battles
        {
            CurrentIntensity += 1;
            ActivePlayer.SightController.UpdateCurrentIntensity(CurrentIntensity, CenterManager.Instance.GetNumberOfBulletsInIntensity());
        }
        ActivePlayer.ActionController.RefreshAP();
        if (CurrentMode == GameMode.ScoreAttack)
        {
            StartNewRound();
        }
        else
        {
            BeginBossPhase();
        }
    }

    private void BeginBossPhase()
    {
        BossPhaseStart.Invoke();
        ActivePlayer.SwapToBossPanel(); // swap to boss panel so active boss patterns are visible
        ActiveBoss.ActivateAllBossPatterns((isSuccessful) =>
        {
            if (_gameIsOver) return; // player could possibly die from the boss pattern
            ActiveBoss.CheckShieldBreak();
            if (_gameIsOver) return; // game is over, no need to process the rest
            ActivePlayer.SightController.DrawBulletsFromCenter(ActiveBoss.GetCurrentBossIntensity());
            ActiveBoss.DrawToMaxActivePatternsSize();
            StartNewRound();
        });
    }

    private void StartNewRound()
    {
        CurrentRound++;
        _activePlayerBoardCanvasGroup.interactable = true;
    }

    private bool _gameIsOver = false;
    public void TriggerGameOver()
    {
        if (_gameIsOver) return; // there is already a game over/victory triggered

        _gameIsOver = true;

        if (CurrentMode == GameMode.ScoreAttack)
        {
            Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
            OverlayManager.Instance.DisplayPopup("GAME OVER\nRounds Survived: " + CurrentRound + "\nBullets Cleared: " + TotalClearedBullets, popupLocation, "OK", EndGame);
        }
        else if(CurrentMode == GameMode.BossBattle)
        {
            Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
            OverlayManager.Instance.DisplayPopup("GAME OVER\nRounds Survived: " + CurrentRound + "\nShields Broken:" + ActiveBoss.BrokenShieldsCount, popupLocation, "OK", EndGame);
        }
    }

    public void TriggerVictory()
    {
        if (_gameIsOver) return; // there is already a game over/victory triggered
        _gameIsOver = true;

        if (CurrentMode == GameMode.BossBattle) // you can't "win" in score attack
        {
            Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
            OverlayManager.Instance.DisplayPopup("VICTORY!\nTotal Rounds: " + CurrentRound, popupLocation, "OK", EndGame);
        }
    }

    public void AddBulletToTotalClear()
    {
        TotalClearedBullets++;
    }

    public PlayerController ActivePlayer { get; private set; }
    private CanvasGroup _activePlayerBoardCanvasGroup;
    public PlayerController Player1 { get; private set; }

    public BossController ActiveBoss { get; private set; }

    public UnityEvent BossPhaseStart { get; private set; } = new UnityEvent();
}
