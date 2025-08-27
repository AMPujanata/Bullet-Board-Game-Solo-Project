using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class BossPatternCard : MonoBehaviour
{
    [SerializeField] private TMP_Text _patternName;
    [SerializeField] private TMP_Text _patternDescription;
    [SerializeField] private TMP_Text _patternPunishment;
    [SerializeField] private TMP_Text _patternOwner;
    [SerializeField] private Transform _patternSpaceGridParent;
    [SerializeField] private GameObject _patternSpacePrefab;
    public BossPatternCardData BossPatternCardProperties { get; private set; }
    private PatternSpaceData[,] _patternSpaceGrid;

    public void Initialize(BossPatternCardData bossPatternCardData)
    {
        _patternName.text = bossPatternCardData.PatternName;
        _patternDescription.text = bossPatternCardData.PatternDescription;
        _patternOwner.text = bossPatternCardData.PatternOwner;
        _patternPunishment.text = bossPatternCardData.OnFailEffect.EffectDescription;
        BossPatternCardProperties = bossPatternCardData;

        int numberOfRows = BossPatternCardProperties.PatternGrid.Length;
        int numberOfColumns = BossPatternCardProperties.PatternGrid[0].PatternSpaces.Length;
        PatternSpaceData[,] patternSpaceGrid = new PatternSpaceData[numberOfRows, numberOfColumns];

        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                patternSpaceGrid[i, j] = BossPatternCardProperties.PatternGrid[i].PatternSpaces[j];
            }
        }

        _patternSpaceGrid = patternSpaceGrid;

        int rows = _patternSpaceGrid.GetLength(0);
        int columns = _patternSpaceGrid.GetLength(1);
        _patternSpaceGridParent.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _patternSpaceGridParent.GetComponent<GridLayoutGroup>().constraintCount = columns;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject newPatternSpace = Instantiate(_patternSpacePrefab, _patternSpaceGridParent);
                newPatternSpace.GetComponent<PatternSpaceView>().Initialize(_patternSpaceGrid[i, j]);
            }
        }

        if(gameObject.activeInHierarchy) StartCoroutine(UpdateGridSpace()); // can't update grid space if not active
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

    public void ActivateBossPattern(Action<bool> callback)
    {
        Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.8f, 0.5f));
        PopupManager.Instance.DisplayPopup("Choose a valid bullet pattern to clear.\n(If there are no valid patterns, click cancel.)", popupLocation, "Cancel", GameManager.Instance.ActivePlayer.SightController.CancelSpaceSelection);

        GameManager.Instance.ActivePlayer.SightController.CheckValidSpacesOnHover(_patternSpaceGrid, (bool isSuccessful, Vector2Int finalTopLeftCell) =>
        {
            if (isSuccessful) // If it's successful, then the boss's on failing pattern effect doesn't activate
            {
                PopupManager.Instance.ClosePopup();
                callback.Invoke(true);
                return;
            }

            BossPatternCardProperties.OnFailEffect.ActivateEffect();
            callback.Invoke(false);
        });
    }

    private void OnEnable()
    {
        UpdateGridSpace();
    }
}
