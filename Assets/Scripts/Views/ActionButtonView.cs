using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ActionButtonView : MonoBehaviour
{
    [SerializeField] private Button actionButton;
    [SerializeField] private Image actionIcon;
    [SerializeField] private TMP_Text actionCostText;
    [SerializeField] private TMP_Text actionDescriptionText;

    public void Initialize(string actionCost, string actionDescription, Sprite actionIconSprite = null, Action onActionClick = null)
    {
        actionCostText.text = actionCost;
        actionDescriptionText.text = actionDescription;
        actionIcon.sprite = actionIconSprite;
        if (onActionClick != null)
        {
            actionButton.onClick.AddListener(() => onActionClick());
        }
    }
}
