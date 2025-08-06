using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PopupView : MonoBehaviour
{
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private Button popupButton;
    [SerializeField] private TMP_Text popupButtonText;

    public void Initialize(string popupString, string popupButtonString, float fadeDuration = 0f, Action onButtonPress = null)
    {
        popupText.text = popupString;
        popupButtonText.text = popupButtonString;
        if(onButtonPress != null)
        {
            popupButton.onClick.RemoveAllListeners();
            popupButton.onClick.AddListener(() => onButtonPress());
        }
        popupButton.onClick.AddListener(() =>
        {
            Destroy(this.gameObject);
        });
    }
}
