using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;

public class CreepUIItemView : UIView
{
    public Toggle _toggle;
    public Image _creepPortrait;

    private CreepDef _creepDef;

    public void Setup(CreepDef creepDef, ToggleGroup group)
    {
        _creepDef = creepDef;
        _toggle.group = group;
        invalidateFlag |= InvalidationFlag.DYNAMIC_DATA;
    }

    public void Awake()
    {
        _toggle.onValueChanged.AddListener(onSelectToggled);
    }

    protected override void OnViewUpdate()
    {
        if(IsInvalid(InvalidationFlag.DYNAMIC_DATA))
        {
            if(_creepPortrait != null && _creepDef != null)
            {
                _creepPortrait.overrideSprite = _creepDef.icon;
            }
        }
    }

    private void onSelectToggled(bool value)
    {
        DispatchEvent(PlayerUIEventType.SELECT_CREEP_TYPE, true, _creepDef);
    }
}
