using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class CreepController
{
    class InterpData
    {
        public InterpData(TSVector start)
        {
            prevPos = currentPos = start;
        }
        public TSVector prevPos;
        public TSVector currentPos;
    }

    private List<Creep> _creepList;

    public bool recalculatePaths { set; get; }


    public CreepController()
    {
        _creepList = new List<Creep>(200);
    }

    public void AddCreep(Creep creep)
    {
        _creepList.Add(creep);
    }

    public void Step(float deltaTime)
    {

        float lerpFactory = deltaTime * 10.0f;
        int count = _creepList.Count;
        for (int i = 0; i < count; ++i)
        {

            //TSTransform tsTransform = _creepList[i];
            //Transform t = tsTransform.transform;
            //tsTransform.UpdatePlayMode();
            //t.position = Vector3.Lerp(t.position, tsTransform.position.ToVector(), deltaTime * 10f);
            //t.position = Vector3.Lerp(t.position, tsTransform.position.ToVector(), lerpFactory);
        }
    }

    public void FixedStep(FP fixedDeltaTime)
    {
        int count = _creepList.Count;
        for(int i = count -1; i >= 0; --i)
        {
            Creep c = _creepList[i];
            if(c.flagForRemoval)
            {
                //TrueSyncManager.SyncedDestroy(c.transform.gameObject);
                GameObject.Destroy(c.transform.gameObject);
                _creepList.RemoveAt(i);
                continue;
            }

            if(recalculatePaths)
            {
                c.RecalculatePath();
            }
            c.FixedStep(fixedDeltaTime);
        }

        recalculatePaths = false;
    }
}
