using UnityEngine;
using UnityEngine.UI;

public class CurrentSpace : MonoBehaviour
{
    [SerializeField] private Outline _outline;
    public BulletData BulletProperties;
    public int CurrentRow { get; private set; }
    public int CurrentColumn { get; private set; }
    //private CurrentView _currentView;
    //private bool _isInitialized = false;

    public void Initialize(int row, int column, CurrentView currentView)
    {
        CurrentRow = row;
        CurrentColumn = column;
        //_currentView = view;
        //_isInitialized = true;
    }
}
