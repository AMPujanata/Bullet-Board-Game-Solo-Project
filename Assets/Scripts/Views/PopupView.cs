using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PopupView : MonoBehaviour
{
    [SerializeField] private TMP_Text _popupText;
    [SerializeField] private Button _popupButton1;
    [SerializeField] private TMP_Text _popupButton1Text;
    [SerializeField] private Button _popupButton2;
    [SerializeField] private TMP_Text _popupButton2Text;
    public void Initialize(string popupString, string popupButton1String, Action onButton1Press = null, string popupButton2String = null, Action onButton2Press = null)
    {
        _popupText.text = popupString;
        _popupButton1Text.text = popupButton1String;

        if(onButton1Press != null)
        {
            _popupButton1.onClick.RemoveAllListeners();
            _popupButton1.onClick.AddListener(() => onButton1Press());
        }

        _popupButton1.onClick.AddListener(() =>
        {
            PopupManager.Instance.ClosePopup();
        });

        if(popupButton2String != null) // only show the second button if necessary
        {
            _popupButton2.gameObject.SetActive(true);
            _popupButton2Text.text = popupButton2String;

            if (onButton2Press != null)
            {
                _popupButton2.onClick.RemoveAllListeners();
                _popupButton2.onClick.AddListener(() => onButton2Press());
            }

            _popupButton2.onClick.AddListener(() =>
            {
                PopupManager.Instance.ClosePopup();
            });

        }
        else
        {
            _popupButton2.gameObject.SetActive(false);
        }
    }
}
