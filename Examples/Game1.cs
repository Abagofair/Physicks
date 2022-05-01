using GameUtilities.Entities;
using GameUtilities.Meshes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameUtilities;
using MonoGameUtilities.Rendering;
using Physicks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Examples
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Entity _boxEntity;
        private Entity _polygonEntity;

        private World _world;

        private DebugSpriteRenderer _debugSpriteRenderer;

        private Entities<EntityContext> _entities;

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
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            var boxShapeEntityContext = new EntityContext();
            var boxShape = new BoxShape(50.0f, 50.0f);
            boxShapeEntityContext.AddOrOverride(new PhysicsObject()
            {
                Position = new System.Numerics.Vector2(200.0f, 40.0f),
                Shape = boxShape
            });
            var t = MeshHelpers.Delaunay_BowyerWatson(boxShape.Vertices);
            boxShapeEntityContext.AddOrOverride(new Renderable(GraphicsDevice, MeshHelpers.GetVertices(t).Select(x => x.Position.ToXnaVector2()).Select(x => new Vertex(new Vector3(x.X, x.Y, 0.0f), new Vector2())).ToArray()));
            _boxEntity = _entities.RegisterEntity(boxShapeEntityContext);

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

        private List<Vector2> positions = new List<Vector2>();
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            /*if (_world.TryGetEntity(_boxEntity.Id, out var box))
            {
                box.TorqueSum = 100000.0f;
            }*/

            var mstate = Mouse.GetState();
            if (mstate.LeftButton == ButtonState.Pressed)
            {
                positions.Add(mstate.Position.ToVector2());
                positions = positions.Distinct().ToList();
            }

            if (mstate.RightButton == ButtonState.Pressed)
            {
                positions.Clear();
            }

            _world.Update(_entities.Query<PhysicsObject>(), (float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            foreach ((Renderable renderable, PhysicsObject physicsObject) in _entities.Query<Renderable, PhysicsObject>())
            {
                _debugSpriteRenderer.Draw(renderable, physicsObject.Transform.ToXnaMatrix4x4());

                /*_spriteBatch.Begin(transformMatrix: physicsObject.Transform.ToXnaMatrix4x4());
                foreach (var item in ((BoxShape)physicsObject.Shape).Vertices)
                {
                    _spriteBatch.DrawCircle(item.ToXnaVector2(), 2.5f, 20, Color.Yellow);
                }
                _spriteBatch.End();*/
            }

            //if (_world.TryGetEntity(_boxEntity.Id, out var boxObject))
            //{
            //    var t = MeshHelpers.Delaunay_BowyerWatson(((BoxShape)boxObject.Shape).Vertices);

            //    _debugSpriteRenderer.Draw(MeshHelpers.GetVertices(t).Select(x => x.Position.ToXnaVector2()).ToArray(), boxObject.Transform.ToXnaMatrix4x4());

            //    _spriteBatch.Begin(transformMatrix: boxObject.Transform.ToXnaMatrix4x4());
            //    foreach (var item in ((BoxShape)boxObject.Shape).Vertices)
            //    {
            //        _spriteBatch.DrawCircle(item.ToXnaVector2(), 2.5f, 20, Color.Yellow);
            //    }
            //    _spriteBatch.End();

            //}
            base.Draw(gameTime);
        }
    }
}
