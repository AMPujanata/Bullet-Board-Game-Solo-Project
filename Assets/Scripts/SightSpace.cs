using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SightSpace : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [field:SerializeField] public Transform BulletParent { get; private set; }
    [SerializeField] private Image _spaceValidityImage;
    [SerializeField] private Color _validSpaceColor;
    [SerializeField] private Color _invalidSpaceColor;
    public BulletData BulletProperties;

    public Vector2Int SightCell { get; private set; }
    private bool _isInitialized = false;

    public void Initialize(Vector2Int cell)
    {
        SightCell = cell;
        _isInitialized = true;
    }

    public void SetSpaceValidity(bool shouldDisplay, bool isValid)
    {
        if (!shouldDisplay)
        {
            _spaceValidityImage.gameObject.SetActive(false);
        }
        else
        {
            _spaceValidityImage.gameObject.SetActive(true);
            _spaceValidityImage.color = isValid ? _validSpaceColor : _invalidSpaceColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isInitialized) return;
        GameManager.Instance.ActivePlayer.SightController.UpdateActiveSpace(SightCell);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isInitialized) return;
        GameManager.Instance.ActivePlayer.SightController.RemoveActiveSpace(SightCell);
    }
}
