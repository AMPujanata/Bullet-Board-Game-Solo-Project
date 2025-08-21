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


    private int _currentHP;
    public int CurrentHP { get; private set; }
    private int _maxHP;


    private void Start()
    {
        _maxHP = _playerData.MaxHP;
        _currentHP = _maxHP;
        _playerView.Initialize(_playerData);
        _actionController.Initialize(_playerData.MaxAP, _playerData.Actions);
        _patternController.Initialize(_playerData.Patterns);
        _playerData.Passive.SetupPassive();
    }

    public void ModifyCurrentHP(int value) // increases or decreases current HP by the value's amount
    {
        _currentHP += value;
        _currentHP = Mathf.Clamp(_currentHP, 0, _maxHP);
        _playerView.ChangeHPValue(_currentHP, _maxHP);
        if (_currentHP <= 0)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }
}
