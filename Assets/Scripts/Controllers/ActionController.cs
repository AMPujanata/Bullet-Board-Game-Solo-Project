using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActionView))]
public class ActionController : MonoBehaviour
{
    [SerializeField] private ActionView _actionView;

    private int _currentAP;
    public int CurrentAP { get { return _currentAP; }}
    private int _maxAP;

    public void Initialize(int newMaxAP, BaseAction[] actionsToSpawn)
    {
        _maxAP = newMaxAP;
        _currentAP = _maxAP;

        _actionView.ChangeAPValue(_currentAP, _maxAP);

        ActionSpace[] actionSpaces = _actionView.GetActionSpaces();
        foreach (BaseAction action in actionsToSpawn)
        {
            for (int i = 0; i < actionSpaces.Length; i++)
            {
                if (actionSpaces[i].SpaceProperties == null)
                {
                    _actionView.SpawnNewActionButtonObject(i, action);
                    break;
                }
            }
        }
    }

    public void ModifyCurrentAP(int value) // increases or decreases current HP by the value's amount
    {
        _currentAP += value;
        Mathf.Clamp(_currentAP, 0, _maxAP);
        _actionView.ChangeAPValue(_currentAP, _maxAP);
    }
}
