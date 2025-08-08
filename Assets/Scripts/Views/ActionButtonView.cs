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

    public void Initialize(string actionCost, string actionDescription, Sprite actionIconSprite = null, Action onActionClick = null)
    {
        _actionCostText.text = actionCost;
        _actionDescriptionText.text = actionDescription;
        _actionIcon.sprite = actionIconSprite;
        if (onActionClick != null)
        {
            _actionButton.onClick.AddListener(() => onActionClick());
        }
    }
}
