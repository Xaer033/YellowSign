using TrueSync;
using UnityEngine;
using Pathfinding;

public interface ICreepView : IActor
{
    Vector3         targetPosition { get; }
    Vector3         healthPosition { get;}
    Seeker          seeker      { get; }

    Creep           creep       { get; set; }
}
