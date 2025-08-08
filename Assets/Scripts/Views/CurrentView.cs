using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class CurrentView : MonoBehaviour
{
    [Serializable]
    private class CurrentRow
    {
        public CurrentSpace[] CurrentSpaces;
    }

    [SerializeField] private GameObject _bulletPrefab;

    [SerializeField] private BulletColorUIProperty[] _bulletColorUIProperties;
    [SerializeField] private CurrentRow[] _currentGrid;

    public CurrentSpace[,] GetCurrentGrid()
    {
        int numberOfRows = _currentGrid.Length;
        int numberOfColumns = _currentGrid[0].CurrentSpaces.Length;
        CurrentSpace[,] returnGrid = new CurrentSpace[numberOfRows, numberOfColumns];

        for(int i = 0; i < numberOfRows; i++)
        {
            for(int j = 0; j < numberOfColumns; j++)
            {
                returnGrid[i, j] = _currentGrid[i].CurrentSpaces[j];
            }
        }

        return returnGrid;
    }

    public CurrentSpace[] GetCurrentColumnByColor(int requestedColumn)
    {
        CurrentSpace[] returnColumn = new CurrentSpace[_currentGrid.Length];

        for(int i = 0; i < returnColumn.Length; i++)
        {
            returnColumn[i] = _currentGrid[i].CurrentSpaces[requestedColumn];
        }

        return returnColumn;
    }

    public void SpawnNewCurrentBulletObject(int row, int column, BulletData bulletData)
    {
        CurrentSpace selectedSpace = _currentGrid[row].CurrentSpaces[column];
        selectedSpace.BulletProperties = bulletData;

        GameObject newBulletObject = Instantiate(_bulletPrefab, selectedSpace.gameObject.transform);
        BulletColorUIProperty chosenColorUIProperty = Array.Find(_bulletColorUIProperties, property => property.BulletColorRequirement == selectedSpace.BulletProperties.Color);
        newBulletObject.GetComponent<BulletView>().Initialize(bulletData, chosenColorUIProperty);
    }
}
