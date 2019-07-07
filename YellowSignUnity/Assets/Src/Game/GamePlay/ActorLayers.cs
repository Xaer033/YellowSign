using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorLayers
{
    public const string clickCheckName   = "click_check";
    public const string towerName        = "tower";
    public const string creepName        = "creep";

    public static int clickCheckLayer { get; private set; }
    public static int towerLayer { get; private set; }
    public static int creepLayer { get; private set; }

    public static void Init()
    {
        clickCheckLayer = LayerMask.NameToLayer(clickCheckName);
        towerLayer = LayerMask.NameToLayer(towerName);
        creepLayer = LayerMask.NameToLayer(creepName);
    }
}
