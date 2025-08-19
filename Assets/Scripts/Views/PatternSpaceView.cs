using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PatternSpaceView : MonoBehaviour
{
    [SerializeField] private Image _patternBulletImage;
    [SerializeField] private TMP_Text _patternSpaceNumber;
    [SerializeField] private Image _patternBulletClearImage;
    [SerializeField] private Image _patternBulletStarImage;

    [SerializeField] private Sprite _patternBulletNeededSprite;
    [SerializeField] private Sprite _patternEmptyNeededSprite;

    #region Color Icon Sprites
    [SerializeField] private Sprite _redBulletNeededSprite;
    [SerializeField] private Sprite _blueBulletNeededSprite;
    [SerializeField] private Sprite _greenBulletNeededSprite;
    [SerializeField] private Sprite _yellowBulletNeededSprite;
    [SerializeField] private Sprite _pinkBulletNeededSprite;
    #endregion

    public void Initialize(PatternSpaceData patternSpaceData)
    {
        if (patternSpaceData.NeedsBullet)
        {
            _patternBulletImage.sprite = _patternBulletNeededSprite;
            _patternBulletImage.gameObject.SetActive(true);

            if (patternSpaceData.NumberRequired != 0)
            {
                _patternSpaceNumber.gameObject.SetActive(true);
                _patternSpaceNumber.text = patternSpaceData.NumberRequired.ToString();
            }
            else if (patternSpaceData.NeedsSameNumber)
            {
                _patternSpaceNumber.gameObject.SetActive(true);
                _patternSpaceNumber.text = "=";
            }
            else if (patternSpaceData.ColorRequired != BulletColor.Any)
            {
                switch (patternSpaceData.ColorRequired)
                {
                    case BulletColor.Red:
                        _patternBulletImage.sprite = _redBulletNeededSprite;
                        break;
                    case BulletColor.Blue:
                        _patternBulletImage.sprite = _blueBulletNeededSprite;
                        break;
                    case BulletColor.Green:
                        _patternBulletImage.sprite = _greenBulletNeededSprite;
                        break;
                    case BulletColor.Yellow:
                        _patternBulletImage.sprite = _yellowBulletNeededSprite;
                        break;
                    case BulletColor.Pink:
                        _patternBulletImage.sprite = _pinkBulletNeededSprite;
                        break;
                }
            }
        }
        else if (patternSpaceData.NeedsEmpty)
        {
            _patternBulletImage.sprite = _patternEmptyNeededSprite;
            _patternBulletImage.gameObject.SetActive(true);
        }
        if (patternSpaceData.WillClearBullet)
        {
            _patternBulletClearImage.gameObject.SetActive(true);
        }
        else if (patternSpaceData.NeedsStarBullet)
        {
            _patternBulletClearImage.gameObject.SetActive(true);
        }
    }
}
