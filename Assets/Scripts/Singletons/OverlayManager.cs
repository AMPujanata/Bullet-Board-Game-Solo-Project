using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour
{
    public static OverlayManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this); // Make sure there's only ever one 
            return; // do NOT run any other code
        }
        DontDestroyOnLoad(gameObject);
    }

    private CanvasGroup _mainCanvasGroup;
    public CanvasGroup MainCanvasGroup { get
        {
            if (_mainCanvasGroup == null)
            {
                _mainCanvasGroup = FindObjectOfType<CanvasGroup>(true);
            }
            return _mainCanvasGroup;
        }
    }
    [SerializeField] private Canvas _overlayCanvas;
    [SerializeField] private GameObject _popupPrefab;
    [SerializeField] private GameObject _damageScreenEffectPrefab;
    [SerializeField] private GameObject _bulletClearPrefab;

    private GameObject _currentPopup;
    public void DisplayPopup(string popupString, Vector3 popupLocation, string popupButton1String, Action onButton1Press = null, string popupButton2String = null, Action onButton2Press = null)
    {
        if(_overlayCanvas.worldCamera == null) // if the render camera is gone (from switching scenes), reassign it to the new main camera
        {
            _overlayCanvas.worldCamera = Camera.main;
        }

        if(!_currentPopup) _currentPopup = Instantiate(_popupPrefab, popupLocation, Quaternion.identity, _overlayCanvas.transform);
        _currentPopup.gameObject.SetActive(true);
        _currentPopup.GetComponent<PopupView>().Initialize(popupString, popupButton1String, onButton1Press, popupButton2String, onButton2Press);
        MainCanvasGroup.interactable = false;
    }

    public void ClosePopup()
    {
        MainCanvasGroup.interactable = true;
        _currentPopup.gameObject.SetActive(false);
        _currentPopup = null;
    }

    public void ShowTakeDamageEffect()
    {
        StartCoroutine(TakeDamageEffectRoutine(0.5f));
    }

    bool _isRunningTakeDamageEffect = false;
    private IEnumerator TakeDamageEffectRoutine(float fadeDuration)
    {
        if (_isRunningTakeDamageEffect) yield break; // don't need to run another instance of the effect

        GameObject takeDamageEffect = Instantiate(_damageScreenEffectPrefab, _overlayCanvas.transform);
        Image takeDamageImage = takeDamageEffect.GetComponent<Image>();
        _isRunningTakeDamageEffect = true;
        float elapsedTime = 0f;

        while(elapsedTime < fadeDuration)
        {
            float transparencyPercentage = elapsedTime / fadeDuration;
            
            Color imageColorTemp = takeDamageImage.color;
            imageColorTemp.a = 1 - transparencyPercentage;
            takeDamageImage.color = imageColorTemp;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(takeDamageEffect);
        _isRunningTakeDamageEffect = false;
    }

    public void ShowBulletClearEffect(Vector3 clearLocation)
    {
        StartCoroutine(ShowBulletClearRoutine(clearLocation, 0.5f));
    }

    private IEnumerator ShowBulletClearRoutine(Vector3 clearLocation, float fadeAndStayDuration)
    {

        GameObject bulletClearObject = Instantiate(_bulletClearPrefab, _overlayCanvas.transform);
        bulletClearObject.transform.position = clearLocation;
        Image bulletClearImage = bulletClearObject.GetComponent<Image>();
        float fadeDuration = fadeAndStayDuration / 2;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float transparencyPercentage = elapsedTime / fadeDuration;

            Color imageColorTemp = bulletClearImage.color;
            imageColorTemp.a = transparencyPercentage;
            bulletClearImage.color = imageColorTemp;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while(elapsedTime < fadeAndStayDuration) // extra time for the bullet clear effect to linger
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(bulletClearObject);
    }
}
