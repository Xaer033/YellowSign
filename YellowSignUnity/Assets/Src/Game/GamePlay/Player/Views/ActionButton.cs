using System.Collections;
using System.Collections.Generic;
using GhostGen;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : UIView
{
    public Button _button;

    public TMP_Text _shortcutText;

    public Image _icon;

    private ActionButtonData _actionData;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (_button != null)
        {
            _button.onClick.AddListener(onAction);
        }
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
        
        if (_actionData != null && gameObject.GetActive())
        {
            if (Input.GetKeyDown(_actionData.shortcutKey))
            {
                onAction();
            }
        }
    }

    protected override void OnViewUpdate()
    {
        if (IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            if (_actionData != null)
            {
                gameObject.SetActive(false);
                
                if (_shortcutText)
                {
                    _shortcutText.gameObject.SetActive(_actionData.shortcutKey != KeyCode.None);
                    _shortcutText.text = _actionData.shortcutKey.ToString();
                }

                if (_icon)
                {
                    _icon.sprite = Singleton.instance.gameConfig.gameplayResources.GetIcon(_actionData.iconName);
                }
                
            }
            else
            {
                gameObject.SetActive(false);
            }
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
