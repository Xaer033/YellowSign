using UnityEngine.EventSystems;

namespace GhostGen
{

    public class EventForwarder : EventDispatcherBehavior,
         IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler, IEventSystemHandler
    {

        public EventTriggerType eventType;
        public string gameEventType;

        public bool withBubbling = true;
        
        public void DispatchEventForward(string eventName, BaseEventData eventData)
        {
            DispatchEvent(eventName, withBubbling, eventData);
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if(eventType == EventTriggerType.BeginDrag)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnCancel(BaseEventData eventData)
        {
            if(eventType == EventTriggerType.Cancel)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnDeselect(BaseEventData eventData)
        {
            if(eventType == EventTriggerType.Deselect)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (eventType == EventTriggerType.Drag)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnDrop(PointerEventData eventData)
        {
            if (eventType == EventTriggerType.Drop)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventType == EventTriggerType.BeginDrag)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (eventType == EventTriggerType.InitializePotentialDrag)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnMove(AxisEventData eventData)
        {
            if (eventType == EventTriggerType.Move)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventType == EventTriggerType.PointerClick)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventType == EventTriggerType.PointerDown)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (eventType == EventTriggerType.PointerEnter)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (eventType == EventTriggerType.PointerExit)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (eventType == EventTriggerType.PointerUp)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnScroll(PointerEventData eventData)
        {
            if (eventType == EventTriggerType.Scroll)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnSelect(BaseEventData eventData)
        {
            if (eventType == EventTriggerType.Select)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnSubmit(BaseEventData eventData)
        {
            if (eventType == EventTriggerType.Submit)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
        public virtual void OnUpdateSelected(BaseEventData eventData)
        {
            if (eventType == EventTriggerType.UpdateSelected)
            {
                DispatchEventForward(gameEventType, eventData);
            }
        }
    }
}
