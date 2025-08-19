using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CurrentView))]
public class CurrentController : MonoBehaviour
{
    [SerializeField] private CurrentView _currentView;
    private List<BulletData> _bulletsInCurrentBag = new List<BulletData>();

    #region Hover Routine Variables
    private bool _isAcceptingHoverInputs = false;
    private Vector2Int _previousActiveSpacePosition = new Vector2Int(-1, -1);
    private Vector2Int _activeSpacePosition = new Vector2Int(-1, -1);
    private PatternSpaceData[,] _currentSpaceRequirements = null;
    private bool _shouldCancelSpaceSelection = false;
    #endregion

    public void DrawBulletsFromCenter(int bulletCount)
    {
        for(int i = 0;  i < bulletCount; i++)
        {
            _bulletsInCurrentBag.Add(CenterManager.Instance.TakeRandomBulletFromCenter());
        }
        _currentView.UpdateCurrentBulletsText(_bulletsInCurrentBag.Count);
    }

    public void SendBulletToCurrent()
    {
        if(_bulletsInCurrentBag.Count <= 0)
        {
            GameManager.Instance.BeginEndPhase();
            //Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
            //PopupManager.Instance.DisplayPopup("No bullet left to place!", "OK", popupLocation);
            return;
        }

        BulletData bulletToSpawn = _bulletsInCurrentBag[UnityEngine.Random.Range(0, _bulletsInCurrentBag.Count - 1)];
        _bulletsInCurrentBag.Remove(bulletToSpawn);
        _currentView.UpdateCurrentBulletsText(_bulletsInCurrentBag.Count);
        CurrentSpace[] chosenCurrentColumn = _currentView.GetCurrentColumnByColor((int)bulletToSpawn.Color);

        int spacesToMoveDown = bulletToSpawn.Number;
        int spacesMoved = 0;
        int spaceToSpawnInto;

        for (spaceToSpawnInto = 0; spaceToSpawnInto < chosenCurrentColumn.Length; spaceToSpawnInto++)
        {
            if (chosenCurrentColumn[spaceToSpawnInto].BulletProperties == null) spacesMoved++;
            if (spacesMoved >= spacesToMoveDown)
            {
                break;
            }
        }

        if(spacesMoved >= spacesToMoveDown)
        {
            _currentView.SpawnNewCurrentBulletObject(new Vector2Int(spaceToSpawnInto, (int)bulletToSpawn.Color), bulletToSpawn);
        }
        else
        {
            GameManager.Instance.ActivePlayer.ModifyCurrentHP(-1);
        }
    }

    public void RemoveBulletFromCurrent(Vector2Int removeCell)
    {
        CurrentSpace chosenSpace = _currentView.GetCurrentSpace(removeCell);
        if (chosenSpace.BulletProperties == null) return; // If there's no bullet, don't bother removing the bullet
        BulletData chosenProperty = chosenSpace.BulletProperties;
        CenterManager.Instance.AddBulletToCenter(chosenProperty);
        if(chosenProperty.IsStar) GameManager.Instance.ActivePlayer.ActionController.ActivateStarActions();
        _currentView.RemoveCurrentBulletObject(removeCell);
    }

    public void RemoveBulletsFromCurrentWithPattern(Vector2Int startingCell, PatternSpaceData[,] patternSpaceDatas)
    {
        int patternRows = patternSpaceDatas.GetLength(0);
        int patternColumns = patternSpaceDatas.GetLength(1);

        for (int i = 0; i < patternRows; i++)
        {
            for (int j = 0; j < patternColumns; j++)
            {
                if (patternSpaceDatas[i,j].WillClearBullet) // if it matches, set to correct color. otherwise, set to incorrect color
                {
                    Vector2Int currentPosition = new Vector2Int(startingCell.x + i, startingCell.y + j);
                    RemoveBulletFromCurrent(currentPosition);
                }
            }
        }
    }

    public void MoveBulletInCurrent(Vector2Int oldCell, Vector2Int newCell)
    {
        CurrentSpace chosenSpace = _currentView.GetCurrentSpace(oldCell);
        if (chosenSpace.BulletProperties == null) return; // If there's no bullet, don't bother moving the bullet
        CurrentSpace finalSpace = _currentView.GetCurrentSpace(newCell);
        BulletData chosenProperty = chosenSpace.BulletProperties;
        finalSpace.BulletProperties = chosenProperty;
        chosenSpace.BulletProperties = null;
        _currentView.MoveCurrentBulletObject(oldCell, newCell);
    }

    public void UpdateActiveSpace(Vector2Int cell)
    {
        if (!_isAcceptingHoverInputs) return; // only update if it is currently looking for hover inputs
        if (_activeSpacePosition == cell) return; // don't bother updating if it's still the same active space
        _activeSpacePosition = cell;
    }

