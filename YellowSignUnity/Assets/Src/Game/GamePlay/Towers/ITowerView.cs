using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public interface ITowerView
{ 
    Bounds bounds { get; }

    TSVector position { get; }
    TSQuaternion rotation { get; }
    GameObject gameObject { get; }

    Tower tower { get; set; }

    FP VisualAttack(ICreepView target);
    
}
