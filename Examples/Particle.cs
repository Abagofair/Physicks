using Microsoft.Xna.Framework.Graphics;
using Physicks;
using System.Numerics;

namespace Examples
{
    public class Particle : Entity
    {
        public Particle(
            int id,
            string name = null) : base(id, name)
        {
        }

        public Texture2D Texture2D { get; set; }
    }
}
