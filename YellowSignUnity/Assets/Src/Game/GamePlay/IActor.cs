using TrueSync;
using UnityEngine;

public interface IActor
{
    Bounds bounds { get; }

    TSVector position { get; }
    TSQuaternion rotation { get; }
    TSTransform transformTS { get; }
    GameObject gameObject { get; }
    
    bool isSelected { get; set; }
}
