using UnityEngine;
using System;
using UnityEngine.UI;

public class SightView : MonoBehaviour
{
    [Serializable]
    private class SightRow
    {
        public SightSpace[] SightSpaces;
    }
    [SerializeField] private TMPro.TMP_Text _sendBulletText;
    [SerializeField] private TMPro.TMP_Text _currentIntensityText;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Button _placeBulletButton;
    [SerializeField] private BulletColorUIProperty[] _bulletColorUIProperties;
    [SerializeField] private SightRow[] _sightGrid;

    public void Initialize(Action placeBulletButtonAction)
    {
        int numberOfRows = _sightGrid.Length;
        int numberOfColumns = _sightGrid[0].SightSpaces.Length;

        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                _sightGrid[i].SightSpaces[j].Initialize(new Vector2Int(i, j));
            }
        }

        _placeBulletButton.onClick.AddListener(() => placeBulletButtonAction());
    }

    public SightSpace[,] GetSightGrid()
    {
        int numberOfRows = _sightGrid.Length;
        int numberOfColumns = _sightGrid[0].SightSpaces.Length;
        SightSpace[,] returnGrid = new SightSpace[numberOfRows, numberOfColumns];

        for(int i = 0; i < numberOfRows; i++)
        {
            for(int j = 0; j < numberOfColumns; j++)
            {
                returnGrid[i, j] = _sightGrid[i].SightSpaces[j];
            }
        }

        return returnGrid;
    }

    public SightSpace[] GetSightColumnByColor(int requestedColumn)
    {
        SightSpace[] returnColumn = new SightSpace[_sightGrid.Length];

        for(int i = 0; i < returnColumn.Length; i++)
        {
            returnColumn[i] = _sightGrid[i].SightSpaces[requestedColumn];
        }

        return returnColumn;
    }

    public SightSpace GetSightSpace(Vector2Int cell)
    {
        SightSpace returnSpace = _sightGrid[cell.x].SightSpaces[cell.y];
        return returnSpace;
    }

    public void SpawnNewSightBulletObject(Vector2Int cell, BulletData bulletData)
    {
        SightSpace selectedSpace = _sightGrid[cell.x].SightSpaces[cell.y];
        selectedSpace.BulletProperties = bulletData;

        GameObject newBulletObject = Instantiate(_bulletPrefab, selectedSpace.BulletParent);
        BulletColorUIProperty chosenColorUIProperty = Array.Find(_bulletColorUIProperties, property => property.BulletColorRequirement == selectedSpace.BulletProperties.Color);
        newBulletObject.GetComponent<BulletView>().Initialize(bulletData, chosenColorUIProperty);
    }

    public void ModifySightBulletProperty(Vector2Int cell, BulletData newData)
    {
        SightSpace selectedSpace = _sightGrid[cell.x].SightSpaces[cell.y];
        selectedSpace.BulletProperties = newData;

        GameObject bulletObject = selectedSpace.BulletParent.GetComponentInChildren<BulletView>().gameObject;
        BulletColorUIProperty chosenColorUIProperty = Array.Find(_bulletColorUIProperties, property => property.BulletColorRequirement == selectedSpace.BulletProperties.Color);
        bulletObject.GetComponent<BulletView>().Initialize(newData, chosenColorUIProperty);
    }

    public void RemoveSightBulletObject(Vector2Int cell)
    {
        SightSpace selectedSpace = _sightGrid[cell.x].SightSpaces[cell.y];
        selectedSpace.BulletProperties = null;

        Destroy(selectedSpace.BulletParent.GetChild(0).gameObject);
    }

    public void MoveSightBulletObject(Vector2Int oldCell, Vector2Int newCell)
    {
        SightSpace oldSpace = _sightGrid[oldCell.x].SightSpaces[oldCell.y];
        SightSpace newSpace = _sightGrid[newCell.x].SightSpaces[newCell.y];
        GameObject bulletObjectToMove = oldSpace.BulletParent.GetChild(0).gameObject;
        bulletObjectToMove.transform.SetParent(newSpace.BulletParent);
        bulletObjectToMove.transform.position = newSpace.BulletParent.transform.position;
    }

    public void UpdateCurrentBulletsText(int currentCount)
    {
        if(currentCount > 0)
        {
            _sendBulletText.text = "Place Bullet In Sight\n(" + currentCount + " bullets remaining)";
        }
        else
        {
            _sendBulletText.text = "Begin End Phase";
        }
    }

    public void UpdateCurrentIntensityText(int currentIntensity, int extraBulletsNextRound = -1)
    {
        if(extraBulletsNextRound >= 0)
        {
            _currentIntensityText.text = currentIntensity + " Intensity\n(+" + extraBulletsNextRound + " Bullets next round)";
        }
        else
        {
            _currentIntensityText.text = currentIntensity + " Intensity";
        }
    }
}
