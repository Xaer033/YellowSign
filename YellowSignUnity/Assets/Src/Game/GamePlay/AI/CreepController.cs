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
    private List<TSTransform> _creepList;
    private List<InterpData> _interpData;

    public CreepController()
    {
        _creepList = new List<TSTransform>(200);
        _interpData = new List<InterpData>(200);
    }

    public void AddCreep(TSTransform creepTransform)
    {
        _creepList.Add(creepTransform);
        _interpData.Add(new InterpData(creepTransform.position));
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

        FP time = TrueSyncManager.Time;
        TSVector velocity = new TSVector(TSMath.Cos(time), 0, TSMath.Sin(time)) * 2.0f; 

        int count = _creepList.Count;
        for(int i = 0; i < count; ++i)
        {
            TSTransform t = _creepList[i];
            InterpData d = _interpData[i];

            t.position = d.prevPos + velocity;
            
            //Vector3 euler = t.transform.eulerAngles;
            //Quaternion useRot = Quaternion.Euler(euler.x, euler.y + 60.0f * fixedDeltaTime.AsFloat(), euler.z);
            //t.rotation = new TSQuaternion(useRot.x, useRot.y, useRot.z, useRot.w);
           // t.Rotate(TSVector.up, 60.0f * fixedDeltaTime);
            
        }
    }
}
