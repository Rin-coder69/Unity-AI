using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    public float maxSpeed = 1.0f;
    public float maxForce = 1.0f;

    public virtual Vector3 Acceleration { get; protected set; }
    public virtual Vector3 Velocity { get; protected set; }

    protected virtual void Update()
    {
        Velocity += Acceleration * Time.deltaTime;
        Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);

        transform.position += Velocity * Time.deltaTime;

        Acceleration = Vector3.zero;

        Debug.Log("Movement Update running");
    }

    public virtual void ApplyForce(Vector3 force)
    {
        force = Vector3.ClampMagnitude(force, maxForce);
        Acceleration += force;
    }
}