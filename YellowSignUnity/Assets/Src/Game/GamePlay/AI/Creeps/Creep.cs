using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Pathfinding;

public class Creep
{
    public FP REPATH_RATE = 2.5f;

    public CreepState state { get; set; }
    public CreepStats stats { get; set; }

    public TSTransform transform { get { return _transform; } }

    private Seeker _seeker;
    private bool _canSearchAgain;
    private TSTransform _transform;
    private FP _nextRepath;

    private List<TSVector> _vectorPath;
    private int _waypointIndex;
    private FP _distanceToNextWaypoint = 0.35f;
    private Vector3 _target;

    private Path _path = null;

    public bool flagForRemoval { get;  set; }

    public Creep(TSTransform transform)
    {
        _transform = transform;
        _seeker = transform.gameObject.GetComponent<Seeker>();
        //_seeker.
        _nextRepath = 0;
        _waypointIndex = 0;
        _vectorPath = new List<TSVector>();
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

        TSVector pos = _transform.position;

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

            pos += (p2 - p1).normalized * 1.5f * fixedDeltaTime;

            if((pos - _vectorPath[_vectorPath.Count - 1]).sqrMagnitude < _distanceToNextWaypoint * _distanceToNextWaypoint)
            {
                flagForRemoval = true;
            }
        }
        else
        {
            // Stand still
            pos = _transform.position;

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

        
        _transform.position = pos;
        
    }

    public void RecalculatePath()
    {
        _canSearchAgain = false;
        _nextRepath = TrueSyncManager.Time + REPATH_RATE;// * (TSRandom.value + 1);
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


        TSVector p1 = p.originalStartPoint.ToTSVector();
        TSVector p2 = _transform.position;
        p1.y = p2.y;
        FP d = (p2 - p1).magnitude;
        _waypointIndex = 0;

        _vectorPath.Clear();
        for(int i = 0; i < p.vectorPath.Count; ++i)
        {
            _vectorPath.Add(p.vectorPath[i].ToTSVector());
        }
        TSVector waypoint;

        if (_distanceToNextWaypoint > 0)
        {
            float dist = _distanceToNextWaypoint.AsFloat() * 0.6f;
            for (float t = 0; t <= d; t += dist )
            {
                _waypointIndex--;
                TSVector pos = p1 + (p2 - p1) * t;

                do
                {
                    _waypointIndex++;
                    waypoint = _vectorPath[_waypointIndex];
                } while ((pos - waypoint).sqrMagnitude < _distanceToNextWaypoint * _distanceToNextWaypoint && _waypointIndex != _vectorPath.Count - 1);
            }
        }
    }

}
