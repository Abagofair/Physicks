﻿using System;
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
            /*var boxShapeEntityContext = new EntityContext();
            var boxShape = new BoxShape(50.0f, 50.0f);
            boxShapeEntityContext.AddOrOverride(new PhysicsComponent()
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
            boxShapeEntityContext.AddOrOverride(new PhysicsComponent()
            {
                Position = new System.Numerics.Vector2(600.0f, 1000.0f),
                Shape = boxShape,
                Mass = 10.0f,
                IsKinematic = true
            });
            t = MeshHelpers.Delaunay_BowyerWatson(boxShape.Vertices);
            boxShapeEntityContext.AddOrOverride(new Renderable(GraphicsDevice, MeshHelpers.GetVertices(t).Select(x => x.Position.ToXnaVector2()).Select(x => new Vertex(new Vector3(x.X, x.Y, 0.0f), new Vector2())).ToArray()));
            _boxEntity1 = _entities.CreateEntity(boxShapeEntityContext);*/

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
            #endregion

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _debugSpriteRenderer = new DebugSpriteRenderer(GraphicsDevice);

            _sceneGraph = _sceneLoader.Load(Path.Combine(Directory.GetCurrentDirectory(), ".//Editor//Scene.json"));
            _sceneGraph.SetupBuffers(GraphicsDevice);
            _world.RegisterBodies(_sceneGraph.Entities.Query<Body>());
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

            //if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
               var c = _sceneGraph.Entities.Query<Body>().First(x => x.Shape is CircleShape);
               c.Position = new System.Numerics.Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y);

            }

            _collisionSystem.HandleCollisions(_sceneGraph.Entities.Query<Body>().ToArray());

            //_world.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: Matrix.Identity);

            var circleShape = _sceneGraph.Entities.Query<RenderableQuad, Body>().First(x => x.Item2.Shape is CircleShape);
            var circlePosition = circleShape.Item2.Position.ToXnaVector2();
            foreach ((RenderableQuad renderable, Body physicsObject) in _sceneGraph.Entities.Query<RenderableQuad, Body>().Where(x => x.Item2.Shape is BoxShape))
            {
                var polygon = ((BoxShape)physicsObject.Shape);
                for (int i = 0; i < polygon.Vertices.Length; i++)
                {
                    Vector2 va1 = physicsObject.WorldPosition(polygon.Vertices[i]).ToXnaVector2();
                    Vector2 vb1 = physicsObject.WorldPosition(polygon.Vertices[(i + 1) % polygon.Vertices.Length]).ToXnaVector2();

                    Vector2 edge = va1 - vb1;

                    Vector2 va1ToCircle = circlePosition - va1;
                    Vector2 edgeProjected = ((Vector2.Dot(circlePosition - va1, edge)) / edge.Length()) * (Vector2.Normalize(edge));

                    //draw edge
                    _spriteBatch.DrawLine(va1, vb1, Color.LightGreen, 1.5f);

                    var normal = -Vector2.Normalize(new Vector2(edge.Y, -edge.X));

                    //draw edge out-normal
                    _spriteBatch.DrawLine(va1, va1 + (normal * 30.0f), Color.Red, 2.0f);

                    //draw line from vertex to circle center
                    _spriteBatch.DrawLine(va1, circlePosition, Color.Chocolate, 1.5f);

                    //draw the projection of va1ToCircle onto the edge
                    _spriteBatch.DrawLine(va1, va1 + (edgeProjected), Color.DarkBlue, 2.0f);

                    var projToCirc = circlePosition - (edgeProjected);

                    //draw a line from projection of va1ToCircle tip to the circle center 
                    _spriteBatch.DrawLine(va1 + (edgeProjected), circlePosition, Color.DarkSeaGreen, 2.0f);
                    
                    Debug.WriteLine(Vector2.Distance(va1 + (edgeProjected), circlePosition));

                    //if the line from the tip of the va1ToCircle edge projection is less than the circle radius there would be a collision with the edge if that edge is the closest edge to the circle center
                    if (Vector2.Distance(va1 + (edgeProjected), circlePosition) < ((CircleShape)circleShape.Item2.Shape).Radius)
                    {
                        _spriteBatch.DrawCircle(new Vector2(circlePosition.X, circlePosition.Y), ((CircleShape)circleShape.Item2.Shape).Radius, 10, Color.Red);
                    }
                    else
                    {
                        _spriteBatch.DrawCircle(new Vector2(circlePosition.X, circlePosition.Y), ((CircleShape)circleShape.Item2.Shape).Radius, 10, Color.Yellow);
                    }

                    break;
                }
            }

            foreach ((RenderableQuad renderable, Body physicsObject) in _sceneGraph.Entities.Query<RenderableQuad, Body>())
            {
                if (physicsObject.Shape is CircleShape shape)
                { 
                    //_spriteBatch.DrawCircle(new Vector2(physicsObject.Position.X, physicsObject.Position.Y), shape.Radius, 10, Color.Yellow);
                }
                else
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
