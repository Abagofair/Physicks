using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameUtilities;
using Physicks;
using System;

namespace Examples
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Particle _particle;
        private Particle _particle1;
        private Particle _particle2;
        private Particle _particle3;
        private Particle _particle4;

        private World _world;
        private SpringSystem _springSystem;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / 144.0);

            _world = new World(
                new System.Numerics.Vector2(
                    _graphics.PreferredBackBufferWidth,
                    _graphics.PreferredBackBufferHeight));

            _springSystem = new SpringSystem();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _particle = new Particle(1);
            _particle1 = new Particle(2);
            _particle2 = new Particle(3);
            _particle3 = new Particle(4);
            _particle4 = new Particle(5);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _particle.Texture2D = Content.Load<Texture2D>("circle");
            _particle1.Texture2D = Content.Load<Texture2D>("circle");
            _particle2.Texture2D = Content.Load<Texture2D>("circle");
            _particle3.Texture2D = Content.Load<Texture2D>("circle");
            _particle4.Texture2D = Content.Load<Texture2D>("circle");

            if (_world.TryRegisterEntity(_particle.Id, out PhysicsObject start))
            {
                start.Position = new System.Numerics.Vector2(260.0f, 25.0f);
                start.Collideable = new CircleCollideable()
                {
                    Radius = _particle.Texture2D.Height * 0.3f
                };
                start.Mass = 1.0f;
                //start.IsKinematic = true;
                _world.TryUpdateEntity(_particle.Id, start);
            }

            if (_world.TryRegisterEntity(_particle1.Id, out PhysicsObject end))
            {
                end.Position = new System.Numerics.Vector2(360.0f, 25.0f);
                end.Collideable = new CircleCollideable()
                {
                    Radius = _particle.Texture2D.Height * 0.3f
                };
                end.Mass = 1.0f;
                //end.IsKinematic = true;
                _world.TryUpdateEntity(_particle1.Id, end);
            }

            if (_world.TryRegisterEntity(_particle2.Id, out PhysicsObject start1))
            {
                start1.Position = new System.Numerics.Vector2(260.0f, 100.0f);
                start1.Collideable = new CircleCollideable()
                {
                    Radius = _particle.Texture2D.Height * 0.50f
                };
                start1.Mass = 1.0f;
                //start1.IsKinematic = true;
                _world.TryUpdateEntity(_particle2.Id, start1);
            }

            if (_world.TryRegisterEntity(_particle3.Id, out PhysicsObject end1))
            {
                end1.Position = new System.Numerics.Vector2(360.0f, 100.0f);
                end1.Collideable = new CircleCollideable()
                {
                    Radius = _particle.Texture2D.Height * 0.50f
                };
                end1.Mass = 1.0f;
                //end1.IsKinematic = true;
                _world.TryUpdateEntity(_particle3.Id, end1);
            }

            _springSystem.AddConnector(start, end, 75.0f, -2000.0f);
            _springSystem.AddConnector(end, start, 75.0f, -2000.0f);

            _springSystem.AddConnector(end, start1, 75.0f, -2000.0f);
            _springSystem.AddConnector(start1, end, 75.0f, -2000.0f);

            _springSystem.AddConnector(end, end1, 75.0f, -2000.0f);
            _springSystem.AddConnector(end1, end, 75.0f, -2000.0f);

            _springSystem.AddConnector(start1, end1, 75.0f, -2000.0f);
            _springSystem.AddConnector(end1, start1, 75.0f, -2000.0f);

            _springSystem.AddConnector(start, start1, 75.0f, -2000.0f);
            _springSystem.AddConnector(start1, start, 75.0f, -2000.0f);

            _springSystem.AddConnector(start, end1, 75.0f, -2000.0f);
            _springSystem.AddConnector(end1, start, 75.0f, -2000.0f);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
               // _world.TryAddForce(_particle.Id, new System.Numerics.Vector2(10.0f, 0.0f));
                _world.TryAddForce(_particle.Id, new System.Numerics.Vector2(1000.0f, 0.0f));
            }

            _springSystem.UpdateForces(_world);

            _world.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            DrawSpringSystem();
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawSpringSystem()
        {
            //(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
            foreach (var connector in _springSystem.Connectors)
            {
                var a = connector.StartAnchor;
                var b = connector.EndAnchor;

                _spriteBatch.DrawLine(
                    new Vector2(a.Position.ToXnaVector2().X + (32.0f * 0.3f), a.Position.ToXnaVector2().Y + (32.0f * 0.3f)),
                    new Vector2(b.Position.ToXnaVector2().X + (32.0f * 0.3f), b.Position.ToXnaVector2().Y + (32.0f * 0.3f)),
                    Color.Black,
                    2.5f);

                _spriteBatch.Draw(
                    _particle.Texture2D,
                    a.Position.ToXnaVector2(),
                    null,
                    Color.White,
                    0.0f,
                    new Vector2(0.0f, 0.0f),
                    0.3f,
                    SpriteEffects.None,
                    0.0f);

                _spriteBatch.Draw(
                    _particle1.Texture2D,
                    b.Position.ToXnaVector2(),
                    null,
                    Color.White,
                    0.0f,
                    new Vector2(0.0f, 0.0f),
                    0.3f,
                    SpriteEffects.None,
                    0.0f);
            }
        }
    }
}
