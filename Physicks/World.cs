using System.Numerics;
using Physicks.Collision;

namespace Physicks;

public class World
{
    private float _accumulator;
    private float _gravity;
    private float _pixelPerMeterGravity;
    private float _airDrag;
    private float _pixelPerMeterAirDrag;

    private Dictionary<int, Body> _bodies;

    private CollisionSystem _collisionSystem;

    public World(
        CollisionSystem collisionSystem,
        float airDrag = 0.001f,
        float gravity = 9.82f,
        int pixelsPerMeter = 50,
        float simulationHertz = 60.0f)
    {
        _collisionSystem = collisionSystem ?? throw new ArgumentNullException(nameof(collisionSystem));
        _collisionSystem.OnCollision += OnCollisionHandler;

        _gravity = gravity;
        _pixelPerMeterGravity = _gravity * pixelsPerMeter;
        _airDrag = airDrag;
        _pixelPerMeterAirDrag = _airDrag * pixelsPerMeter;

        PixelsPerMeter = pixelsPerMeter;
        SimulationHertz = simulationHertz;

        _bodies = new Dictionary<int, Body>();
    }

    public static int PixelsPerMeter { get; private set; }

    public float ElapsedTimeMilliseconds { get; private set; }
    public float SecondsPerFrame { get; private set; }

    private float _simulationHertz;
    public float SimulationHertz
    {
        get => _simulationHertz;
        set
        {
            _simulationHertz = value;
            SecondsPerFrame = 1.0f / _simulationHertz;
        }
    }

    public void RegisterBody(Body body)
    {
        if (body == null) throw new ArgumentNullException(nameof(body));
        
        _bodies.Add(body.Id, body);
    }

    private void OnCollisionHandler(object? sender, CollisionSystem.CollisionResult e)
    {
        if (_bodies.TryGetValue(e.A.Id, out Body? bodyA) &&
            _bodies.TryGetValue(e.B.Id, out Body? bodyB))
        {
            e.CollisionContact?.ResolvePenetrationByImpulse(bodyA, bodyB);
        }
    }

    public void RegisterBodies(IEnumerable<Body> bodies)
    {
        if (bodies == null) throw new ArgumentNullException(nameof(bodies));

        foreach (Body body in bodies)
        {
            RegisterBody(body);
        }
    }

    public void Update(float dt)
    {
        _accumulator += dt;

        while (_accumulator >= SecondsPerFrame)
        {
            IntegrateObjects(_bodies.Values, dt);

            _accumulator -= dt;
            ElapsedTimeMilliseconds += dt;
        }
    }

    public static Vector2 CreateDragForce(Body physics2DObject, float dragCoeff)
    {
        if (physics2DObject == null) throw new ArgumentNullException(nameof(physics2DObject));

        return CreateDragForce(physics2DObject.Velocity, dragCoeff);
    }

    public static Vector2 CreateDragForce(Vector2 velocity, float dragCoeff)
    {
        Vector2 dragForce = Vector2.Zero;

        float magSquared = velocity.LengthSquared();
        if (magSquared > 0)
        {
            dragForce = dragCoeff * magSquared * Vector2.Normalize(velocity) * -1.0f;
        }

        return dragForce;
    }

    public static Vector2 CreateFrictionForce(Body physics2DObject, float frictionCoeff)
    {
        if (physics2DObject == null) throw new ArgumentNullException(nameof(physics2DObject));

        return CreateFrictionForce(physics2DObject.Velocity, frictionCoeff);
    }

    public static Vector2 CreateFrictionForce(Vector2 velocity, float frictionCoeff)
        => frictionCoeff * Vector2.Normalize(velocity) * -1.0f;

    public static Vector2 CreateGravitationalForce(Body a,
        Body b, float gravitationalCoeff)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        Vector2 distanceBA = (b.Position - a.Position);
        float distanceBASquared = distanceBA.LengthSquared();

        float attrMag = gravitationalCoeff * (a.Mass * b.Mass) / distanceBASquared;

        return Vector2.Normalize(distanceBA) * attrMag;
    }

    public static Vector2 CreateSpringForce(Body physics2DObject, Vector2 anchor, float restLength, float k)
    {
        if (physics2DObject == null) throw new ArgumentNullException(nameof(physics2DObject));

        Vector2 d = physics2DObject.Position - anchor;

        float displacement = d.Length() - restLength;

        Vector2 springDirection = Vector2.Normalize(d);
        float springMagnitude = -k * displacement;

        Vector2 springForce = springDirection * springMagnitude;
        return springForce;
    }

    public int TransformToWorldUnit(int t) => t * PixelsPerMeter;
    public float TransformToWorldUnit(float t) => t * PixelsPerMeter;
    public double TransformToWorldUnit(double t) => t * PixelsPerMeter;
    public Vector2 TransformToWorldUnit(Vector2 vector2) => vector2 * PixelsPerMeter;

    private void IntegrateObjects(IEnumerable<Body> physicsObjects, float dt)
    {
        foreach (Body physicsObject in physicsObjects)
        {
            if (!physicsObject.IsKinematic)
            {
                physicsObject.AddForce(CreateDragForce(physicsObject, _pixelPerMeterAirDrag));
                physicsObject.AddForce(new Vector2(0.0f, physicsObject.Mass * _pixelPerMeterGravity));
                physicsObject.Integrate(dt);
            }
        }
    }
}