    public void RemoveActiveSpace(Vector2Int cell)
    {
        if (!_isAcceptingHoverInputs) return; // only update if it is currently looking for hover inputs
        if (_activeSpacePosition != cell) return; // if activespaceposition is already changed, don't have to remove it again
        _activeSpacePosition.Set(-1, -1);
    }

    private void ResetAllSpaceHighlights()
    {
        CurrentSpace[,] currentGrid = _currentView.GetCurrentGrid();
        int currentRows = currentGrid.GetLength(0);
        int currentColumns = currentGrid.GetLength(1);
        for (int i = 0; i < currentRows; i++)
        {
            for (int j = 0; j < currentColumns; j++)
            {
                currentGrid[i, j].SetSpaceValidity(false, false);
            }
        }
    }

    private bool IsSpaceValidForPattern(PatternSpaceData spaceRequirement, BulletData spaceProperty)
    {
        // return false if any conditions are not met
        if (spaceRequirement.NeedsBullet && (spaceProperty == null)) return false;
        if (spaceProperty != null) // this is a wrapper to make sure no comparisons are made with properties of the null space property
        {
            if (spaceRequirement.NeedsEmpty) return false;
            if ((spaceRequirement.ColorRequired != BulletColor.Any) && (spaceProperty.Color != spaceRequirement.ColorRequired)) return false;
            if ((spaceRequirement.NumberRequired != 0) && (spaceProperty.Number != spaceRequirement.NumberRequired)) return false;
            // check same number pattern later, more complex than others
            if (spaceRequirement.NeedsStarBullet && (spaceProperty.IsStar == false)) return false;
        }

        // if all conditions are met, return true
        return true;
    }

    public void CheckSpacesToMoveIntoOrthogonal(Vector2Int startingCell, int radius, Direction[] allowedDirections, Action<bool, Vector2Int, int> callback)
    {
        CurrentSpace[,] currentGrid = _currentView.GetCurrentGrid();
        int upBoundary = 0;
        int downBoundary = currentGrid.GetLength(0) - 1;
        int leftBoundary = 0;
        int rightBoundary = currentGrid.GetLength(1) - 1;

        // clamp within the Grid's boundaries, and set the spaces to search inbase on allowed directions
        int rowsStart = Mathf.Clamp(Array.Exists(allowedDirections, direction => direction == Direction.Up) ? startingCell.x - radius : startingCell.x, upBoundary, downBoundary);
        int rowsEnd = Mathf.Clamp(Array.Exists(allowedDirections, direction => direction == Direction.Down) ? startingCell.x + radius : startingCell.x, upBoundary, downBoundary);
        int columnsStart = Mathf.Clamp(Array.Exists(allowedDirections, direction => direction == Direction.Left) ? startingCell.y - radius : startingCell.y, leftBoundary, rightBoundary);
        int columnsEnd = Mathf.Clamp(Array.Exists(allowedDirections, direction => direction == Direction.Right) ? startingCell.y + radius : startingCell.y, leftBoundary, rightBoundary);

        Dictionary<Vector2Int, int> validCellsWithDistances = new Dictionary<Vector2Int, int>();

        for (int row = rowsStart; row <= rowsEnd; row++)
        {
            for (int column = columnsStart; column <= columnsEnd; column++)
            {
                int distance = Mathf.Abs(row - startingCell.x) + Mathf.Abs(column - startingCell.y); // calculate distance from starting cell
                if (distance <= radius)
                {
                    if (currentGrid[row, column].BulletProperties == null && !((row == startingCell.x) && (column == startingCell.y))) // don't highlight spaces with bullets already in them and don't highlight starting space
                    {
                        validCellsWithDistances.Add(new Vector2Int(row, column), distance);
                        currentGrid[row, column].SetSpaceValidity(true, true);
                    }
                }
            }
        }

        StartCoroutine(SelectSpaceToMoveIntoRoutine(validCellsWithDistances, callback));
    }

