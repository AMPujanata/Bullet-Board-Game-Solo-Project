using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this); // Make sure there's only ever one 
            return; // do NOT run any other code
        }
    }

    [SerializeField] private CanvasGroup _mainCanvasGroup;
    [SerializeField] private Canvas _notificationCanvas;
    [SerializeField] private GameObject _popupPrefab;
    private GameObject _currentPopup;

    public void DisplayPopup(string popupString, string popupButtonString, Vector3 popupLocation, Action onButtonPress = null)
    {
        if (_currentPopup) // if there is already an existing popup
        {
            Debug.Log("Already existing popup! Returning!");
            return; // don't make a new popup
        }

        _currentPopup = Instantiate(_popupPrefab, popupLocation, Quaternion.identity, _notificationCanvas.transform);
        _currentPopup.GetComponent<PopupView>().Initialize(popupString, popupButtonString, onButtonPress);
        _mainCanvasGroup.interactable = false;
    }

    public void ClosePopup()
    {
        _mainCanvasGroup.interactable = true;
        Destroy(_currentPopup);
        _currentPopup = null;
    }
}
