using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PatternCard : MonoBehaviour
{
    [SerializeField] private Button _patternCardButton;
    [SerializeField] private TMP_Text _patternName;
    [SerializeField] private TMP_Text _patternDescription;
    [SerializeField] private TMP_Text _patternOwner;
    [SerializeField] private Transform _patternSpaceGridParent;
    [SerializeField] private GameObject _patternSpacePrefab;
    public PatternCardData PatternCardDataProperties { get; private set; }
    private PatternSpaceData[,] _patternSpaceGrid;
    public void Initialize(PatternCardData patternCardData, bool isShowcasePattern = false)
    {
        _patternName.text = patternCardData.PatternName;
        _patternDescription.text = patternCardData.PatternDescription;
        _patternOwner.text = patternCardData.PatternOwner;
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

        if(gameObject.activeInHierarchy) StartCoroutine(UpdateGridSpace());

        if(!isShowcasePattern) _patternCardButton.onClick.AddListener(SelectPattern); // some cards are only meant to be seen, not interacted with
    }

    private IEnumerator UpdateGridSpace()
    {
        RectTransform gridRect = _patternSpaceGridParent.GetComponent<RectTransform>();
        _patternSpaceGridParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(0, 0);
        yield return new WaitForEndOfFrame();
        float minSize = Mathf.Min(gridRect.rect.width, gridRect.rect.height) / 6; // extra padding
        _patternSpaceGridParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(minSize, minSize);
        yield break;
    }

    public void SelectPattern()
    {
        Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.8f, 0.5f));
        OverlayManager.Instance.DisplayPopup("Choose a valid bullet pattern to clear.", popupLocation, "Cancel", GameManager.Instance.ActivePlayer.SightController.CancelSpaceSelection);

        GameManager.Instance.ActivePlayer.SightController.CheckValidSpacesOnHover(_patternSpaceGrid, (bool isSuccessful, Vector2Int finalTopLeftCell) =>
        {
            if (!isSuccessful) return;
            OverlayManager.Instance.ClosePopup();

            GameManager.Instance.ActivePlayer.SightController.RemoveBulletsFromSightWithPattern(finalTopLeftCell, _patternSpaceGrid);
            GameManager.Instance.ActivePlayer.PatternController.DiscardPatternFromHand(this);
        });
    }

    private void OnEnable()
    {
        UpdateGridSpace();
    }
}
