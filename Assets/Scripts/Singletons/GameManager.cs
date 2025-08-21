using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameMode CurrentMode = GameMode.ScoreAttack;

    public static GameManager Instance { get; private set; }
    public int CurrentIntensity { get; private set; } = 4;
    public int CurrentRound { get; private set; } = 1;
    private int _startingBullets = 10;
    public int TotalClearedBullets { get; private set; } = 1; // used for end of match statistics

    [SerializeField] private PlayerData[] _selectablePlayerDatas;
    [SerializeField] private PlayerData _activePlayerData;

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

    public void StartGame()
    {
        SceneManager.LoadScene("MainGameplayScene");
        StartCoroutine(LoadMainGameplayScene());
    }

    private IEnumerator LoadMainGameplayScene()
    {
        AsyncOperation asyncLoadGameplay = SceneManager.LoadSceneAsync("MainGameplayScene", LoadSceneMode.Single);
        while (!asyncLoadGameplay.isDone) yield return null;
        yield return new WaitForEndOfFrame(); // let all objects on scene go through Awake and Start methods first

        StartCoroutine(BeginGameSetup());
    }

    private IEnumerator BeginGameSetup()
    {

        Player1 = FindObjectOfType<PlayerController>();
        ActivePlayer = Player1;
        ActivePlayer.Initialize(_activePlayerData);
        _activePlayerBoardCanvasGroup = ActivePlayer.GetComponentInParent<CanvasGroup>();
        _activePlayerBoardCanvasGroup.interactable = false;

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
