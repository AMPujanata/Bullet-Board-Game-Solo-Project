using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CurrentView))]
public class CurrentController : MonoBehaviour
{
    [SerializeField] private CurrentView _currentView;

    #region Hover Routine Variables
    private bool _isAcceptingHoverInputs = false;
    private Vector2Int _previousActiveSpacePosition = new Vector2Int(-1, -1);
    private Vector2Int _activeSpacePosition = new Vector2Int(-1, -1);
    private PatternSpaceData _currentSpaceRequirement = null;
    private bool _shouldCancelHoverRoutine = false;
    #endregion

    public void SendBulletToCurrent()
    {
        BulletData bulletToSpawn = CenterManager.Instance.TakeRandomBulletFromCenter();
        CurrentSpace[] chosenCurrentColumn = _currentView.GetCurrentColumnByColor((int)bulletToSpawn.Color);

        int spacesToMoveDown = bulletToSpawn.Number;
        int spacesMoved = 0;
        int spaceToSpawnInto;

        for (spaceToSpawnInto = 0; spaceToSpawnInto < chosenCurrentColumn.Length; spaceToSpawnInto++)
        {
            if (chosenCurrentColumn[spaceToSpawnInto].BulletProperties == null) spacesMoved++;
            if (spacesMoved >= spacesToMoveDown)
            {
                Debug.Log("Space found in time!");
                break;
            }
        }

        if(spacesMoved >= spacesToMoveDown)
        {
            _currentView.SpawnNewCurrentBulletObject(new Vector2Int(spaceToSpawnInto, (int)bulletToSpawn.Color), bulletToSpawn);
        }
        else
        {
            Debug.Log("No space remaining! Life lost!");
            GameManager.Instance.ActivePlayer.ModifyCurrentHP(-1);
        }
    }

    public void RemoveBulletFromCurrent(Vector2Int removeCell)
    {
        CurrentSpace chosenSpace = _currentView.GetCurrentSpace(removeCell);
        BulletData chosenProperty = chosenSpace.BulletProperties;
        // do something with the bullet properties later, esp. if it's star
        CenterManager.Instance.AddBulletToCenter(chosenProperty);
        _currentView.RemoveCurrentBulletObject(removeCell);

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
        if (spaceRequirement.NeedsEmpty && (spaceProperty != null)) return false;
        if ((spaceRequirement.ColorRequired != BulletColor.Any) && (spaceProperty.Color != spaceRequirement.ColorRequired)) return false;
        if ((spaceRequirement.NumberRequired != -1) && (spaceProperty.Number != spaceRequirement.NumberRequired)) return false;
        // check same number pattern later, more complex than others
        if (spaceRequirement.NeedsStarBullet && (spaceProperty.IsStar == false)) return false;

        // if all conditions are met, return true
        return true;
    }
    #region On Hover Functions
    public void CheckValidSpacesOnHover(PatternSpaceData[,] spaceRequirements, System.Action<bool, Vector2Int> callback)
    {
        _isAcceptingHoverInputs = true;

        // for now, use only a single space for the debug action
        PatternSpaceData spaceRequirement = spaceRequirements[0,0];
        _currentSpaceRequirement = spaceRequirement;
        StartCoroutine(CheckValidSpacesOnHoverRoutine(callback));
    }

    private IEnumerator CheckValidSpacesOnHoverRoutine(System.Action<bool, Vector2Int> callback) // this will constantly check and highlight spaces that are valid or invalid
    {
        while (!_shouldCancelHoverRoutine)
        {
            BulletData activeSpaceData;

            if (_previousActiveSpacePosition != _activeSpacePosition) // only update the "validity" of spaces if there is a change in the active space
            {
                _previousActiveSpacePosition = _activeSpacePosition;

                ResetAllSpaceHighlights(); // always blank out all highlights before starting to highlight spaces
                if ((_activeSpacePosition.x != -1) && (_activeSpacePosition.y != -1))
                {
                    CurrentSpace currentSpace = _currentView.GetCurrentSpace(_activeSpacePosition);
                    activeSpaceData = currentSpace.BulletProperties;

                    if (IsSpaceValidForPattern(_currentSpaceRequirement, activeSpaceData)) // if it matches, set to correct color. otherwise, set to incorrect color
                    {
                        currentSpace.SetSpaceValidity(true, true);
                    }
                    else
                    {
                        currentSpace.SetSpaceValidity(true, false);
                    }
                }
            }

            if (Input.GetMouseButtonDown(0)) // try to do something when the left mouse button is clicked
            {
                if ((_activeSpacePosition.x != -1) && (_activeSpacePosition.y != -1))
                {
                    activeSpaceData = _currentView.GetCurrentSpace(_activeSpacePosition).BulletProperties;
                    if (IsSpaceValidForPattern(_currentSpaceRequirement, activeSpaceData))
                    {
                        callback.Invoke(true, _activeSpacePosition);
                        CheckValidSpacesOnHoverCleanup();
                        yield break;
                    }
                }
            }
            yield return null;
        }
        // function was cancelled, do cleanup and tell the callback it was cancelled
        callback.Invoke(false, new Vector2Int(-1, -1));
        CheckValidSpacesOnHoverCleanup();
        yield break;
    }

    private void CheckValidSpacesOnHoverCleanup()
    {
        _isAcceptingHoverInputs = false;
        _previousActiveSpacePosition.Set(-1, -1);
        _activeSpacePosition.Set(-1, -1);
        _currentSpaceRequirement = null;
        _shouldCancelHoverRoutine = false;
        ResetAllSpaceHighlights();
    }

    public void CancelCheckValidSpacesOnHover()
    {
        _shouldCancelHoverRoutine = true;
    }
    #endregion
}
