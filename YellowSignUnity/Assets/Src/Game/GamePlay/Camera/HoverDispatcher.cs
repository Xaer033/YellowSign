using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GhostGen;

public class HoverDispatcher : EventDispatcherBehavior, IPointerEnterHandler, IPointerExitHandler
{
    public const string EVENT_HOVER = "eventHover";

    public bool isHover { get; private set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHover = true;
        DispatchEvent(EVENT_HOVER, true, isHover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHover = false;
        DispatchEvent(EVENT_HOVER, true, isHover);
    }
}
