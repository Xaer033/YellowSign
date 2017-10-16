using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public enum CreepType
{
    NONE = 0,
    GROUND,
    AIR
}

public class CreepState
{
    public TSVector position;
    public TSQuaternion rotation;

    public int health;
}

