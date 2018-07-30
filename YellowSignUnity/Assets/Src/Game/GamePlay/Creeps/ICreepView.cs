using TrueSync;
using UnityEngine;
using Pathfinding;

public interface ICreepView
{
    Bounds          bounds      { get; }

    TSVector        position    { get; }
    Vector3         targetPosition { get; }
    TSQuaternion    rotation    { get; }
    TSTransform     transformTS { get; }
    GameObject      gameObject  { get; }
    Seeker          seeker      { get; }

    Creep           creep       { get; set; }
}
