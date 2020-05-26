using GhostGen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : UIView
{
    public CanvasGroup _canvasGroup;
    
    public Button _button;
    public Toggle _toggle;
    public TMP_Text _shortcutText;

    public Image _icon;

    private ActionButtonData _actionData;

    private GameplayResources _gameplayResources;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (_button != null)
        {
            _button.onClick.AddListener(onAction);
        }

        if (_toggle)
        {
            _toggle.onValueChanged.AddListener(onToggle);
        }

        _gameplayResources = Singleton.instance.gameConfig.gameplayResources;
    }

    public void SetActionData(ActionButtonData data)
    {
        if (_actionData != data)
        {
            _actionData = data;
            invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if (_actionData != null && gameObject.activeSelf)
        {
            if (Input.GetKeyDown(_actionData.shortcutKey))
            {
                if (_actionData.isToggle)
                {
                    _toggle.isOn = !_toggle.isOn;
                }
                else
                {
                    onAction();
                }
            }
        }
    }

    protected override void OnViewUpdate()
    {
        if (IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            if (_actionData != null)
            {
//                gameObject.SetActive(true);
                if (_canvasGroup)
                {
                    _canvasGroup.interactable = true;
                    _canvasGroup.alpha = 1;
                }
                
                if (_toggle)
                {
                    _toggle.enabled = _actionData.isToggle;
                    _toggle.SetIsOnWithoutNotify(_actionData.toggleValue);
                }

                if (_button)
                {
                    _button.enabled = !_actionData.isToggle;
                }
                
                if (_shortcutText)
                {
                    _shortcutText.gameObject.SetActive(_actionData.shortcutKey != KeyCode.None);
                    _shortcutText.text = _actionData.shortcutKey.ToString();
                }

                if (_icon)
                {
                    _icon.overrideSprite = _gameplayResources.GetIcon(_actionData.iconName);
                }
                
            }
            else
            {
                if (_canvasGroup)
                {
                    _canvasGroup.interactable = false;
                    _canvasGroup.alpha = 0;
                }
            }
        }
    }

    private void onToggle(bool value)
    {
        if (_actionData != null)
        {
            _actionData.toggleValue = value;
            onAction();
        }
    }
    private void onAction()
    {
        if (_actionData != null)
        {
            DispatchEvent(_actionData.actionId, true, _actionData);
        }
    }
}
