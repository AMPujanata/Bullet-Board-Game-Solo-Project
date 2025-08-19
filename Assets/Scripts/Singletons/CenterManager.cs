using System.Collections.Generic;
using UnityEngine;

public class CenterManager : MonoBehaviour
{
    public static CenterManager Instance { get; private set; }

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

    private List<BulletData> _bulletsInCenter = new List<BulletData>();
    private List<BulletData> _bulletsInIntensity = new List<BulletData>();

    public BulletData GetRandomBulletFromCenter()
    {
        BulletData randomChosenBullet = _bulletsInCenter[Random.Range(0, _bulletsInCenter.Count - 1)];
        _bulletsInCenter.Remove(randomChosenBullet);
        return randomChosenBullet;
    }

    public void ReturnAllBulletsFromIntensityToCenter()
    {
        List<BulletData> allIntensityBullets = _bulletsInIntensity;
        _bulletsInIntensity.Clear();
        foreach(BulletData data in allIntensityBullets)
        {
            AddBulletToCenter(data);
        }
    }

    public int GetNumberOfBulletsInIntensity()
    {
        return _bulletsInIntensity.Count;
    }

    public void AddBulletToCenter(BulletData newBullet)
    {
        _bulletsInCenter.Add(newBullet);
    }

    public void AddBulletToIntensity(BulletData newBullet)
    {
        _bulletsInIntensity.Add(newBullet);
    }

    private void InitializeStartingBullets()
    {
        // There are 140 bullets. 5 colors, 4 numbers (with 6 bullets for 1/2/3, and 2 for 4), and 2 star bullets of each number.
        // Instead of writing them all down one by one, better to programatically create new bullets; easier to scale up later

        for(int color = 0; color < 5; color++)
        {
            for(int number = 1; number <= 4; number++)
            {
                int numberOfRegularBullets = number != 4 ? 6 : 2;
                for(int bulletLoop = 0; bulletLoop < numberOfRegularBullets; bulletLoop++)
                {
                    AddBulletToCenter(new BulletData((BulletColor)color, number, false));
                }
                for(int starBulletLoop = 0; starBulletLoop < 2; starBulletLoop++)
                {
                    AddBulletToCenter(new BulletData((BulletColor)color, number, true));
                }
            }
        }
    }
}
