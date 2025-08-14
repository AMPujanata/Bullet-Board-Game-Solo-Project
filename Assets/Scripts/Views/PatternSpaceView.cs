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
                // add in color icons here
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
