﻿using UnityEngine;

public struct SphereCastParams
{
    public Vector3 center;
    public Vector3 halfExtends;
    public Vector3 direction;

    public SphereCastParams(Vector3 c, Vector3 halfExt, Vector3 dir)
    {
        center = c;
        halfExtends = halfExt;
        direction = dir;
    }
}