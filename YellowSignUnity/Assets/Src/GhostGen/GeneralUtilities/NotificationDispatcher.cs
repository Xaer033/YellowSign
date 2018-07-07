using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace GhostGen
{
    public class Event
    {
        public string type;
        public object target;
        public Hashtable data;
    }

    public class NotificationDispatcher  
    {
        private Dictionary<string, List<Action<GhostGen.Event>>> _eventDictionary;
   
        public NotificationDispatcher()
        {
            _eventDictionary = new Dictionary<string, List<Action<GhostGen.Event>>>();
        }

        public void AddListener(string eventKey, Action<GhostGen.Event> callback)
        {
            Assert.IsNotNull(callback);
            if (callback == null) { return; }

            List<Action<GhostGen.Event>> callbackList = null;
            if (!_eventDictionary.TryGetValue(eventKey, out callbackList))
            {
                callbackList = new List<Action<GhostGen.Event>>();
                _eventDictionary.Add(eventKey, callbackList);
            }
            callbackList.Add(callback);
        }

        public void RemoveListener(string eventKey, Action<GhostGen.Event> callback)
        {
            Assert.IsNotNull(callback);
            if (callback == null) { return; }

            List<Action<GhostGen.Event>> callbackList = null;
            if (_eventDictionary.TryGetValue(eventKey, out callbackList))
            {
                int index = callbackList.FindIndex((x) => x == callback);
                callbackList.RemoveAt(index);
            }
        }

        public void RemoveAllListeners(string eventKey)
        {
            List<Action<GhostGen.Event>> callbackList = null;
            if (_eventDictionary.TryGetValue(eventKey, out callbackList))
            {
                callbackList.Clear();
            }
        }

        public void DispatchEvent(string eventKey, Hashtable eventData = null)
        {
            List<Action<GhostGen.Event>> callbackList = null;
            if (_eventDictionary.TryGetValue(eventKey, out callbackList))
            {
                GhostGen.Event e = new GhostGen.Event();
                e.type = eventKey;
                e.target = this;
                e.data = eventData;

                int length = callbackList.Count;
                for(int i = 0; i < length; ++i)
                {
                    if(callbackList[i] != null)
                    {
                        callbackList[i].Invoke(e);
                    }
                }
            }
        }
    }
}