using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShieldSpace : MonoBehaviour
{
    [SerializeField] private Image _shieldBrokenIcon;
    [SerializeField] private Image _shieldCoveredIcon;
    [SerializeField] private TMP_Text _shieldIntensityText;
    [SerializeField] private TMP_Text _shieldAbilityText;
    [SerializeField] private Transform _shieldBreakParent;
    [SerializeField] private TMP_Text _shieldBreakRequirementText;
    [HideInInspector] public ShieldData ShieldProperties;

    public void Initialize(ShieldData shieldData, bool isFinalShield)
    {
        ShieldProperties = shieldData;
        _shieldBrokenIcon.gameObject.SetActive(false);
        _shieldCoveredIcon.gameObject.SetActive(true);
        if (isFinalShield)
        {
            _shieldIntensityText.text = "";
            _shieldBreakParent.gameObject.SetActive(false);
        }
        else
        {
            _shieldIntensityText.text = ShieldProperties.ShieldIntensity.ToString();
            _shieldBreakParent.gameObject.SetActive(true);
            _shieldBreakRequirementText.text = ShieldProperties.NextShieldBreak.ToString();
        }
        _shieldAbilityText.text = ShieldProperties.OnShieldBreak != null ? ShieldProperties.OnShieldBreak.EffectDescription : "";
    }

    public void SetAsClearedInactiveShield()
    {
        _shieldBrokenIcon.gameObject.SetActive(true);
        _shieldCoveredIcon.gameObject.SetActive(false);
        _shieldIntensityText.text = "";
        _shieldAbilityText.text = "";
    }

    public void SetAsCurrentActiveShield()
    {
        _shieldBrokenIcon.gameObject.SetActive(false);
        _shieldCoveredIcon.gameObject.SetActive(false);
        _shieldIntensityText.text = ShieldProperties.ShieldIntensity.ToString();
        _shieldAbilityText.text = ShieldProperties.OnShieldBreak != null ? ShieldProperties.OnShieldBreak.EffectDescription : "";
    }

    public void SetAsUnclearedInactiveShield()
    {
        _shieldBrokenIcon.gameObject.SetActive(false);
        _shieldCoveredIcon.gameObject.SetActive(true);
        _shieldIntensityText.text = ShieldProperties.ShieldIntensity.ToString();
        _shieldAbilityText.text = ShieldProperties.OnShieldBreak != null ? ShieldProperties.OnShieldBreak.EffectDescription : "";
    }
}
