using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectUtilities
{
    public static void DestroyAllChildren(Transform root)
    {
        if(root == null) { return; }

        int count = root.childCount;
        for(int i = count - 1; i >= 0; --i)
        {
            GameObject.Destroy(root.GetChild(i).gameObject);
        }
    }
}
