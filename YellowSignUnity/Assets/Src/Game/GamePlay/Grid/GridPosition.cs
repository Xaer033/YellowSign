using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

[System.Serializable]
public struct GridPosition
{
    public int x;
    public int z;

    public static GridPosition Create(int _x, int _z)
    {
        GridPosition pos = new GridPosition();
        pos.x = _x;
        pos.z = _z;
        return pos;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, 0, z);
    }

    public TSVector ToTSVector()
    {
        return new TSVector(x, 0, z);
    }
}
