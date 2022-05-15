namespace Physicks.Collision;

public class CollisionSystem
{
    public event EventHandler<CollisionResult>? OnCollision;

    public void HandleCollisions(ICollideable[] collideables)
    {
        for (int i = 0; i < collideables.Length - 1; i++)
        {
            for (int j = i + 1; j < collideables.Length; j++)
            {
                ICollideable a = collideables[i];
                ICollideable b = collideables[j];

                if (CollisionDetection.IsCollidingCircleCircle(a, b, out CollisionContact? collisionContact) ||
                    CollisionDetection.IsCollidingPolygonPolygon(a, b, out collisionContact) ||
                    CollisionDetection.IsCollidingPolygonCircle(a, b, out collisionContact))
                {
                    var collisionResult = new CollisionResult(
                        a,
                        b,
                        collisionContact);

                    OnCollision?.Invoke(this, collisionResult);
                }
            }
        }
    }

    public class CollisionResult : EventArgs
    {
        public CollisionResult(
            ICollideable a,
            ICollideable b,
            CollisionContact? collisionContact)
        {
            A = a;
            B = b;
            CollisionContact = collisionContact;
        }

        public ICollideable A { get; }
        public ICollideable B { get; }
        public CollisionContact? CollisionContact { get; }
    }
}