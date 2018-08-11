using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public interface ITowerView
{ 
    Bounds bounds { get; }

    TSVector position { get; set; }
    TSQuaternion rotation { get; set; }
    TSTransform transformTS { get; }

    Material material { get; }
    Material highlighterMaterial { get; }

    //Transform transform { get; }
    GameObject gameObject { get; }

    Tower tower { get; set; }

    FP VisualAttack(ICreepView target);
    
}
