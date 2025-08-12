using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class ActionView : MonoBehaviour
{
    [SerializeField] private Slider _apBar;
    [SerializeField] private TMP_Text _apBarText;
    [SerializeField] private GameObject _actionButtonPrefab;
    [SerializeField] private ActionSpace[] _actionSpaces;

    public void ChangeAPValue(int currentAP, int maxAP)
    {
        _apBar.maxValue = maxAP;
        _apBar.value = currentAP;
        _apBarText.text = currentAP + " / " + maxAP;
    }

    public ActionSpace[] GetActionSpaces()
    {
        return _actionSpaces;
    }

    public void SpawnNewActionButtonObject(int index, BaseAction action)
    {
        _actionSpaces[index].SpaceProperties = action;
        GameObject newActionButton = Instantiate(_actionButtonPrefab, _actionSpaces[index].gameObject.transform);
        if (action.IsStar) // don't add the OnActivated to the button
        {
            newActionButton.GetComponent<ActionButtonView>().Initialize(action.ActionCost.ToString(), action.ActionText, action.ActionIcon, null);
        }
        else // otherwise, do
        {
            newActionButton.GetComponent<ActionButtonView>().Initialize(action.ActionCost.ToString(), action.ActionText, action.ActionIcon, action.OnActivated);
        }
    }
}
