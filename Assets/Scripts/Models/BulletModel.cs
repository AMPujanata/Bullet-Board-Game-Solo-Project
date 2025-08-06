using UnityEngine;

public enum BulletColor { Red = 0, Blue = 1, Green = 2, Yellow = 3, Pink = 4 }

[System.Serializable]
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

[System.Serializable]
public class BulletColorUIProperty
{
    public BulletColor BulletColorRequirement;
    public Sprite BulletSprite;
    public Color MainUIColor;
}

