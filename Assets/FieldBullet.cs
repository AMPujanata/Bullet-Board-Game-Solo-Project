using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldBullet : MonoBehaviour
{
    [SerializeField] private Image bulletBackground;
    [SerializeField] private Image bulletIcon;
    [SerializeField] private Image bulletStarIcon;
    [SerializeField] private TMPro.TMP_Text bulletNumberText;
    private Bullet bulletProperties;
    public void Initialize(Bullet initialBullet, BulletColorUIProperty uiProperties)
    {
        bulletProperties = initialBullet;
        bulletBackground.color = uiProperties.MainUIColor;
        bulletIcon.sprite = uiProperties.BulletSprite;
        bulletNumberText.text = bulletProperties.Number.ToString();
        if(bulletProperties.IsStar == true)
        {
            bulletStarIcon.gameObject.SetActive(true);
            bulletStarIcon.color = uiProperties.MainUIColor;
            bulletNumberText.color = Color.white;
        }
        else
        {
            bulletStarIcon.gameObject.SetActive(false);
            bulletNumberText.color = uiProperties.MainUIColor;
        }
    }
}
