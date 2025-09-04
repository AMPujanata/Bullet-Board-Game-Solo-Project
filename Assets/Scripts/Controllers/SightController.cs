using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SightView))]
public class SightController : MonoBehaviour
{
    [SerializeField] private SightView _sightView;
    private List<BulletData> _bulletsInCurrentBag = new List<BulletData>();
    public delegate BulletData BulletCheckPropertyModifier(BulletData data, PatternSpaceData requirement);
    public List<BulletCheckPropertyModifier> CheckPropertyModifiersList { get; private set; } = new List<BulletCheckPropertyModifier>(); 

    #region Hover Routine Variables
    private bool _isAcceptingHoverInputs = false;
    [SerializeField] private Vector2Int _previousActiveSpacePosition = new Vector2Int(-1, -1);
    [SerializeField] private Vector2Int _activeSpacePosition = new Vector2Int(-1, -1);
    private PatternSpaceData[,] _sightSpaceRequirements = null;
    private bool _shouldCancelSpaceSelection = false;
    #endregion

    public void Initialize()
    {
        _sightView.Initialize(PlaceBulletInSight);
    }

    public void DrawBulletsFromCenter(int bulletCount)
    {
        for(int i = 0;  i < bulletCount; i++)
        {
            _bulletsInCurrentBag.Add(CenterManager.Instance.GetRandomBulletFromCenter());
        }
        _sightView.UpdateCurrentBulletsText(_bulletsInCurrentBag.Count);
    }

