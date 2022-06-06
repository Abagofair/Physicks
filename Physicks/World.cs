using System.Numerics;
using Physicks.Collision;

namespace Physicks;

public class World
{
    private float _accumulator;
    private float _gravity;
    private float _airDrag;

    private List<Particle> _particles;

    private List<Constraint> _constraints;
    private List<PenetrationConstraint> _penetrationContraints;

    private CollisionSystem _collisionSystem;

    private IIntegrator _integrator;

    public World(
        CollisionSystem collisionSystem,
        IIntegrator integrator,
        float airDrag = 0.001f,
        float gravity = 10.0f,
        float pixelsPerMeter = 1.0f,
        float simulationHertz = 60.0f)
    {
        _collisionSystem = collisionSystem ?? throw new ArgumentNullException(nameof(collisionSystem));
        _integrator = integrator ?? throw new ArgumentNullException(nameof(integrator));
        _collisionSystem.OnCollision += OnCollisionHandler;

        _gravity = gravity;
        _airDrag = airDrag;

        MetersPerPixel = 1.0f / pixelsPerMeter;
        PixelsPerMeter = pixelsPerMeter;
        SimulationHertz = simulationHertz;

        _particles = new List<Particle>();
        _constraints = new List<Constraint>();
        _penetrationContraints = new List<PenetrationConstraint>();
    }

    public static float MetersPerPixel { get; private set; }
    public static float PixelsPerMeter { get; private set; }

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

    private void OnCollisionHandler(object? sender, CollisionSystem.CollisionResult e)
    {
        foreach (CollisionContact collisionContact in e.CollisionContacts)
        {
            var pc = new PenetrationConstraint(
                e.A,
                e.B,
                collisionContact.StartPosition,
                collisionContact.EndPosition,
                collisionContact.Normal);

            _penetrationContraints.Add(pc);
        }
    }

    public void RegisterParticle(Particle particle)
    {
        if (particle == null) throw new ArgumentNullException(nameof(particle));

        _particles.Add(particle);
    }

    public void RegisterParticles(IEnumerable<Particle> particles)
    {
        if (particles == null) throw new ArgumentNullException(nameof(particles));

        foreach (Particle particle in particles)
        {
            RegisterParticle(particle);
        }
    }

    public void RegisterConstraint(Constraint constraint)
    {
        if (constraint == null) throw new ArgumentNullException(nameof(constraint));

        _constraints.Add(constraint);
    }

    public void RegisterConstraints(IEnumerable<Constraint> constraints)
    {
        if (constraints == null) throw new ArgumentNullException(nameof(constraints));

        foreach (Constraint body in constraints)
        {
            RegisterConstraint(body);
        }
    }

    public void Update(float dt)
    {
        _penetrationContraints.Clear();

        _accumulator += dt;

        while (_accumulator >= SecondsPerFrame)
        {
            foreach (Collideable collideable in _collisionSystem.Collideables.Where(x => x.Particle.Type != ParticleType.Static))
            {
                //physicsObject.AddForce(CreateDragForce(physicsObject, _airDrag * MetersPerPixel));
                collideable.Particle.AddForce(new Vector2(0.0f, collideable.Shape.Mass * 9.8f * 50.0f));
                _integrator.IntegrateForces(collideable.Particle, collideable.Shape, dt);
            }

            _collisionSystem.HandleCollisions();

            var constraintsToSolve = _constraints.Concat(_penetrationContraints);

            foreach (Constraint constraint in constraintsToSolve)
            {
                constraint.PreSolve(dt);
            }

            for (int i = 0; i < 10; i++)
            {
                foreach (Constraint constraint in constraintsToSolve)
                {
                    constraint.Solve();
                }
            }

            foreach (Constraint constraint in constraintsToSolve)
            {
                constraint.PostSolve();
            }

            foreach (Particle particle in _particles)
            {
                _integrator.IntegrateVelocities(particle, dt);
            }

            _accumulator -= dt;
            ElapsedTimeMilliseconds += dt;
        }
    }
}
