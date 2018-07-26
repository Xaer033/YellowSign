﻿using System.Collections;
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

    public TSVector position { get; }

    public TSQuaternion rotation { get; }

    public GameObject gameObject { get; }
}
