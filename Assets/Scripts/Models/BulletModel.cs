using UnityEngine;

public enum BulletColor { Red = 0, Blue = 1, Green = 2, Yellow = 3, Pink = 4, Any = -1 }

public class BulletData
{
    public BulletColor Color;
    public int Number;
    public bool IsStar;
    public bool IsFacedown;
    public BulletData(BulletColor color = BulletColor.Red, int number = 1, bool isStar = false, bool isFacedown = false)
    {
        Color = color;
        Number = number;
        IsStar = isStar;
        IsFacedown = isFacedown;
    }
}

[System.Serializable]
public class BulletColorUIProperty
{
    public BulletColor BulletColorRequirement;
    public Sprite BulletSprite;
    public Color MainUIColor;
}

[System.Serializable]
public class PatternSpaceData
{
    public bool NeedsBullet = false;
    public bool NeedsEmpty = false;
    public BulletColor ColorRequired = BulletColor.Any;
    public int NumberRequired = -1;
    public bool NeedsSameNumber = false;
    public bool NeedsStarBullet = false;
    public bool WillClearBullet = false;
}
