using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class TestAI : TrueSyncBehaviour
{
    public float speed;

    public Transform visual;
    public float rotationSpeed;

    public float randTurnTime;

    TSVector acceleration;
    TSVector velocity;

    FP turnTimer = 0.0f;
    FP randDir_1;
    FP randDir_2;

    Quaternion prevRot, currentRot;
    public override void OnSyncedStart()
    {
        velocity = tsTransform.forward;
        turnTimer = randTurnTime;
        prevRot = currentRot = visual.rotation;
    }

    public void Update()
    {
        visual.rotation = Quaternion.RotateTowards(prevRot, currentRot, Time.deltaTime * rotationSpeed);
    }

    public override void OnSyncedUpdate()
    {
        FP delta = TrueSyncManager.DeltaTime;
        if(turnTimer <= 0.0f)
        {
            randDir_1 = TSRandom.Range(-1.0f, 1.0f);
            randDir_2 = TSRandom.Range(-1.0f, 1.0f);
            turnTimer = randTurnTime;
        }

        TSVector acceleration = new TSVector(randDir_1, 0, randDir_2).normalized * speed;// * delta;
        tsRigidBody.AddForce(acceleration * delta, ForceMode.Force);

        turnTimer -= delta;

        Vector3 lookDir = tsRigidBody.velocity.ToVector();
        lookDir = lookDir.WithY(0);
        lookDir = (lookDir.sqrMagnitude < 0.001f) ? new Vector3(0, 0, 1.0f) : lookDir;
        if (tsRigidBody.interpolation == TSRigidBody.InterpolateMode.Interpolate)
        {
            prevRot = currentRot;
            currentRot = Quaternion.LookRotation(lookDir, Vector3.up);        
        }
        else
        {
            prevRot = currentRot = Quaternion.LookRotation(lookDir, Vector3.up);
        }
    }
}
