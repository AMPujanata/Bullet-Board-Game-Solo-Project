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

    [SerializeField] private GameObject popupPrefab;
    private GameObject currentPopup;
    [SerializeField] private Canvas notificationCanvas;

    public void DisplayPopup(string popupString, string popupButtonString, Vector3 popupLocation, float fadeDuration = 0f,  Action onButtonPress = null)
    {
        if (currentPopup) // if there is already an existing popup
        {
            Debug.Log("Already existing popup! Returning!");
            return; // don't make a new popup
        }

        GameObject newPopup = Instantiate(popupPrefab, notificationCanvas.transform);
        newPopup.GetComponent<RectTransform>().position = popupLocation;
        newPopup.GetComponent<PopupView>().Initialize(popupString, popupButtonString, fadeDuration, onButtonPress);
    }
}
