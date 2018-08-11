using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class AbstractTowerView : ITowerView
{
    public Bounds bounds
    {
        get
        {
            return new Bounds();
        }
    }

    public TSVector position { get; set; }
    public TSQuaternion rotation { get; set; }
    public TSTransform transformTS { get; }

    public GameObject gameObject { get; }
    //public Transform transform { get; }

    public Tower tower { get; set; }

    public FP VisualAttack(ICreepView target) { return 0; }
}
