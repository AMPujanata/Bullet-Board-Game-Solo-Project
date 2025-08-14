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
    private PatternSpaceData[,] _patternSpaceDatas;

    public void Initialize(string patternName, string patternDescription, string patternOwner, PatternSpaceData[,] patternSpaceDatas)
    {
        _patternName.text = patternName;
        _patternDescription.text = patternDescription;
        _patternOwner.text = patternOwner;
        _patternSpaceDatas = patternSpaceDatas;

        int rows = patternSpaceDatas.GetLength(0);
        int columns = patternSpaceDatas.GetLength(1);
        _patternSpaceGridParent.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _patternSpaceGridParent.GetComponent<GridLayoutGroup>().constraintCount = columns;

        for(int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                GameObject newPatternSpace = Instantiate(_patternSpacePrefab, _patternSpaceGridParent);
                newPatternSpace.GetComponent<PatternSpaceView>().Initialize(patternSpaceDatas[i, j]);
            }
        }

    }


    public void SelectPattern()
    {
        Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.8f, 0.5f));
        PopupManager.Instance.DisplayPopup("Choose a valid bullet pattern to clear.", "Cancel", popupLocation, GameManager.Instance.ActivePlayer.CurrentController.CancelSpaceSelection);

        GameManager.Instance.ActivePlayer.CurrentController.CheckValidSpacesOnHover(_patternSpaceDatas, (bool isSuccessful, Vector2Int finalTopLeftCell) =>
        {
            if (!isSuccessful) return;
            PopupManager.Instance.ClosePopup();

            GameManager.Instance.ActivePlayer.CurrentController.RemoveBulletsFromCurrentWithPattern(finalTopLeftCell, _patternSpaceDatas);
            // Card disposal pattern here
        });
    }
}
