namespace Examples
{
    public class Entity
    {
        private const string NoName = "NoName";

        public Entity(
            int id,
            string name = null)
        {
            Id = id;
            Name = name ?? NoName;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
