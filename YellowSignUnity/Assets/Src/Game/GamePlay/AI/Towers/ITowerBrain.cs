using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public interface ITowerBrain
{ 
    void FixedStep(Tower tower, FP fixedDeltaTime);
}
