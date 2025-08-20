using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletView : MonoBehaviour
{
    [SerializeField] private Image _bulletBackground;
    [SerializeField] private Image _bulletIcon;
    [SerializeField] private Image _bulletStarIcon;
    [SerializeField] private TMPro.TMP_Text _bulletNumberText;
    public void Initialize(BulletData initialBullet, BulletColorUIProperty uiProperties)
    {

        if (initialBullet.IsFacedown)
        {
            _bulletBackground.color = Color.gray;
            _bulletIcon.gameObject.SetActive(false);
            _bulletStarIcon.gameObject.SetActive(false);
            _bulletNumberText.gameObject.SetActive(false);
        }
        else
        {
            _bulletIcon.gameObject.SetActive(true);
            _bulletStarIcon.gameObject.SetActive(true);
            _bulletNumberText.gameObject.SetActive(true);

            _bulletBackground.color = uiProperties.MainUIColor;
            _bulletIcon.sprite = uiProperties.BulletSprite;
            _bulletNumberText.text = initialBullet.Number.ToString();
            if (initialBullet.IsStar == true)
            {
                _bulletStarIcon.gameObject.SetActive(true);
                _bulletStarIcon.color = uiProperties.MainUIColor;
                _bulletNumberText.color = Color.white;
            }
            else
            {
                _bulletStarIcon.gameObject.SetActive(false);
                _bulletNumberText.color = uiProperties.MainUIColor;
            }
        }
    }
}