    private void PlaceBulletInSight()
    {
        if(_bulletsInCurrentBag.Count <= 0)
        {
            Vector2 popupLocation = Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f));
            OverlayManager.Instance.DisplayPopup("Are you sure you want to enter the end phase?", popupLocation, "Yes", GameManager.Instance.BeginEndPhase, "No");
            return;
        }

        BulletData bulletToSpawn = _bulletsInCurrentBag[UnityEngine.Random.Range(0, _bulletsInCurrentBag.Count - 1)];
        _bulletsInCurrentBag.Remove(bulletToSpawn);
        _sightView.UpdateCurrentBulletsText(_bulletsInCurrentBag.Count);
        SightSpace[] chosenSightColumn = _sightView.GetSightColumnByColor((int)bulletToSpawn.Color);

        int spacesToMoveDown = bulletToSpawn.Number;
        int spacesMoved = 0;
        int spaceToSpawnInto;

        for (spaceToSpawnInto = 0; spaceToSpawnInto < chosenSightColumn.Length; spaceToSpawnInto++)
        {
            if (chosenSightColumn[spaceToSpawnInto].BulletProperties == null) spacesMoved++;
            if (spacesMoved >= spacesToMoveDown)
            {
                break;
            }
        }

        if(spacesMoved >= spacesToMoveDown)
        {
            _sightView.SpawnNewSightBulletObject(new Vector2Int(spaceToSpawnInto, (int)bulletToSpawn.Color), bulletToSpawn);
        }
        else
        {
            GameManager.Instance.ActivePlayer.ModifyCurrentHP(-1);
        }
    }

    public void RemoveBulletFromSight(Vector2Int removeCell, bool isDamage = false)
    {
        SightSpace chosenSpace = _sightView.GetSightSpace(removeCell);
        if (chosenSpace.BulletProperties == null) return; // If there's no bullet, don't bother removing the bullet
        BulletData chosenProperty = chosenSpace.BulletProperties;
        chosenProperty.IsFacedown = false; // always flip bullets face up before returning
        if (!isDamage)
        {
            if (GameManager.Instance.CurrentMode == GameMode.ScoreAttack)
            {
                CenterManager.Instance.AddBulletToIntensity(chosenProperty);
                UpdateCurrentIntensity(GameManager.Instance.CurrentIntensity, CenterManager.Instance.GetNumberOfBulletsInIntensity());
            }
            else if (GameManager.Instance.CurrentMode == GameMode.BossBattle)
            {
                GameManager.Instance.ActiveBoss.AddBulletToBossIncoming(chosenProperty);
            }
            if (chosenProperty.IsStar) GameManager.Instance.ActivePlayer.ActionController.ActivateStarActions();
            _sightView.RemoveSightBulletObject(removeCell);
            GameManager.Instance.AddBulletToTotalClear();
        }
        else
        {
            _sightView.RemoveSightBulletObject(removeCell);
            GameManager.Instance.ActivePlayer.ModifyCurrentHP(-1);
        }
    }

    public void RemoveBulletsFromSightWithPattern(Vector2Int startingCell, PatternSpaceData[,] patternSpaceDatas)
    {
        int patternRows = patternSpaceDatas.GetLength(0);
        int patternColumns = patternSpaceDatas.GetLength(1);

        for (int i = 0; i < patternRows; i++)
        {
            for (int j = 0; j < patternColumns; j++)
            {
                if (patternSpaceDatas[i,j].WillClearBullet) // if it matches, set to correct color. otherwise, set to incorrect color
                {
                    Vector2Int sightPosition = new Vector2Int(startingCell.x + i, startingCell.y + j);
                    RemoveBulletFromSight(sightPosition);
                }
            }
        }
    }

    public void MoveBulletInSight(Vector2Int oldCell, Vector2Int newCell) // newCell possibly asking for a space that doesn't exist; verify this
    {
        Vector2Int sightGridSize = new Vector2Int(GetSightGrid().GetLength(0), GetSightGrid().GetLength(1));
        if (oldCell.x < 0 || oldCell.x > sightGridSize.x - 1 || oldCell.y < 0 || oldCell.y > sightGridSize.y - 1) return; // don't go on if old cell is out of bounds
        SightSpace chosenSpace = _sightView.GetSightSpace(oldCell);
        if (chosenSpace.BulletProperties == null) return; // If there's no bullet, don't bother moving the bullet

        if(newCell.x > _sightView.GetSightGrid().GetLength(0) - 1) // if the y would move past the bottom of the grid, cause damage
        {
            RemoveBulletFromSight(oldCell, true);
            return;
        }
        newCell.x = Mathf.Clamp(newCell.x, 0, sightGridSize.x - 1);
        newCell.y = Mathf.Clamp(newCell.y, 0, sightGridSize.y - 1);
        SightSpace finalSpace = _sightView.GetSightSpace(newCell);
        if (finalSpace.BulletProperties != null) return; // If there's a bullet there already, don't move the bullet

        BulletData chosenProperty = chosenSpace.BulletProperties;
        finalSpace.BulletProperties = chosenProperty;
        chosenSpace.BulletProperties = null;
        _sightView.MoveSightBulletObject(oldCell, newCell);
    }

    public void FlipBulletFaceDown(Vector2Int cell)
    {
        SightSpace chosenSpace = _sightView.GetSightSpace(cell);
        if (chosenSpace.BulletProperties == null) return; // If there's no bullet you can't flip it face down.
        chosenSpace.BulletProperties.IsFacedown = true;
        _sightView.ModifySightBulletProperty(cell, chosenSpace.BulletProperties);
    }

    public void UpdateCurrentIntensity(int currentIntensity, int extraBulletsNextRound = -1)
    {
        _sightView.UpdateCurrentIntensityText(currentIntensity, extraBulletsNextRound);
    }

    #region Highlighted Space Functions
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
        SightSpace[,] sightGrid = _sightView.GetSightGrid();
        int sightRows = sightGrid.GetLength(0);
        int sightColumns = sightGrid.GetLength(1);
        for (int i = 0; i < sightRows; i++)
        {
            for (int j = 0; j < sightColumns; j++)
            {
                sightGrid[i, j].SetSpaceValidity(false, false);
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

            BulletData bulletToCheck = new BulletData // duplicate bulletdata so the original space is not affected
            {
                Color = spaceProperty.Color,
                Number = spaceProperty.Number,
                IsStar = spaceProperty.IsStar,
                IsFacedown = spaceProperty.IsFacedown
            };

            for (int i = 0; i < CheckPropertyModifiersList.Count; i++) // activate any passives that modify bullet requirements. bulletToCheck will be a different 
            {
                bulletToCheck = CheckPropertyModifiersList[i].Invoke(bulletToCheck, spaceRequirement);
            }

            if (spaceRequirement.ColorRequired != BulletColor.Any && bulletToCheck.Color != spaceRequirement.ColorRequired) return false;
            if ((spaceRequirement.NumberRequired != 0) && (bulletToCheck.Number != spaceRequirement.NumberRequired)) return false;
            // same number pattern currently being checked in the wrapping functions themself (hover)
            if (spaceRequirement.NeedsStarBullet && (bulletToCheck.IsStar == false)) return false;
        }

        // if all conditions are met, return true
        return true;
    }
    #endregion

    #region Check Spaces Functions
    public void CheckSpacesToMoveIntoOrthogonal(Vector2Int startingCell, int radius, Direction[] allowedDirections, Action<bool, Vector2Int, int> callback)
    {
        SightSpace[,] sightGrid = _sightView.GetSightGrid();
        int upBoundary = 0;
        int downBoundary = sightGrid.GetLength(0) - 1;
        int leftBoundary = 0;
        int rightBoundary = sightGrid.GetLength(1) - 1;

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
                    if (sightGrid[row, column].BulletProperties == null && !((row == startingCell.x) && (column == startingCell.y))) // don't highlight spaces with bullets already in them and don't highlight starting space
                    {
                        validCellsWithDistances.Add(new Vector2Int(row, column), distance);
                        sightGrid[row, column].SetSpaceValidity(true, true);
                    }
                }
            }
        }

        StartCoroutine(SelectSpaceToMoveIntoRoutine(validCellsWithDistances, callback));
    }

    public void CheckSpacesToMoveIntoDiagonal(Vector2Int startingCell, int radius, Action<bool, Vector2Int, int> callback) // currently always assumes all diagonals are allowed
    {
        SightSpace[,] sightGrid = _sightView.GetSightGrid();

        int upBoundary = 0;
        int downBoundary = sightGrid.GetLength(0) - 1;
        int leftBoundary = 0;
        int rightBoundary = sightGrid.GetLength(1) - 1;

        int rowsStart = Mathf.Clamp(startingCell.x - radius, upBoundary, downBoundary);
        int rowsEnd = Mathf.Clamp(startingCell.x + radius, upBoundary, downBoundary);
        int columnsStart = Mathf.Clamp(startingCell.y - radius, leftBoundary, rightBoundary);
        int columnsEnd = Mathf.Clamp(startingCell.y + radius, leftBoundary, rightBoundary);

        Dictionary<Vector2Int, int> validCellsWithDistances = new Dictionary<Vector2Int, int>();
        for (int row = rowsStart; row <= rowsEnd; row++)
        {
            for(int column = columnsStart; column <= columnsEnd; column++)
            {
                int horizontalDistance = Mathf.Abs(column - startingCell.y);
                int verticalDistance = Mathf.Abs(row - startingCell.x);
                if(sightGrid[row, column].BulletProperties == null && (Mathf.Abs(horizontalDistance - verticalDistance) % 2) == 0 && ((startingCell.x != row) || (startingCell.y != column))) // using a mathematical algorithm, all grids with a total distance that is even are valid. Except the starting space
                {
                    validCellsWithDistances.Add(new Vector2Int(row, column), Mathf.Max(horizontalDistance, verticalDistance)); // and the actual distance is equal to the higher one out of horizontal and vertical distance
                    sightGrid[row, column].SetSpaceValidity(true, true);
                }
            }
        }

        StartCoroutine(SelectSpaceToMoveIntoRoutine(validCellsWithDistances, callback));
    }

    private IEnumerator SelectSpaceToMoveIntoRoutine(Dictionary<Vector2Int, int> validCellsWithDistances, Action<bool, Vector2Int, int> callback) // currently supports orthogonally adjacent squares only
    {
        while (!_shouldCancelSpaceSelection)
        {
            yield return null;
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
                    if (result.gameObject.GetComponent<SightSpace>()) // find the first sight space our mouse is over
                    {
                        Vector2Int cellPosition = result.gameObject.GetComponent<SightSpace>().SightCell;
                        if (validCellsWithDistances.ContainsKey(cellPosition))
                        {
                            ResetAllSpaceHighlights();
                            callback.Invoke(true, cellPosition, validCellsWithDistances[cellPosition]);
                            yield break;
                        }
                    }
                }
            }
        }
        // Asked to cancel, callback that the function was cancelled
        ResetAllSpaceHighlights();
        _shouldCancelSpaceSelection = false;
        callback.Invoke(false, new Vector2Int(-1, -1), -1);
        yield break;
    }
    #endregion

    #region On Hover Functions
    public void CheckValidSpacesOnHover(PatternSpaceData[,] spaceRequirements, Action<bool, Vector2Int> callback)
    {
        _isAcceptingHoverInputs = true;

        _sightSpaceRequirements = spaceRequirements;
        StartCoroutine(SelectValidSpacesOnHoverRoutine(callback));
    }

    private IEnumerator SelectValidSpacesOnHoverRoutine(Action<bool, Vector2Int> callback) // this will constantly check and highlight spaces that are valid or invalid
    {
        while (!_shouldCancelSpaceSelection)
        {
            yield return null;
            if (_previousActiveSpacePosition != _activeSpacePosition || Input.GetMouseButton(0))
            {
                _previousActiveSpacePosition = _activeSpacePosition;
                ResetAllSpaceHighlights(); // always blank out all highlights before starting to highlight spaces

                if ((_activeSpacePosition.x != -1) && (_activeSpacePosition.y != -1)) // no need to highlight spaces or process mouse clicks if we're not on an active space
                {
                    // our pattern's "anchor" is on our mouse, but that "anchor" corresponds to the "topleft middle" square, or in other words: hovered space - ((Ceil(length / 2) - 1)
                    // the extra -1 is needed to account for the fact arrays start from 0
                    // EX: Length 1 = 0, Length 2 = 0, Length 3 = -1, Length  4 = -1, Length 5 = -2
                    // Also, clamp the pattern to be within the sight grid arrays

                    SightSpace[,] sightGrid = _sightView.GetSightGrid();

                    int patternRows = _sightSpaceRequirements.GetLength(0);
                    int patternColumns = _sightSpaceRequirements.GetLength(1);

                    int startingRow = _activeSpacePosition.x - (Mathf.CeilToInt((float)patternRows / 2) - 1);
                    int startingColumn = _activeSpacePosition.y - (Mathf.CeilToInt((float)patternColumns / 2) - 1);

                    int sightHoveredRow = Mathf.Clamp(startingRow, 0, sightGrid.GetLength(0) - patternRows);
                    int sightHoveredColumn = Mathf.Clamp(startingColumn, 0, sightGrid.GetLength(1) - patternColumns);

                    List<Vector2Int> needsSameNumberCells = new List<Vector2Int>();
                    int sameNumberRequirement = -1;
                    if (Input.GetMouseButtonDown(0))
                    {
                        bool isPatternValid = true;

                        for (int i = 0; i < patternRows; i++)
                        {
                            for (int j = 0; j < patternColumns; j++)
                            {
                                Vector2Int comparisonPosition = new Vector2Int(sightHoveredRow + i, sightHoveredColumn + j);
                                if (_sightSpaceRequirements[i, j].NeedsSameNumber)
                                {
                                    needsSameNumberCells.Add(comparisonPosition);
                                    continue;
                                }
                                SightSpace sightSpace = _sightView.GetSightSpace(comparisonPosition);
                                if (!IsSpaceValidForPattern(_sightSpaceRequirements[i, j], sightSpace.BulletProperties))
                                {
                                    isPatternValid = false;
                                    break;
                                }
                            }
                            if (!isPatternValid) break;
                        }

                        if(needsSameNumberCells.Count > 0)
                        {
                            foreach(Vector2Int cell in needsSameNumberCells)
                            {
                                Vector2Int comparisonPosition = new Vector2Int(cell.x, cell.y);
                                SightSpace sightSpace = _sightView.GetSightSpace(comparisonPosition);
                                if (sightSpace.BulletProperties != null) // make sure not to check properties if space is empty
                                {
                                    if(sameNumberRequirement == -1)
                                    {
                                        sameNumberRequirement = sightSpace.BulletProperties.Number;
                                    }
                                    else if(sightSpace.BulletProperties.Number != sameNumberRequirement)
                                    {
                                        isPatternValid = false;
                                        break;
                                    }
                                }
                                if (!IsSpaceValidForPattern(_sightSpaceRequirements[cell.x - sightHoveredRow, cell.y - sightHoveredColumn], sightSpace.BulletProperties))
                                {
                                    isPatternValid = false;
                                    break;
                                }
                            }
                        }

                        if (isPatternValid)
                        {
                            Vector2Int returnValue = new Vector2Int(sightHoveredRow, sightHoveredColumn);
                            CheckValidSpacesOnHoverCleanup();
                            callback.Invoke(true, returnValue);
                            yield break;
                        }
                    }

                    needsSameNumberCells.Clear();

                    for (int i = 0; i < patternRows; i++)
                    {
                        for (int j = 0; j < patternColumns; j++)
                        {
                            Vector2Int comparisonPosition = new Vector2Int(sightHoveredRow + i, sightHoveredColumn + j);
                            if (_sightSpaceRequirements[i, j].NeedsSameNumber)
                            {
                                needsSameNumberCells.Add(comparisonPosition);
                                continue;
                            }
                            SightSpace sightSpace = _sightView.GetSightSpace(comparisonPosition);
                            if (IsSpaceValidForPattern(_sightSpaceRequirements[i, j], sightSpace.BulletProperties)) // if it matches, set to correct color. otherwise, set to incorrect color
                            {
                                sightSpace.SetSpaceValidity(true, true);
                            }
                            else
                            {
                                sightSpace.SetSpaceValidity(true, false);
                            }
                        }
                    }

                    sameNumberRequirement = -1;
                    bool hasSameNumbers = true;

                    if(needsSameNumberCells.Count > 0)
                    {
                        foreach (Vector2Int cell in needsSameNumberCells) // check if pattern is valid or not
                        {
                            Vector2Int comparisonPosition = new Vector2Int(cell.x, cell.y);
                            SightSpace sightSpace = _sightView.GetSightSpace(comparisonPosition);
                            if (sightSpace.BulletProperties != null) // make sure not to check properties if space is empty
                            {
                                if (sameNumberRequirement == -1)
                                {
                                    sameNumberRequirement = sightSpace.BulletProperties.Number;
                                }
                                else if (sightSpace.BulletProperties.Number != sameNumberRequirement)
                                {
                                    hasSameNumbers = false;
                                    break;
                                }
                            }
                            if (!IsSpaceValidForPattern(_sightSpaceRequirements[cell.x - sightHoveredRow, cell.y - sightHoveredColumn], sightSpace.BulletProperties))
                            {
                                hasSameNumbers = false;
                                break;
                            }
                        }

                        foreach (Vector2Int cell in needsSameNumberCells) // then, actually set each space's highlight visibility
                        {
                            _sightView.GetSightSpace(cell).SetSpaceValidity(true, hasSameNumbers);
                        }
                    }
                }
            }
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
        _sightSpaceRequirements = null;
        _shouldCancelSpaceSelection = false;
        ResetAllSpaceHighlights();
    }

    public void CancelSpaceSelection()
    {
        _shouldCancelSpaceSelection = true;
    }
    #endregion

    public SightSpace[,] GetSightGrid()
    {
        return _sightView.GetSightGrid();
    }
}
