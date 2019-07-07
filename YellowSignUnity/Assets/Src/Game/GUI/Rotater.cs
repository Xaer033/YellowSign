using System;
using DG.Tweening;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public float rotateSpeed;
    public float scaleBob;
    
    private Vector3 _originalScale;
    private Tween _scaleTween;
    void Start()
    {
        _originalScale = transform.localScale;
        _scaleTween = transform.DOScale(_originalScale * scaleBob, 1).SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
    // Update is called once per frame
    void Update()
    {
        if (transform != null)
        {
            Vector3 eulerAngl = transform.localRotation.eulerAngles;
            eulerAngl.y += rotateSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(eulerAngl);
        }
    }

    private void OnDestroy()
    {
        if (_scaleTween != null)
        {
            _scaleTween.Kill();
            _scaleTween = null;
        }
    }
}
