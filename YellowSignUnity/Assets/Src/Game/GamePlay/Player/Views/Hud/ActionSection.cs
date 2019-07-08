using System;
using System.Collections;
using System.Collections.Generic;
using GhostGen;
using TMPro;
using UnityEngine;

public class ActionSection : EventDispatcherBehavior
{
    public ActionButton[] _actionButtonList;
    public TMP_Text _titleText;


    public void SetActions(List<ActionButtonData> actionDataList)
    {
        if (_actionButtonList == null || actionDataList == null)
        {
            return;
        }

        for (int i = 0; i < _actionButtonList.Length; ++i)
        {
            
            ActionButton button = _actionButtonList[i];
            if (button == null)
            {
                continue;
            }
            
            if (i < actionDataList.Count)
            {
                ActionButtonData data = actionDataList[i];
                button.SetActionData(data);
            }
            else
            {
                button.SetActionData(null);
            }
        }
    }
}
