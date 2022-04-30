using System.Numerics;

namespace Physicks;

public class World
{
    private readonly Vector2 _worldBounds;
    private float _accumulator;
    private float _gravity;
    private float _pixelPerMeterGravity;
    private float _airDrag;
    private float _pixelPerMeterAirDrag;

    private Dictionary<int, PhysicsObject> _objects;
    //private Memory<Physics2DObject> _objects;

    public World(
        Vector2 worldBounds,
        float airDrag = 0.001f,
        float gravity = 9.82f,
        int pixelsPerMeter = 50,
        float simulationHertz = 60.0f)
    {
        _gravity = gravity;
        _pixelPerMeterGravity = _gravity * pixelsPerMeter;
        _airDrag = airDrag;
        _pixelPerMeterAirDrag = _airDrag * pixelsPerMeter;

        _worldBounds = worldBounds;
        PixelsPerMeter = pixelsPerMeter;
        SimulationHertz = simulationHertz;

        _objects = new Dictionary<int, PhysicsObject>();
    }

    public int PixelsPerMeter { get; }

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

    public void Update(float dt)
    {
        _accumulator += dt;

        while (_accumulator >= SecondsPerFrame)
        {
            IntegrateObjects(dt);

            _accumulator -= dt;
            ElapsedTimeMilliseconds += dt;
        }
    }

    //find better way to handle this entity thingy
    //could have a func that handle creation
    public bool TryRegisterEntity(int entityId, out PhysicsObject? pObject)
    {
        pObject = null;

        if (_objects.ContainsKey(entityId))
        {
            return false;
        }

        pObject = new PhysicsObject();
        _objects.Add(entityId, pObject);
        return true;
    }

    public bool TryGetEntity(int entityId, out PhysicsObject? pObject)
    {
        pObject = null;

        if (_objects.ContainsKey(entityId))
        {
            pObject = _objects[entityId];
            return true;
        }

        return false;
    }

    public bool TryUpdateEntity(int entityId, PhysicsObject physics2DObject)
    {
        if (physics2DObject == null) throw new ArgumentNullException(nameof(physics2DObject));

        if (_objects.ContainsKey(entityId))
        {
            _objects[entityId] = physics2DObject;
            return true;
        }

        return false;
    }

    public bool TryAddForce(int entityId, Vector2 force)
    {
        if (_objects.ContainsKey(entityId))
        {
            var pObject = _objects[entityId];
            pObject.AddForce(force);
            return true;
        }

        return false;
    }

    public static Vector2 CreateDragForce(PhysicsObject physics2DObject, float dragCoeff)
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

    public static Vector2 CreateFrictionForce(PhysicsObject physics2DObject, float frictionCoeff)
    {
        if (physics2DObject == null) throw new ArgumentNullException(nameof(physics2DObject));

        return CreateFrictionForce(physics2DObject.Velocity, frictionCoeff);
    }

    public static Vector2 CreateFrictionForce(Vector2 velocity, float frictionCoeff)
        => frictionCoeff * Vector2.Normalize(velocity) * -1.0f;

    public static Vector2 CreateGravitationalForce(PhysicsObject a, 
        PhysicsObject b, float gravitationalCoeff)
    {
        if (a == null) throw new ArgumentNullException(nameof(a));
        if (b == null) throw new ArgumentNullException(nameof(b));

        Vector2 distanceBA = (b.Position - a.Position);
        float distanceBASquared = distanceBA.LengthSquared();

        float attrMag = gravitationalCoeff * (a.Mass * b.Mass) / distanceBASquared;

        return Vector2.Normalize(distanceBA) * attrMag;
    }

    public static Vector2 CreateSpringForce(PhysicsObject physics2DObject, Vector2 anchor, float restLength, float k)
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

    public void HandleWorldBounds(PhysicsObject physicsObject)
    {
        if (TryHandleCircleWorldBoundsCollision(physicsObject))
            return;
    }

    private void IntegrateObjects(float dt)
    {
        foreach (PhysicsObject physicsObject in _objects.Values)
        {
            if (!physicsObject.IsKinematic)
            {
                //physicsObject.AddForce(CreateDragForce(physicsObject, _pixelPerMeterAirDrag));
                //physicsObject.AddForce(new Vector2(0.0f, physicsObject.Mass * _pixelPerMeterGravity));
            }

            physicsObject.Integrate(dt);
            HandleWorldBounds(physicsObject);
        }
    }

    private bool TryHandleCircleWorldBoundsCollision(PhysicsObject physicsObject)
    {
        //todo handle origin
        if (physicsObject?.Collideable is CircleCollideable circleCollideable)
        {
            if (physicsObject.Position.X >= _worldBounds.X - circleCollideable.Radius)
            {
                physicsObject.Position = new Vector2(_worldBounds.X - circleCollideable.Radius, physicsObject.Position.Y);
                physicsObject.Velocity = new Vector2(-(physicsObject.Velocity.X), physicsObject.Velocity.Y);
            }
            else if (physicsObject.Position.X <= 0)
            {
                physicsObject.Position = new Vector2(0.0f, physicsObject.Position.Y);
                physicsObject.Velocity = new Vector2(-(physicsObject.Velocity.X), physicsObject.Velocity.Y);
            }

            if (physicsObject.Position.Y >= _worldBounds.Y - circleCollideable.Radius)
            {
                physicsObject.Position = new Vector2(physicsObject.Position.X, _worldBounds.Y - circleCollideable.Radius);
                physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, -(physicsObject.Velocity.Y));
            }
            else if (physicsObject.Position.Y <= 0)
            {
                physicsObject.Position = new Vector2(physicsObject.Position.X, 0.0f);
                physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, -(physicsObject.Velocity.Y));
            }

            return true;
        }

        return false;
    }
}
