using System.Collections.Generic;
using UnityEngine;


public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this); // Make sure there's only ever one 
            return; // do NOT run any other code
        }

        InitializeStartingBullets();
    }

    // SerializeField exposes the List in editor to make debugging easier
    [SerializeField] private List<Bullet> bulletsInCenter = new List<Bullet>();

    public Bullet TakeRandomBulletFromCenter()
    {
        Bullet randomChosenBullet = bulletsInCenter[Random.Range(0, bulletsInCenter.Count - 1)];
        bulletsInCenter.Remove(randomChosenBullet);
        return randomChosenBullet;
    }

    public void AddBulletToCenter(Bullet newBullet)
    {
        bulletsInCenter.Add(newBullet);
    }

    private void InitializeStartingBullets()
    {
        // There are 140 bullets. 5 colors, 4 numbers (with 6 bullets for 1/2/3, and 2 for 4), and 2 star bullets of each number.
        // Instead of writing them all down one by one, better to programatically create new bullets; easier to scale up later
        // There will never be any new bullets added to the bag mid-game. So there is no reason to separate the Bullet initialization into a separate function

        for(int color = 0; color < 5; color++)
        {
            for(int number = 1; number <= 4; number++)
            {
                int numberOfRegularBullets = number != 4 ? 6 : 2;
                for(int bulletLoop = 0; bulletLoop < numberOfRegularBullets; bulletLoop++)
                {
                    bulletsInCenter.Add(new Bullet((BulletColor)color, number, false));
                }
                for(int starBulletLoop = 0; starBulletLoop < 2; starBulletLoop++)
                {
                    bulletsInCenter.Add(new Bullet((BulletColor)color, number, true));
                }
            }
        }
    }
}
