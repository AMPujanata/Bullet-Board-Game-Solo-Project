using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
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


    public void Initialize(PlayerData playerData)
    {
        _maxHP = playerData.MaxHP;
        _currentHP = _maxHP;
        _playerView.Initialize(playerData);
        _actionController.Initialize(playerData.MaxAP, playerData.Actions);
        _patternController.Initialize(playerData.Patterns);
        _sightController.Initialize();
        playerData.Passive.SetupPassive();
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
