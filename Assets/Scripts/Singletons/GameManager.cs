using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this); // Make sure there's only ever one 
            return; // do NOT run any other code
        }

        DontDestroyOnLoad(gameObject);

        Player1 = FindObjectOfType<PlayerController>(); // move to a "when starting gameplay" method later
    }

    public PlayerController Player1 { get; private set; }

}
