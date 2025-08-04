using UnityEngine;
using System;

[Serializable]
public class BulletColorUIProperty
{
    public BulletColor BulletColorRequirement;
    public Sprite BulletSprite;
    public Color MainUIColor;
}

public class CurrentManager : MonoBehaviour
{

    [Serializable]
    public class BulletSpace
    {
        public GameObject SpaceObject;
        [NonSerialized] public Bullet SpaceProperties; // bullet properties don't need to be serialized on launch, only modified at runtime
    }

    [SerializeField] private BulletColorUIProperty[] bulletColorUIProperties;
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private BulletSpace[] redCurrentColumn;
    [SerializeField] private BulletSpace[] blueCurrentColumn;
    [SerializeField] private BulletSpace[] greenCurrentColumn;
    [SerializeField] private BulletSpace[] yellowCurrentColumn;
    [SerializeField] private BulletSpace[] pinkCurrentColumn;

    public void SpawnBulletToCurrent()
    {
        Bullet bulletToSpawn = BulletManager.Instance.TakeRandomBulletFromCenter();

        BulletColorUIProperty chosenColorUIProperty = Array.Find(bulletColorUIProperties, property => property.BulletColorRequirement == bulletToSpawn.Color);
        BulletSpace[] columnToSearchIn = bulletToSpawn.Color switch // pick column to spawn into depending on bullet color
        {
            BulletColor.Red => redCurrentColumn,
            BulletColor.Blue => blueCurrentColumn,
            BulletColor.Green => greenCurrentColumn,
            BulletColor.Yellow => yellowCurrentColumn,
            BulletColor.Pink => pinkCurrentColumn,
            _ => redCurrentColumn,
        };

        int spacesToMoveDown = bulletToSpawn.Number;
        int spacesMoved = 0;
        int spaceToSpawnInto;

        for (spaceToSpawnInto = 0; spaceToSpawnInto < columnToSearchIn.Length; spaceToSpawnInto++)
        {
            if (columnToSearchIn[spaceToSpawnInto].SpaceProperties == null) spacesMoved++;
            if (spacesMoved >= spacesToMoveDown)
            {
                Debug.Log("Space found in time!");
                break;
            }
        }

        if(spacesMoved >= spacesToMoveDown)
        {
            GameObject newBulletObject = Instantiate(bulletPrefab, columnToSearchIn[spaceToSpawnInto].SpaceObject.transform);
            newBulletObject.GetComponent<CurrentBullet>().Initialize(bulletToSpawn, chosenColorUIProperty);
            columnToSearchIn[spaceToSpawnInto].SpaceProperties = bulletToSpawn; // NOTE: this will eventually be moved to per-space properties to facilitate better interaction with other mechanics
        }
        else
        {
            Debug.Log("No space remaining! Life lost!");
        }
    }

}
