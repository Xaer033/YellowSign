using GhostGen;
using UnityEngine;
using UnityEngine.UI;

public class MiniSelectIcon : UIView
{
    public Image _fillBar;
    public Image _icon;
    public Button _button;

    private float _fillAmount;
    private Sprite _iconSprite;
    private IActor _actor;

    protected void Awake()
    {
        if (_button)
        {
            _button.onClick.AddListener(onAction);
        }
    }
    
    public void SetData(Sprite icon, float fillAmount, IActor actor)
    {
        if (_fillAmount == fillAmount && _iconSprite == icon && _actor == actor)
        {
            return;
        }
        
        _fillAmount = fillAmount;
        _iconSprite = icon;
        _actor = actor;
        
        invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
    }

    protected override void OnViewUpdate()
    {
        if (IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            if (_fillBar != null)
            {
                _fillBar.fillAmount = _fillAmount;
            }

            if (_icon)
            {
                _icon.overrideSprite = _iconSprite;
            }
        }
    }

    private void onAction()
    {
        if (_actor != null)
        {
            DispatchEvent(PlayerUIEventType.MINI_ACTOR_SELECTED, true, _actor);
        }
    }
}
