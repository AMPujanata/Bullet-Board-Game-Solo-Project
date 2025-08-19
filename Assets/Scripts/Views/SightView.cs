using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class SightView : MonoBehaviour
{
    [Serializable]
    private class SightRow
    {
        public SightSpace[] SightSpaces;
    }
    [SerializeField] private TMPro.TMP_Text _sendBulletText;
    [SerializeField] private GameObject _bulletPrefab;

    [SerializeField] private BulletColorUIProperty[] _bulletColorUIProperties;
    [SerializeField] private SightRow[] _sightGrid;

    private void Awake()
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
            _sendBulletText.text = "Place Bullet In Sight\n(" + currentCount + " bullets remaining";
        }
        else
        {
            _sendBulletText.text = "Begin End Phase";
        }
    }
}
