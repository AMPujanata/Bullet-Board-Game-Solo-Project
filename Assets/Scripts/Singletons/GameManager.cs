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
        ActivePlayer = Player1;

        StartCoroutine(BeginSetup());
    }

    private IEnumerator BeginSetup()
    {
        yield return new WaitForSeconds(0.5f);
        int precaution = 0;
        while (ActivePlayer.PatternController.GetNumberOfCardsInHand() < 3)
        {
            ActivePlayer.PatternController.DrawPatternFromDeck();
            precaution++;
            if (precaution > 10)
            {
                Debug.Log("Infinite loop beginning; breaking!");
                break;
            }
        }
        yield break;
    }

    public PlayerController ActivePlayer { get; private set; }

    public PlayerController Player1 { get; private set; }
}
