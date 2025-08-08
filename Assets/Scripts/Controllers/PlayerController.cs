using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PlayerView _playerView;
    [SerializeField] private ActionController _actionManager;
    [SerializeField] private CurrentController _currentManager;

    private int currentHP;
    public int CurrentHP { get; private set; }
    private int maxHP;


    private void Start()
    {
        maxHP = _playerData.MaxHP;
        currentHP = maxHP;
        _playerView.Initialize(_playerData);
        _actionManager.Initialize(_playerData.MaxAP, _playerData.Actions);
    }

    public void ModifyCurrentHP(int value) // increases or decreases current HP by the value's amount
    {
        currentHP += value;
        Mathf.Clamp(currentHP, 0, maxHP);
        _playerView.ChangeHPValue(currentHP, maxHP);
    }
}
