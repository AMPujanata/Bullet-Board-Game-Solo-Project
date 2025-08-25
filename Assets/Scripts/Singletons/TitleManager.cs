using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    private enum MenuName { TitleMenu = 1, MatchSettings = 2}
    public static TitleManager Instance { get; private set; }

    #region Main Title 
    [SerializeField] private GameObject _titleMenu;
    [SerializeField] private Button _scoreAttackButton;
    [SerializeField] private Button _tutorialButton;
    [SerializeField] private Button _quitButton;
    #endregion

    #region Match Settings
    [SerializeField] private GameObject _matchSettingsMenu;
    [SerializeField] private TMP_Dropdown _characterSelectDropdown;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _backToTitleButton;
    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this); // Make sure there's only ever one 
            return; // do NOT run any other code
        }
    }

    private void Start()
    {
        _scoreAttackButton.onClick.AddListener(() => ScoreAttackSetup());
        _tutorialButton.onClick.AddListener(() => OpenTutorial());
        _quitButton.onClick.AddListener(() => QuitGame());

        _backToTitleButton.onClick.AddListener(() => ChangeActiveMenu(MenuName.TitleMenu));
        _startGameButton.onClick.AddListener(() => GameManager.Instance.StartGame());

        PlayerData[] allPlayers = GameManager.Instance.GetSelectablePlayers();
        List<string> allPlayerNames = new List<string>();
        foreach (PlayerData data in allPlayers)
        {
            allPlayerNames.Add(data.PlayerName);
        }
        _characterSelectDropdown.AddOptions(allPlayerNames);
        _characterSelectDropdown.onValueChanged.AddListener((value) => OnCharacterDropdownChanged(_characterSelectDropdown));
        OnCharacterDropdownChanged(_characterSelectDropdown); // Sets initial value to GameManager, since dropdown won't do it automatically
    }

    private void ChangeActiveMenu(MenuName menuName)
    {
        if(menuName == MenuName.TitleMenu)
        {
            _titleMenu.SetActive(true);
            _matchSettingsMenu.SetActive(false);
        }
        else if(menuName == MenuName.MatchSettings)
        {
            _titleMenu.SetActive(false);
            _matchSettingsMenu.SetActive(true);
        }
    }

    private void OnCharacterDropdownChanged(TMP_Dropdown dropdown)
    {
        string characterName = dropdown.options[dropdown.value].text;
        PlayerData[] allPlayers = GameManager.Instance.GetSelectablePlayers();
        foreach(PlayerData data in allPlayers)
        {
            if(characterName == data.PlayerName)
            {
                GameManager.Instance.SetActivePlayerData(data);
                break;
            }
        }
    }

    private void ScoreAttackSetup()
    {
        GameManager.Instance.CurrentMode = GameMode.ScoreAttack;
        ChangeActiveMenu(MenuName.MatchSettings);
    }

    private void OpenTutorial()
    {
        Application.OpenURL("https://cdn.shopify.com/s/files/1/0277/6724/2855/files/Bullet_Star_Rulebook.pdf");
        // could be expanded into a full tutorial popup if there is enough time
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
