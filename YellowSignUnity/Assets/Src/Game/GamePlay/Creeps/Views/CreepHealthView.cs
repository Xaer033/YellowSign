using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;

public class CreepHealthView : MonoBehaviour
{
    public RectTransform rectTransform;
    public Image _healthBar;

    private float _healthFill;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public float healthFill
    {
        set
        {
            if(_healthBar != null && _healthFill != value)
            {
                _healthFill = value;
                _healthBar.fillAmount = value;
            }
        }
    }
    
}
