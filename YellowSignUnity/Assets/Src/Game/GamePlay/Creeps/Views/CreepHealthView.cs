using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using DG.Tweening;

public class CreepHealthView : MonoBehaviour
{
    public float fadeOutDuration;
    public float fadeDelay;

    public CanvasGroup _canvasGroup;
    public RectTransform rectTransform;
    public Image _healthBar;

    private float _healthFill;
    private Tween _fadeTween;

    public void Awake()
    {
        //rectTransform = GetComponent<RectTransform>();
        _canvasGroup.alpha = 0;
    }

    public float healthFill
    {
        set
        {
            if(_healthBar != null && _healthFill != value)
            {
                _healthFill = value;
                _healthBar.fillAmount = value;

                restartFadeTween();
            }
        }
    }

    public void OnDisable()
    {
        _canvasGroup.alpha = 0;
        killTween();
    }

    private void killTween()
    {
        if(_fadeTween != null)
        {
            _fadeTween.Kill();
            _fadeTween = null;
        }
    }

    private void restartFadeTween()
    {
        killTween();

        _canvasGroup.alpha = 1;

        _fadeTween = _canvasGroup.DOFade(0.0f, fadeOutDuration);
        _fadeTween.SetDelay(fadeDelay);
        _fadeTween.OnComplete(() => _fadeTween = null);

    }
    
}
