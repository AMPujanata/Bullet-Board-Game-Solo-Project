using System;
using UnityEngine;

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
        DontDestroyOnLoad(gameObject);
    }

    private CanvasGroup _mainCanvasGroup;
    public CanvasGroup MainCanvasGroup { get
        {
            if (_mainCanvasGroup == null)
            {
                _mainCanvasGroup = FindObjectOfType<CanvasGroup>(true);
            }
            return _mainCanvasGroup;
        }
    }
    [SerializeField] private Canvas _notificationCanvas;
    [SerializeField] private GameObject _popupPrefab;
    private GameObject _currentPopup;

    public void DisplayPopup(string popupString, Vector3 popupLocation, string popupButton1String, Action onButton1Press = null, string popupButton2String = null, Action onButton2Press = null)
    {
        if(_notificationCanvas.worldCamera == null) // if the render camera is gone (from switching scenes), reassign it to the new main camera
        {
            _notificationCanvas.worldCamera = Camera.main;
        }

        if(!_currentPopup) _currentPopup = Instantiate(_popupPrefab, popupLocation, Quaternion.identity, _notificationCanvas.transform);
        _currentPopup.gameObject.SetActive(true);
        _currentPopup.GetComponent<PopupView>().Initialize(popupString, popupButton1String, onButton1Press, popupButton2String, onButton2Press);
        MainCanvasGroup.interactable = false;
    }

    public void ClosePopup()
    {
        MainCanvasGroup.interactable = true;
        _currentPopup.gameObject.SetActive(false);
        _currentPopup = null;
    }
}
