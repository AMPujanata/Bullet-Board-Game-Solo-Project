using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PatternCard : MonoBehaviour
{
    [SerializeField] private TMP_Text _patternName;
    [SerializeField] private TMP_Text _patternDescription;
    [SerializeField] private TMP_Text _patternOwner;
    [SerializeField] private Transform _patternSpaceGridParent;
    [SerializeField] private GameObject _patternSpacePrefab;
    public PatternCardData PatternCardDataProperties { get; private set; }
    private PatternSpaceData[,] _patternSpaceGrid;
    public void Initialize(string patternName, string patternDescription, string patternOwner, PatternCardData patternCardData)
    {
        _patternName.text = patternName;
        _patternDescription.text = patternDescription;
        _patternOwner.text = patternOwner;
        PatternCardDataProperties = patternCardData;

        int numberOfRows = PatternCardDataProperties.PatternGrid.Length;
        int numberOfColumns = PatternCardDataProperties.PatternGrid[0].PatternSpaces.Length;
        PatternSpaceData[,] patternSpaceGrid = new PatternSpaceData[numberOfRows, numberOfColumns];

        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                patternSpaceGrid[i, j] = PatternCardDataProperties.PatternGrid[i].PatternSpaces[j];
            }
        }

        _patternSpaceGrid = patternSpaceGrid;

        int rows = _patternSpaceGrid.GetLength(0);
        int columns = _patternSpaceGrid.GetLength(1);
        _patternSpaceGridParent.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _patternSpaceGridParent.GetComponent<GridLayoutGroup>().constraintCount = columns;

        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                GameObject newPatternSpace = Instantiate(_patternSpacePrefab, _patternSpaceGridParent);
                newPatternSpace.GetComponent<PatternSpaceView>().Initialize(_patternSpaceGrid[i, j]);
            }
        }

    }

    public void SelectPattern()
    {
        Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.8f, 0.5f));
        PopupManager.Instance.DisplayPopup("Choose a valid bullet pattern to clear.", "Cancel", popupLocation, GameManager.Instance.ActivePlayer.SightController.CancelSpaceSelection);

        GameManager.Instance.ActivePlayer.SightController.CheckValidSpacesOnHover(_patternSpaceGrid, (bool isSuccessful, Vector2Int finalTopLeftCell) =>
        {
            if (!isSuccessful) return;
            PopupManager.Instance.ClosePopup();

            GameManager.Instance.ActivePlayer.SightController.RemoveBulletsFromSightWithPattern(finalTopLeftCell, _patternSpaceGrid);
            GameManager.Instance.ActivePlayer.PatternController.DiscardPatternFromHand(this);
        });
    }
}