    private IEnumerator SelectSpaceToMoveIntoRoutine(Dictionary<Vector2Int, int> validCellsWithDistances, Action<bool, Vector2Int, int> callback) // currently supports orthogonally adjacent squares only
    {
        while (!_shouldCancelSpaceSelection)
        {
            if (Input.GetMouseButtonDown(0)) // try to do something when the left mouse button is clicked
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);

                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.GetComponent<CurrentSpace>()) // find the first current space our mouse is over
                    {
                        Vector2Int cellPosition = result.gameObject.GetComponent<CurrentSpace>().CurrentCell;
                        if (validCellsWithDistances.ContainsKey(cellPosition))
                        {
                            ResetAllSpaceHighlights();
                            callback.Invoke(true, cellPosition, validCellsWithDistances[cellPosition]);
                            yield break;
                        }
                    }
                }
            }
            yield return null;
        }
        // Asked to cancel, callback that the function was cancelled
        ResetAllSpaceHighlights();
        _shouldCancelSpaceSelection = false;
        callback.Invoke(false, new Vector2Int(-1, -1), -1);
        yield break;
    }

    #region On Hover Functions
    public void CheckValidSpacesOnHover(PatternSpaceData[,] spaceRequirements, Action<bool, Vector2Int> callback)
    {
        _isAcceptingHoverInputs = true;

        _currentSpaceRequirements = spaceRequirements;
        StartCoroutine(SelectValidSpacesOnHoverRoutine(callback));
    }

    private IEnumerator SelectValidSpacesOnHoverRoutine(Action<bool, Vector2Int> callback) // this will constantly check and highlight spaces that are valid or invalid
    {
        while (!_shouldCancelSpaceSelection)
        {
            if (_previousActiveSpacePosition != _activeSpacePosition || Input.GetMouseButton(0))
            {
                _previousActiveSpacePosition = _activeSpacePosition;
                ResetAllSpaceHighlights(); // always blank out all highlights before starting to highlight spaces

                if ((_activeSpacePosition.x != -1) && (_activeSpacePosition.y != -1)) // no need to highlight spaces or process mouse clicks if we're not on an active space
                {
                    // our pattern's "anchor" is on our mouse, but that "anchor" corresponds to the "topleft middle" square, or in other words: hovered space - ((Ceil(length / 2) - 1)
                    // the extra -1 is needed to account for the fact arrays start from 0
                    // EX: Length 1 = 0, Length 2 = 0, Length 3 = -1, Length  4 = -1, Length 5 = -2
                    // Also, clamp the pattern to be within the current grid arrays

                    CurrentSpace[,] currentGrid = _currentView.GetCurrentGrid();

                    int patternRows = _currentSpaceRequirements.GetLength(0);
                    int patternColumns = _currentSpaceRequirements.GetLength(1);

                    int startingRow = _activeSpacePosition.x - (Mathf.CeilToInt((float)patternRows / 2) - 1);
                    int startingColumn = _activeSpacePosition.y - (Mathf.CeilToInt((float)patternColumns / 2) - 1);

                    int currentHoveredRow = Mathf.Clamp(startingRow, 0, currentGrid.GetLength(0) - patternRows);
                    int currentHoveredColumn = Mathf.Clamp(startingColumn, 0, currentGrid.GetLength(1) - patternColumns);

                    if (Input.GetMouseButtonDown(0))
                    {
                        bool isPatternValid = true;
                        for (int i = 0; i < patternRows; i++)
                        {
                            for (int j = 0; j < patternColumns; j++)
                            {
                                Vector2Int comparisonPosition = new Vector2Int(currentHoveredRow + i, currentHoveredColumn + j);
                                CurrentSpace currentSpace = _currentView.GetCurrentSpace(comparisonPosition);
                                if (!IsSpaceValidForPattern(_currentSpaceRequirements[i, j], currentSpace.BulletProperties)) // if it matches, set to correct color. otherwise, set to incorrect color
                                {
                                    isPatternValid = false;
                                    break;
                                }
                            }
                            if (!isPatternValid) break;
                        }
                        if (isPatternValid)
                        {
                            Vector2Int returnValue = new Vector2Int(currentHoveredRow, currentHoveredColumn);
                            CheckValidSpacesOnHoverCleanup();
                            callback.Invoke(true, returnValue);
                            yield break;
                        }
                    }

                    for (int i = 0; i < patternRows; i++)
                    {
                        for (int j = 0; j < patternColumns; j++)
                        {
                            Vector2Int comparisonPosition = new Vector2Int(currentHoveredRow + i, currentHoveredColumn + j);
                            CurrentSpace currentSpace = _currentView.GetCurrentSpace(comparisonPosition);
                            if (IsSpaceValidForPattern(_currentSpaceRequirements[i, j], currentSpace.BulletProperties)) // if it matches, set to correct color. otherwise, set to incorrect color
                            {
                                currentSpace.SetSpaceValidity(true, true);
                            }
                            else
                            {
                                currentSpace.SetSpaceValidity(true, false);
                            }
                        }
                    }
                }
            }
            yield return null;
        }
        // function was cancelled, do cleanup and tell the callback it was cancelled
        CheckValidSpacesOnHoverCleanup();
        callback.Invoke(false, new Vector2Int(-1, -1));
        yield break;
    }

    private void CheckValidSpacesOnHoverCleanup()
    {
        _isAcceptingHoverInputs = false;
        _previousActiveSpacePosition.Set(-1, -1);
        _activeSpacePosition.Set(-1, -1);
        _currentSpaceRequirements = null;
        _shouldCancelSpaceSelection = false;
        ResetAllSpaceHighlights();
    }

    public void CancelSpaceSelection()
    {
        _shouldCancelSpaceSelection = true;
    }
    #endregion
}
