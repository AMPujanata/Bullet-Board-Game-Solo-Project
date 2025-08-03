using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum BulletColor { Red = 0, Blue = 1, Green = 2, Yellow = 3, Pink = 4}

[Serializable]
public class Bullet
{
    public BulletColor Color;
    public int Number;
    public bool IsStar;
    public bool IsFacedown;
    public Bullet(BulletColor color = BulletColor.Red, int number = 1, bool isStar = false, bool isFacedown = false)
    {
        Color = color;
        Number = number;
        IsStar = isStar;
        IsFacedown = isFacedown;
    }
}

[Serializable]
public class BulletColorUIProperty
{
    public BulletColor BulletColorRequirement;
    public Sprite BulletSprite;
    public Color MainUIColor;
}

public class BulletManager : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private BulletColorUIProperty[] bulletColorUIProperties;

    // SerializeField exposes the List in editor to make debugging easier
    [SerializeField] private List<Bullet> bulletsInCenter = new List<Bullet>();

    public Bullet TakeRandomBulletFromCenter()
    {
        Bullet randomChosenBullet = bulletsInCenter[UnityEngine.Random.Range(0, bulletsInCenter.Count - 1)];
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

    private void Start()
    {
        // currently only used to test functions. Currently testing: initializing bullets, grabbing a random bullet from the list, then initializing it
        InitializeStartingBullets();
        Bullet randomChosenBullet = TakeRandomBulletFromCenter();
        BulletColorUIProperty chosenColorUIProperty = Array.Find(bulletColorUIProperties, property => property.BulletColorRequirement == randomChosenBullet.Color);
        GameObject newBulletObject = GameObject.Instantiate(bulletPrefab, this.transform); // THIS.TRANSFORM TEMPORARY UNTIL CANVAS IS FLESHED OUT
        newBulletObject.GetComponent<FieldBullet>().Initialize(randomChosenBullet, chosenColorUIProperty);
    }
}
