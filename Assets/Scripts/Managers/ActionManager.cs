using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class ActionManager : MonoBehaviour // SPLIT INTO MANAGER AND VIEW LATER
{
    [Serializable]
    public class ActionSpace
    {
        public GameObject SpaceObject;
        [NonSerialized] public BaseAction SpaceProperties;
    }

    [SerializeField] private Slider apBar;
    [SerializeField] private TMP_Text apBarText;
    [SerializeField] private GameObject actionButtonPrefab;

    [SerializeField] private ActionSpace[] actionSpaces;
    private PlayerManager playerView;
    private int maxAP;
    public void Initialize(int maxAP, BaseAction[] actionsToSpawn)
    {
        apBar.maxValue = maxAP;
        ChangeAPValue(maxAP);
        foreach (BaseAction action in actionsToSpawn)
        {
            for(int i = 0; i < actionSpaces.Length; i++)
            {
                if(actionSpaces[i].SpaceProperties == null)
                {
                    actionSpaces[i].SpaceProperties = action;
                    GameObject newActionButton = Instantiate(actionButtonPrefab, actionSpaces[i].SpaceObject.transform);
                    newActionButton.GetComponent<ActionButtonView>().Initialize(action.ActionCost.ToString(), action.ActionText, action.ActionIcon, action.OnActivated);
                    break;
                }
            }
        }
    }

    public void ChangeAPValue(int currentAP)
    {
        apBar.value = currentAP;
        apBarText.text = currentAP + " / " + maxAP;
    }
}
