using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public abstract class AbstractTowerBrain : ScriptableObject, ITowerBrain
{
    public abstract void FixedStep(Tower tower, FP fixedDeltaTime, CreepSystem creepSystem);
};
