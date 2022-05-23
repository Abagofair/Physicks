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
            _world = new World(_collisionSystem, gravity: 0.5f, pixelsPerMeter: 1.0f, simulationHertz: 60.0f);

            _sceneLoader = new SceneLoader(
                new IComponentParser[]
                {
                    new BodyComponentParser(),
                    new RenderableQuadComponentParser()
                });

            _sceneGraph = new SceneGraph();
        }

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

            #region oldentitystuff

            var posA = new Vector2(_gameOptions.Graphics.Display.Width / 2.0f, _gameOptions.Graphics.Display.Height / 2.0f);
            var posB = new Vector2(posA.X - 50.0f, posA.Y - 50.0f);

            var aEntity = new EntityContext("aEntity");
            var bodyA = new Body()
            {
                Position = new System.Numerics.Vector2(posA.X, posA.Y),
                Shape = new BoxShape(50.0f, 50.0f),
                Mass = 0.0f,
                IsKinematic = false
            };
            var renderableQuadA = new RenderableQuad()
            {
                IsDrawable = true,
                Scale = new System.Numerics.Vector2(50.0f, 50.0f)
            };

            aEntity.AddOrOverride(bodyA);
            aEntity.AddOrOverride(renderableQuadA);

            var bEntity = new EntityContext("bEntity");
            var bodyB = new Body()
            {
                Position = new System.Numerics.Vector2(posB.X, posB.Y),
                Shape = new BoxShape(50.0f, 50.0f),
                Mass = 1.0f,
                IsKinematic = false
            };
            var renderableQuadB = new RenderableQuad()
            {
                IsDrawable = true,
                Scale = new System.Numerics.Vector2(50.0f, 50.0f)
            };

            bEntity.AddOrOverride(bodyB);
            bEntity.AddOrOverride(renderableQuadB);

            var cEntity = new EntityContext("cEntity");
            var bodyC = new Body()
            {
                Position = new System.Numerics.Vector2(posB.X - 50.0f, posB.Y - 50.0f),
                Shape = new BoxShape(50.0f, 50.0f),
                Mass = 1.0f,
                IsKinematic = false
            };
            var renderableQuadC = new RenderableQuad()
            {
                IsDrawable = true,
                Scale = new System.Numerics.Vector2(50.0f, 50.0f)
            };

            cEntity.AddOrOverride(bodyC);
            cEntity.AddOrOverride(renderableQuadC);

            _sceneGraph.AddEntity(aEntity);
            _sceneGraph.AddEntity(bEntity);
            _sceneGraph.AddEntity(cEntity);

            _sceneGraph.SetupBuffers(GraphicsDevice);

            var jointConstraintA = new JointConstraint(
                bodyA,
                bodyB,
                bodyA.Position);

            var jointConstraintB = new JointConstraint(
                bodyB,
                bodyC,
                bodyB.Position);

            _world.RegisterConstraint(jointConstraintA);
            _world.RegisterConstraint(jointConstraintB);

            _world.RegisterBody(bodyA);
            _world.RegisterBody(bodyB);
            _world.RegisterBody(bodyC);

            #endregion

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
                    Shape = new BoxShape(50.0f, 50.0f)
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

            _collisionSystem.HandleCollisions(_sceneGraph.Entities.Query<Body>().ToArray());

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
