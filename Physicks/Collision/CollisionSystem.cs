namespace Physicks.Collision;

public class CollisionSystem
{
    public event EventHandler<CollisionResult>? OnCollision;

    public CollisionSystem()
    {
        Collideables = new List<Collideable>();
    }

    public List<Collideable> Collideables { get; }

    public void HandleCollisions()
    {
        for (int i = 0; i < Collideables.Count - 1; i++)
        {
            for (int j = i + 1; j < Collideables.Count; j++)
            {
                Collideable a = Collideables[i];
                Collideable b = Collideables[j];

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
            Collideable a,
            Collideable b,
            List<CollisionContact> collisionContacts)
        {
            A = a;
            B = b;
            CollisionContacts = collisionContacts;
        }

        public Collideable A { get; }
        public Collideable B { get; }
        public List<CollisionContact> CollisionContacts { get; }
    }
}