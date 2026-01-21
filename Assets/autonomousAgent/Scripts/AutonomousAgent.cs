using UnityEngine;

public class AutonomousAgent : AIagent
{
    [SerializeField] Movement movement;
    [SerializeField] Perception fleePerception;
    [SerializeField] Perception seekPerception;
    [Header("Wander")]

    [SerializeField] float wanderRadius = 1;

    [SerializeField] float wanderDistance = 1;

    [SerializeField] float wanderDisplacement = 1;

    float wanderAngle = 0.0f;


    void Start()
    {
        // random angle within a full circle (0–360 degrees)
        wanderAngle = Random.Range(0f, 360f);
    }

    void Update()
    {
        // store if target found, used for wander if no target
        bool hasTarget = false;
        Vector3 force;

        // ---------- FLEE (highest priority) ----------
        if (fleePerception != null)
        {
            var fleeTargets = fleePerception.GetGameObjects();
            if (fleeTargets.Length > 0)
            {
                hasTarget = true;
                force = Flee(fleeTargets[0]);
                movement.ApplyForce(force);
            }
        }

        // ---------- SEEK ----------
        if (!hasTarget && seekPerception != null)
        {
            var seekTargets = seekPerception.GetGameObjects();
            if (seekTargets.Length > 0)
            {
                hasTarget = true;
                force = Seek(seekTargets[0]);
                movement.ApplyForce(force);
            }
        }

        // ---------- WANDER ----------
        // if no target then wander
        if (!hasTarget)
        {
            force = Wander();
            movement.ApplyForce(force);
        }

        // ---------- Wrap world ----------
        transform.position = Utilities.Wrap(
            transform.position,
            new Vector3(-15, -15, -15),
            new Vector3(15, 15, 15)
        );

        // ---------- Face movement direction ----------
        if (movement.Velocity.sqrMagnitude > 0)
            transform.rotation = Quaternion.LookRotation(movement.Velocity, Vector3.up);
    }



    // ---------- Steering Behaviors ----------

    Vector3 Seek(GameObject gameObject)
    {
        Vector3 direction = gameObject.transform.position - transform.position;
        return GetSteeringForce(direction);
    }

    Vector3 Flee(GameObject gameObject)
    {
        Vector3 direction = transform.position - gameObject.transform.position;
        return GetSteeringForce(direction);
    }

    Vector3 GetSteeringForce(Vector3 direction)
    {
        Vector3 desired = direction.normalized * movement.maxSpeed;
        Vector3 steer = desired - movement.Velocity;
        return Vector3.ClampMagnitude(steer, movement.maxForce);
    }

    private Vector3 Wander()
    {
        // randomly adjust the wander angle within (+/-) displacement range
        wanderAngle += Random.Range(-wanderDisplacement, wanderDisplacement);

        // calculate a point on the wander circle using the wander angle
        Quaternion rotation = Quaternion.AngleAxis(wanderAngle, Vector3.up);
        Vector3 pointOnCircle = rotation * (Vector3.forward * wanderRadius);

        // project the wander circle in front of the agent
        Vector3 forward = movement.Velocity.sqrMagnitude > 0.001f
            ? movement.Velocity.normalized
            : transform.forward;

        Vector3 circleCenter = forward * wanderDistance;

        // steer toward the target point
        Vector3 force = GetSteeringForce(circleCenter + pointOnCircle);

        return force;
    }


}