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

    private Queue<Action<Action<bool>>> _starActionsQueue = new Queue<Action<Action<bool>>>(); // This specific Action type means an Action that takes an Action<bool> as a parameter, which is used for callbacks
    private bool _currentlyResolvingStarActions = false;                                       // It looks gross but it does work
    public void ActivateStarActions()
    {
        // Because there are some star actions that need player interaction to resolve,
        // star actions should go on queue so that they can be resolved in a set, specific order
        // and prevent duplicate actions from conflicting with each other.

        ActionSpace[] actionSpaces = _actionView.GetActionSpaces();
        for (int i = 0; i < actionSpaces.Length; i++)
        {
            if (actionSpaces[i].SpaceProperties != null)
            {
                if (actionSpaces[i].SpaceProperties.IsStar)
                {
                    _starActionsQueue.Enqueue(actionSpaces[i].SpaceProperties.ActivateAction);
                }
            }
        }

        NextStarActionInQueue();
    }

    private void NextStarActionInQueue()
    {
        if (_starActionsQueue.Count <= 0 || _currentlyResolvingStarActions) return; // no actions left to perform, and don't perform actions while another is happening
        
        Action<Action<bool>> actionToActivate = _starActionsQueue.Dequeue();
        _currentlyResolvingStarActions = true;

        actionToActivate.Invoke((isSucessful) =>
        {
            // currently no use for the bool callback, but it is nice to have for troubleshooting
            _currentlyResolvingStarActions = false;
            NextStarActionInQueue();
        });
        // callback here that turns resolving star actions back on
    }

    public void ModifyCurrentAP(int value) // increases or decreases current HP by the value's amount
    {
        _currentAP += value;
        _currentAP = Mathf.Clamp(_currentAP, 0, _maxAP);
        _actionView.ChangeAPValue(_currentAP, _maxAP);
    }

    public void RefreshAP()
    {
        _currentAP = _maxAP;
        _actionView.ChangeAPValue(_currentAP, _maxAP);
    }
}
