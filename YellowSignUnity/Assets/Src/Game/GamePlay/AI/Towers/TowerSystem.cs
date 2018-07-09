﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class TowerSystem
{
    private List<Tower> _towerList;

    public TowerSystem()
    {
        _towerList = new List<Tower>(200);
    }

    public void AddTower(Tower tower)
    {
        _towerList.Add(tower);
    }

    public void Step(float deltaTime)
    {
        float lerpFactory = deltaTime * 10.0f;
        int count = _towerList.Count;
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
        int count = _towerList.Count;
        for (int i = count - 1; i >= 0; --i)
        {
            Tower t = _towerList[i];
            t.FixedStep(fixedDeltaTime);
        }
        
    }
}