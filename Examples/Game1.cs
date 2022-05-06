using GameUtilities.Entities;
using GameUtilities.Meshes;
using GameUtilities.Options;
using GameUtilities.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameUtilities;
using MonoGameUtilities.Rendering;
using Physicks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Examples
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Entity _boxEntity;
        private Entity _boxEntity1;

        private Entity _circleEntity;
        private Entity _circleEntity1;

        private World _world;

        private DebugSpriteRenderer _debugSpriteRenderer;

        private Entities<EntityContext> _entities;

        private GameOptions _gameOptions;

        private FileWatcherService _fileWatcherService;

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

            _entities = new Entities<EntityContext>(100);

            _fileWatcherService = new FileWatcherService("Editor");
            _fileWatcherService.WatchFile("Scene.json", () => Debug.WriteLine("Changed scene.json"));
        }

        protected override void Initialize()
        {
            _gameOptions = GameOptions.Load("Options.json");

            _graphics.PreferredBackBufferHeight = _gameOptions.Graphics.Display.Height;
            _graphics.PreferredBackBufferWidth = _gameOptions.Graphics.Display.Width;
            _graphics.IsFullScreen = _gameOptions.Graphics.Display.Fullscreen;
            _graphics.ApplyChanges();

            var boxShapeEntityContext = new EntityContext();
            var boxShape = new BoxShape(50.0f, 50.0f);
            boxShapeEntityContext.AddOrOverride(new PhysicsObject()
            {
                Position = new System.Numerics.Vector2(600.0f, 120.0f),
                Shape = boxShape,
                Mass = 5.0f,
                Rotation = -0.5f,
                Restitution = 1.0f
            });
            var t = MeshHelpers.Delaunay_BowyerWatson(boxShape.Vertices);
            boxShapeEntityContext.AddOrOverride(new Renderable(GraphicsDevice, MeshHelpers.GetVertices(t).Select(x => x.Position.ToXnaVector2()).Select(x => new Vertex(new Vector3(x.X, x.Y, 0.0f), new Vector2())).ToArray()));
            _boxEntity = _entities.CreateEntity(boxShapeEntityContext);

            boxShapeEntityContext = new EntityContext();
            boxShape = new BoxShape(50.0f, 50.0f);
            boxShapeEntityContext.AddOrOverride(new PhysicsObject()
            {
                Position = new System.Numerics.Vector2(600.0f, 1000.0f),
                Shape = boxShape,
                Mass = 10.0f,
                IsKinematic = true
            });
            t = MeshHelpers.Delaunay_BowyerWatson(boxShape.Vertices);
            boxShapeEntityContext.AddOrOverride(new Renderable(GraphicsDevice, MeshHelpers.GetVertices(t).Select(x => x.Position.ToXnaVector2()).Select(x => new Vertex(new Vector3(x.X, x.Y, 0.0f), new Vector2())).ToArray()));
            _boxEntity1 = _entities.CreateEntity(boxShapeEntityContext);

            /*var circleShapeEntityContext = new EntityContext();
            var circleShape = new CircleShape(50.0f);
            circleShapeEntityContext.AddOrOverride(new PhysicsObject()
            {
                Position = new System.Numerics.Vector2(300.0f, 300.0f),
                Shape = circleShape,
                IsKinematic = true,
                Mass = 1.0f,
                Restitution = 0.2f
            });

            var points = MeshHelpers.PointsFromCircle(circleShape.Radius, 15);
            t = MeshHelpers.Delaunay_BowyerWatson(points);
            circleShapeEntityContext.AddOrOverride(new Renderable(GraphicsDevice, MeshHelpers.GetVertices(t).Select(x => x.Position.ToXnaVector2()).Select(x => new Vertex(new Vector3(x.X, x.Y, 0.0f), new Vector2())).ToArray()));
            _circleEntity = _entities.CreateEntity(circleShapeEntityContext);

            circleShapeEntityContext = new EntityContext();
            circleShape = new CircleShape(40.0f);
            circleShapeEntityContext.AddOrOverride(new PhysicsObject()
            {
                Position = new System.Numerics.Vector2(300.0f, 60.0f),
                Shape = circleShape,
                IsKinematic = false,
                Mass = 10.0f,
                Restitution = 0.9f
            });
            points = MeshHelpers.PointsFromCircle(circleShape.Radius, 9);
            t = MeshHelpers.Delaunay_BowyerWatson(points);
            circleShapeEntityContext.AddOrOverride(new Renderable(GraphicsDevice, MeshHelpers.GetVertices(t).Select(x => x.Position.ToXnaVector2()).Select(x => new Vertex(new Vector3(x.X, x.Y, 0.0f), new Vector2())).ToArray()));
            _circleEntity1 = _entities.CreateEntity(circleShapeEntityContext);*/

            /*_polygonEntity = new Entity(3);
            if (_world.TryRegisterEntity(_polygonEntity.Id, out var polygonObject))
            {
                polygonObject.Position = new System.Numerics.Vector2(200.0f, 40.0f);
                polygonObject.Shape = new PolygonShape()
                {
                    Vertices = new System.Numerics.Vector2[]
                    {
                        new System.Numerics.Vector2(50.0f, 40.0f),
                        new System.Numerics.Vector2(100.0f, 40.0f),
                        new System.Numerics.Vector2(75.0f, 80.0f),
                        new System.Numerics.Vector2(50.0f, 40.0f)
                    }
                };
            }*/

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _debugSpriteRenderer = new DebugSpriteRenderer(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _world.HandleCollisions(_entities.Query<PhysicsObject>());

            /*if (_world.TryGetEntity(_boxEntity.Id, out var box))
            {
                box.TorqueSum = 100000.0f;
            }*/

            EntityContext circleContext = _entities.GetEntityContext(ref _circleEntity);
            /*circleContext.Query<PhysicsObject>().Position = new System.Numerics.Vector2(
                Mouse.GetState().X, Mouse.GetState().Y);*/

            _world.Update(_entities.Query<PhysicsObject>(), (float)gameTime.ElapsedGameTime.TotalSeconds);
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach ((Renderable renderable, PhysicsObject physicsObject) in _entities.Query<Renderable, PhysicsObject>())
            {
                _debugSpriteRenderer.Draw(renderable, physicsObject.Transform.ToXnaMatrix4x4());

                //_spriteBatch.Begin(transformMatrix: physicsObject.Transform.ToXnaMatrix4x4());
                //foreach (var item in (renderable.Vertices))
                //{
                //    _spriteBatch.DrawCircle(new Vector2(item.Position.X, item.Position.Y), 2.5f, 20, Color.Yellow);
                //}
                //_spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
