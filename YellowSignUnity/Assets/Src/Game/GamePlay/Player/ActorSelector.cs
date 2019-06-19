using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSelector
{
    private byte _ownerId;
    private int _maxSelection;
    private RaycastHit[] _hitResults;
    
    public ActorSelector(byte ownerId, int maxSelection)
    {
        _ownerId = ownerId;
        _maxSelection = maxSelection;
        _hitResults = new RaycastHit[_maxSelection];
    }

    public bool CheckPickSelection<T>(Ray ray, int validLayers, out T view) where T: IActor
    {
        bool result = false;
        view = default(T);

        int hitCount = Physics.RaycastNonAlloc(ray, _hitResults, 2000.0f, validLayers, QueryTriggerInteraction.Collide);
        if (hitCount > 0)
        {
            RaycastHit hit = _hitResults[0]; // Only care about the first one
            view = hit.transform.GetComponent<T>();
            result = true;
        }

        return result;
    }
}
