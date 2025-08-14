using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PlayerView _playerView;
    [SerializeField] private ActionController _actionController;
    public ActionController ActionController { get { return _actionController; } }
    [SerializeField] private CurrentController _currentController;
    public CurrentController CurrentController { get { return _currentController; } }

    [SerializeField] private PatternCardData _testCardData;
    [SerializeField] private GameObject _testPatternCardPrefab;
    private int currentHP;
    public int CurrentHP { get; private set; }
    private int maxHP;


    private void Start()
    {
        maxHP = _playerData.MaxHP;
        currentHP = maxHP;
        _playerView.Initialize(_playerData);
        _actionController.Initialize(_playerData.MaxAP, _playerData.Actions);
    }

    public void ModifyCurrentHP(int value) // increases or decreases current HP by the value's amount
    {
        currentHP += value;
        Mathf.Clamp(currentHP, 0, maxHP);
        _playerView.ChangeHPValue(currentHP, maxHP);
    }

    public void DrawNewPattern()
    {
        // NOTE: test pattern only for now
        int numberOfRows = _testCardData.PatternGrid.Length;
        int numberOfColumns = _testCardData.PatternGrid[0].PatternSpaces.Length;
        PatternSpaceData[,] patternGrid = new PatternSpaceData[numberOfRows, numberOfColumns];

        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                patternGrid[i, j] = _testCardData.PatternGrid[i].PatternSpaces[j];
            }
        }
        GameObject testPrefab = Instantiate(_testPatternCardPrefab, _playerView.transform);
        testPrefab.GetComponent<PatternCard>().Initialize(_testCardData.PatternName, _testCardData.PatternDescription, _testCardData.PatternOwner, patternGrid);
    }

}
