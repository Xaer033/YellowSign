using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public interface ITowerView : IActor
{ 

    Material material { get; }
    Material highlighterMaterial { get; }


    Tower tower { get; set; }

    FP VisualAttack(ICreepView target);
    
}
