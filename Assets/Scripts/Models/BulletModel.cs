using UnityEngine;

public enum BulletColor { Red = 0, Blue = 1, Green = 2, Yellow = 3, Pink = 4, Any = -1 }

public class BulletData
{
    public BulletColor Color;
    public int Number;
    public bool IsStar;
    public bool IsFacedown;
    public BulletData(BulletColor color = BulletColor.Any, int number = 0, bool isStar = false, bool isFacedown = false)
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
    public bool NeedsBullet;
    public bool NeedsEmpty;
    public BulletColor ColorRequired;
    public int NumberRequired;
    public bool NeedsSameNumber;
    public bool NeedsStarBullet;
    public bool WillClearBullet;
    [HideInInspector] public bool NeedsFaceUp; // There is never a pattern that needs to be serialized with NeedsFaceUp.
    public PatternSpaceData(bool needsBullet = false, bool needsEmpty = false, BulletColor colorRequired = BulletColor.Any, int numberRequired = 0, bool needsSameNumber = false, bool needsStarBullet = false, bool willClearBullet = false, bool needsFaceUp = false)
    {
        NeedsBullet = needsBullet;
        NeedsEmpty = needsEmpty;
        ColorRequired = colorRequired;
        NumberRequired = numberRequired;
        NeedsSameNumber = needsSameNumber;
        NeedsStarBullet = needsStarBullet;
        WillClearBullet = willClearBullet;
        NeedsFaceUp = needsFaceUp;
    }
}
