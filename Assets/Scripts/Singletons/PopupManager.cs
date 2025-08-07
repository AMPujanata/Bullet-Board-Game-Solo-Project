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

    [SerializeField] private CanvasGroup mainCanvasGroup;
    [SerializeField] private Canvas notificationCanvas;
    [SerializeField] private GameObject popupPrefab;
    private GameObject currentPopup;

    public void DisplayPopup(string popupString, string popupButtonString, Vector3 popupLocation, float fadeDuration = 0f,  Action onButtonPress = null)
    {
        if (currentPopup) // if there is already an existing popup
        {
            Debug.Log("Already existing popup! Returning!");
            return; // don't make a new popup
        }

        currentPopup = Instantiate(popupPrefab, notificationCanvas.transform);
        currentPopup.GetComponent<RectTransform>().position = popupLocation;
        currentPopup.GetComponent<PopupView>().Initialize(popupString, popupButtonString, fadeDuration, onButtonPress);
        mainCanvasGroup.interactable = false;
    }

    public void ClosePopup()
    {
        mainCanvasGroup.interactable = true;
        Destroy(currentPopup);
    }
}
