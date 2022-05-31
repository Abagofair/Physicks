using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GameUtilities.EntitySystem;
using GameUtilities.Options;
using GameUtilities.Scene;
using GameUtilities.Serialization.Parsers.Physicks;
using GameUtilities.System;
using GameUtilities.System.Serialization;
using GameUtilities.System.Serialization.Parsers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameUtilities;
using MonoGameUtilities.Rendering;
using MonoGameUtilities.Serialization;
using Physicks;
using Physicks.Collision;

namespace Examples
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private static SpriteBatch _spriteBatch;

        private CollisionSystem _collisionSystem;
        private World _world;

        private DebugSpriteRenderer _debugSpriteRenderer;

        private GameOptions _gameOptions;

        private FileWatcherService _fileWatcherService;
        private SceneLoader _sceneLoader;
        private SceneGraph _sceneGraph;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0 / 144.0);

            _collisionSystem = new CollisionSystem();
            _world = new World(_collisionSystem, new SemiImplicitEuler(), gravity: 0.5f, pixelsPerMeter: 1.0f, simulationHertz: 60.0f);

            _sceneLoader = new SceneLoader(
                new IComponentParser[]
                {
                    new BodyComponentParser(),
                    new RenderableQuadComponentParser()
                });

            _sceneGraph = new SceneGraph();
        }

        private ReadOnlyMemory<Particle> _particles;

        protected override void Initialize()
        {
            _gameOptions = GameOptions.Load("Options.json");

            _graphics.PreferredBackBufferHeight = _gameOptions.Graphics.Display.Height;
            _graphics.PreferredBackBufferWidth = _gameOptions.Graphics.Display.Width;
            _graphics.IsFullScreen = _gameOptions.Graphics.Display.Fullscreen;
            _graphics.ApplyChanges();

            _fileWatcherService = new FileWatcherService("Editor");
            _fileWatcherService.WatchFile("Scene.json",
                (string fileName) =>
                {
                    _sceneGraph = _sceneLoader.Load(Path.Combine(Directory.GetCurrentDirectory(), fileName));
                    _sceneGraph.SetupBuffers(GraphicsDevice);
                    _world.RegisterBodies(_sceneGraph.Entities.Query<Body>());
                });

            //Spring[] springs = new[]
            //{
            //    new Spring(0, 1, 10.0f, 25.0f),
            //    new Spring(1, 2, 10.0f, 25.0f),
            //    new Spring(2, 3, 10.0f, 25.0f),
            //    new Spring(3, 0, 10.0f, 25.0f)
            //};

            //var particles = new Particle[]
            //{
            //    new Particle(new System.Numerics.Vector2(400.0f, 400.0f), 5.0f),
            //    new Particle(new System.Numerics.Vector2(425.0f, 400.0f), 5.0f),
            //    new Particle(new System.Numerics.Vector2(425.0f, 425.0f), 5.0f),
            //    new Particle(new System.Numerics.Vector2(400.0f, 425.0f), 5.0f)
            //};

            //_particles = new ReadOnlyMemory<Particle>(particles);

            //_massSpringSystem = new MassSpringSystem(springs);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _debugSpriteRenderer = new DebugSpriteRenderer(GraphicsDevice);

            //_sceneGraph = _sceneLoader.Load(Path.Combine(Directory.GetCurrentDirectory(), ".//Editor//Scene.json"));
            //_sceneGraph.SetupBuffers(GraphicsDevice);
            //_world.RegisterBodies(_sceneGraph.Entities.Query<Body>());
        }

        bool pressed = false;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !pressed)
            {
                var ec = new EntityContext("a");
                var b = new Body()
                {
                    Position = new System.Numerics.Vector2(Mouse.GetState().X * World.MetersPerPixel, Mouse.GetState().Y * World.MetersPerPixel),
                    Shape = new BoxShape(50.0f, 50.0f),
                    Restitution = 0.02f
                };
                ec.AddOrOverride<Physicks.Body>(b);
                ec.AddOrOverride<RenderableQuad>(new RenderableQuad()
                {
                    IsDrawable = true,
                    Scale = new System.Numerics.Vector2(50.0f, 50.0f)
                });
                _sceneGraph.AddEntity(ec);
                _sceneGraph.SetupBuffers(GraphicsDevice);
                _world.RegisterBody(b);
                pressed = true;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                pressed = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
               var c = _sceneGraph.Entities.Query<Body>().First(x => x.Shape is CircleShape);
               c.Position = new System.Numerics.Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);

            }

            _world.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: Matrix.Identity);


            foreach ((RenderableQuad renderable, Body physicsObject) in _sceneGraph.Entities.Query<RenderableQuad, Body>())
            {
                //if (physicsObject.Shape is CircleShape shape)
                //{ 
                //    //_spriteBatch.DrawCircle(new Vector2(physicsObject.Position.X, physicsObject.Position.Y), shape.Radius, 10, Color.Yellow);
                //}
                //else
                {
                    _debugSpriteRenderer.Draw(renderable, Matrix.CreateScale(renderable.Scale.X, renderable.Scale.Y, 1.0f) * physicsObject.PixelsPerMeterTransform.ToXnaMatrix4x4());
                }

                /*var boxShapeWidthScaled = ((BoxShape)physicsObject.Shape).Width * World.PixelsPerMeter;
                var boxShapeHeightScaled = ((BoxShape)physicsObject.Shape).Height * World.PixelsPerMeter;
                _debugSpriteRenderer.Draw(renderable, Matrix.CreateScale(boxShapeWidthScaled, boxShapeHeightScaled, 1.0f) * physicsObject.PixelsPerMeterTransform.ToXnaMatrix4x4());

                /*_spriteBatch.Begin(transformMatrix: physicsObject.Transform.ToXnaMatrix4x4());
                foreach (var item in ((BoxShape)physicsObject.Shape).Vertices)
                {
                    _spriteBatch.DrawCircle(new Vector2(item.X, item.Y), 2.5f, 20, Color.Yellow);
                }
                _spriteBatch.End();*/
            }

            var span = _particles.Span;
            for (int i = 0; i < span.Length; i++)
            {
                var particle = span[i];
                _spriteBatch.DrawCircle(new Vector2(particle.Position.X, particle.Position.Y), 1.0f, 10, Color.Yellow);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void DebugDrawLine(Vector2 a, Vector2 b)
        {
            _spriteBatch.Begin(transformMatrix: Matrix.Identity);
            _spriteBatch.DrawLine(a, b, Color.LightGreen, 1.25f);
            _spriteBatch.End();
        }
    }
}
