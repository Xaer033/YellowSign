using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Pathfinding;

public class Creep
{
    public const float REPATH_RATE = 2.0f;

    public CreepState state { get; set; }
    public CreepStats stats { get; set; }

    public TSTransform transform { get { return _transform; } }

    private Seeker _seeker;
    private bool _canSearchAgain;
    private TSTransform _transform;
    private FP _nextRepath;

    private List<Vector3> _vectorPath;
    private int _waypointIndex;
    private FP _distanceToNextWaypoint = 0.5f;
    private Vector3 _target;

    private Path _path = null;

    public bool flagForRemoval { get;  set; }

    public Creep(TSTransform transform)
    {
        _transform = transform;
        _seeker = transform.gameObject.AddComponent<Seeker>();
        _nextRepath = 0;
        _waypointIndex = 0;
    }

    public void Start(Vector3 target)
    {
        _target = target;
        _canSearchAgain = true;

        RecalculatePath();
    }
    
    public void FixedStep(FP fixedDeltaTime)
    {
        if (TrueSyncManager.Time >= _nextRepath && _canSearchAgain)
        {
            RecalculatePath();
        }

        Vector3 pos = _transform.position.ToVector();

        if (_vectorPath != null && _vectorPath.Count != 0)
        {
            while ((_waypointIndex < _vectorPath.Count - 1 && (pos - _vectorPath[_waypointIndex]).sqrMagnitude < _distanceToNextWaypoint * _distanceToNextWaypoint) || _waypointIndex == 0)
            {
                _waypointIndex++;
            }

            // Current path segment goes from vectorPath[wp-1] to vectorPath[wp]
            // We want to find the point on that segment that is 'moveNextDist' from our current position.
            // This can be visualized as finding the intersection of a circle with radius 'moveNextDist'
            // centered at our current position with that segment.
            var p1 = _vectorPath[_waypointIndex - 1];
            var p2 = _vectorPath[_waypointIndex];

            pos += (p2 - p1).normalized * 4 * fixedDeltaTime.AsFloat();

            if((pos - _vectorPath[_vectorPath.Count - 1]).sqrMagnitude < _distanceToNextWaypoint * _distanceToNextWaypoint)
            {
                flagForRemoval = true;
            }
        }
        else
        {
            // Stand still
            pos = _transform.position.ToVector();

        }

        // Rotate the character if the velocity is not extremely small
        //if (Time.deltaTime > 0 && movementDelta.magnitude / Time.deltaTime > 0.01f)
        //{
        //    var rot = transform.rotation;
        //    var targetRot = Quaternion.LookRotation(movementDelta, controller.To3D(Vector2.zero, 1));
        //    const float RotationSpeed = 5;
        //    if (controller.movementPlane == MovementPlane.XY)
        //    {
        //        targetRot = targetRot * Quaternion.Euler(-90, 180, 0);
        //    }
        //    transform.rotation = Quaternion.Slerp(rot, targetRot, Time.deltaTime * RotationSpeed);
        //}

        
        _transform.position = pos.ToTSVector();
        
    }

    public void RecalculatePath()
    {
        _canSearchAgain = false;
        _nextRepath = TrueSyncManager.Time + REPATH_RATE * (TSRandom.value + 1);
        _seeker.StartPath(_transform.position.ToVector(), _target, OnPathComplete);
    }

    public void OnPathComplete(Path _p)
    {
        ABPath p = _p as ABPath;

        _canSearchAgain = true;

        if (_path != null) _path.Release(this);
        _path = p;
        p.Claim(this);

        if (p.error)
        {
            _waypointIndex = 0;
            _vectorPath = null;
            return;
        }


        Vector3 p1 = p.originalStartPoint;
        Vector3 p2 = _transform.position.ToVector();
        p1.y = p2.y;
        float d = (p2 - p1).magnitude;
        _waypointIndex = 0;

        _vectorPath = p.vectorPath;
        Vector3 waypoint;

        if (_distanceToNextWaypoint > 0)
        {
            float dist = _distanceToNextWaypoint.AsFloat() * 0.6f;
            for (float t = 0; t <= d; t += dist )
            {
                _waypointIndex--;
                Vector3 pos = p1 + (p2 - p1) * t;

                do
                {
                    _waypointIndex++;
                    waypoint = _vectorPath[_waypointIndex];
                } while ((pos - waypoint).sqrMagnitude < _distanceToNextWaypoint * _distanceToNextWaypoint && _waypointIndex != _vectorPath.Count - 1);
            }
        }
    }

}
