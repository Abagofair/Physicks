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

                if (CollisionDetection.IsCollidingCircleCircle(a, b, out List<CollisionContact> collisionContacts) ||
                    CollisionDetection.IsCollidingPolygonPolygon(a, b, out collisionContacts) ||
                    CollisionDetection.IsCollidingPolygonCircle(a, b, out collisionContacts))
                {
                    var collisionResult = new CollisionResult(
                        a,
                        b,
                        collisionContacts);

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
            List<CollisionContact> collisionContacts)
        {
            A = a;
            B = b;
            CollisionContacts = collisionContacts;
        }

        public ICollideable A { get; }
        public ICollideable B { get; }
        public List<CollisionContact> CollisionContacts { get; }
    }
}