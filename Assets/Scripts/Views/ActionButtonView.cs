using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ActionButtonView : MonoBehaviour
{
    [SerializeField] private Button _actionButton;
    [SerializeField] private Image _actionIcon;
    [SerializeField] private TMP_Text _actionCostText;
    [SerializeField] private TMP_Text _actionDescriptionText;

    public void Initialize(BaseAction action)
    {
        _actionCostText.text = action.ActionCost.ToString();
        _actionDescriptionText.text = action.ActionText;
        _actionIcon.sprite = action.ActionIcon;
        if (action.IsStar)
        {
            _actionCostText.text = "S";
        }
        else
        {
            _actionButton.onClick.AddListener(() => action.OnActivated((isSuccessful) => { }));
        }
    }
}
