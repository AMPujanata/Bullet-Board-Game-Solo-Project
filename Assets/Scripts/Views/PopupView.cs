using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PopupView : MonoBehaviour
{
    [SerializeField] private TMP_Text _popupText;
    [SerializeField] private Button _popupButton;
    [SerializeField] private TMP_Text _popupButtonText;
    public void Initialize(string popupString, string popupButtonString, Action onButtonPress = null)
    {
        _popupText.text = popupString;
        _popupButtonText.text = popupButtonString;

        if(onButtonPress != null)
        {
            _popupButton.onClick.RemoveAllListeners();
            _popupButton.onClick.AddListener(() => onButtonPress());
        }

        _popupButton.onClick.AddListener(() =>
        {
            PopupManager.Instance.ClosePopup();
        });
    }
}
