using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PlayerView _playerView;

    [SerializeField] private ActionController _actionController;
    public ActionController ActionController { get { return _actionController; } }
    [SerializeField] private SightController _sightController;
    public SightController SightController { get { return _sightController; } }
    [SerializeField] private PatternController _patternController;
    public PatternController PatternController { get { return _patternController; } }


    private int currentHP;
    public int CurrentHP { get; private set; }
    private int maxHP;


    private void Start()
    {
        maxHP = _playerData.MaxHP;
        currentHP = maxHP;
        _playerView.Initialize(_playerData);
        _actionController.Initialize(_playerData.MaxAP, _playerData.Actions);
        _patternController.Initialize(_playerData.Patterns);
    }

    public void ModifyCurrentHP(int value) // increases or decreases current HP by the value's amount
    {
        currentHP += value;
        Mathf.Clamp(currentHP, 0, maxHP);
        _playerView.ChangeHPValue(currentHP, maxHP);
    }
}
