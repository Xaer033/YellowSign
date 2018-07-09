using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GhostGen
{
    [CustomEditor(typeof(EventForwarder))]
    public class EventForwarderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Target.eventType = (EventTriggerType)EditorGUILayout.EnumPopup("Event Trigger Type", Target.eventType);
            Target.gameEventType = EditorGUILayout.DelayedTextField("Game Event Type", Target.gameEventType);
            Target.withBubbling = EditorGUILayout.Toggle("With Bubbling", Target.withBubbling);
               

        }
    
        private EventForwarder Target
        {
            get
            {
                return (EventForwarder)target;
            }
        }
    }
}
