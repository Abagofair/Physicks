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

        private Entity _circleEntity;
        private Entity _boxEntity;
        private Entity _polygonEntity;

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
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _circleEntity = new Entity(1);
            if (_world.TryRegisterEntity(_circleEntity.Id, out var circleObject))
            {
                circleObject.Position = new System.Numerics.Vector2(20.0f, 40.0f);
                circleObject.Shape = new CircleShape(20.0f);
            }

            _boxEntity = new Entity(2);
            if (_world.TryRegisterEntity(_boxEntity.Id, out var boxObject))
            {
                boxObject.Position = new System.Numerics.Vector2(50.0f, 40.0f);
                boxObject.Shape = new BoxShape(50.0f, 40.0f);
            }

            _polygonEntity = new Entity(3);
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
            }


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        private List<Vector2> positions = new List<Vector2>();
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_world.TryGetEntity(_circleEntity.Id, out var circleObject))
            {
                circleObject.TorqueSum = 10.0f;
            }

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

            _world.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //_spriteBatch.GraphicsDevice.DrawUserPrimitives

            /*if (_world.TryGetEntity(_circleEntity.Id, out var circleObject))
            {
                var radius = ((CircleShape)circleObject.Shape).Radius;
                var center = new System.Numerics.Vector2(circleObject.Position.X + radius, circleObject.Position.Y + radius);
                _spriteBatch.DrawCircle(center.ToXnaVector2(), radius, 10, Color.Black);
                _spriteBatch.DrawLine(center.ToXnaVector2(), radius, circleObject.Rotation, Color.Black, 1.0f);
            }*/



            if (_world.TryGetEntity(_boxEntity.Id, out var boxObject))
            {
                /*_spriteBatch.DrawPoints(boxObject.Position.ToXnaVector2(),
                    ((BoxShape)boxObject.Shape).Vertices.Select(x => x.ToXnaVector2()).ToList(),
                    Color.Black,
                    1.0f);*/
                var vert = new VertexDeclaration(
                    new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0));
                //var vertices = ((BoxShape)boxObject.Shape).Vertices.Select(x => x.ToXnaVector2()).ToArray();

                var v = Triangulation.FromVertices(((BoxShape)boxObject.Shape).Vertices[0], ((BoxShape)boxObject.Shape).Vertices[1], ((BoxShape)boxObject.Shape).Vertices[2], ((BoxShape)boxObject.Shape).Vertices[3]);

                /*_basicEffect.CurrentTechnique.Passes[0].Apply();
                _spriteBatch.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    vertices,
                    0,
                    1,
                    vert);*/
                var sp = new DebugSpriteRenderer(GraphicsDevice);

                /*var super = MeshHelpers.CreateSuperTriangle(MeshHelpers.GetVertices(v).Select(x => x.Position).ToArray(), 10.0f, 1000);
                var t = MeshHelpers.Delaunay_BowyerWatson(MeshHelpers.GetVertices(v).Select(x => x.Position).ToArray());

                sp.Draw(MeshHelpers.GetVertices(t).Select(x => x.Position.ToXnaVector2()).ToArray(), Matrix.CreateTranslation(new Vector3(100.0f, 100.0f, 0.0f)));

                _spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(new Vector3(100.0f, 100.0f, 0.0f)));
                foreach (var item in ((BoxShape)boxObject.Shape).Vertices)
                {
                    _spriteBatch.DrawCircle(item.ToXnaVector2(), 2.5f, 20, Color.Yellow);
                }
                foreach (var item in super.DistinctVertices())
                {
                    _spriteBatch.DrawCircle(item.Position.ToXnaVector2(), 2.5f, 20, Color.Orange);
                }
                _spriteBatch.End();*/

                _spriteBatch.Begin();
                var t1 = MeshHelpers.Delaunay_BowyerWatson(positions.Select(x => new System.Numerics.Vector2(x.X, x.Y)).ToArray());

                foreach (var item in positions)
                {
                    _spriteBatch.DrawCircle(item, 2.5f, 5, Color.Green);
                }

                sp.Draw(MeshHelpers.GetVertices(t1).Select(x => x.Position.ToXnaVector2()).ToArray());

                _spriteBatch.End();
            }

            /*if (_world.TryGetEntity(_polygonEntity.Id, out var polygonObject))
            {
                _spriteBatch.DrawPoints(polygonObject.Position.ToXnaVector2(),
                    ((PolygonShape)polygonObject.Shape).Vertices.Select(x => x.ToXnaVector2()).ToList(),
                    Color.Black,
                    1.0f);
            }*/
            base.Draw(gameTime);
        }

        private void DrawSpringSystem()
        {
            //(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
            foreach (var connector in _springSystem.Connectors)
            {
                var a = connector.StartAnchor;
                var b = connector.EndAnchor;

                /*_spriteBatch.DrawLine(
                    new Vector2(a.Position.ToXnaVector2().X + (32.0f * 0.3f), a.Position.ToXnaVector2().Y + (32.0f * 0.3f)),
                    new Vector2(b.Position.ToXnaVector2().X + (32.0f * 0.3f), b.Position.ToXnaVector2().Y + (32.0f * 0.3f)),
                    Color.Black,
                    2.5f);*/

                /*_spriteBatch.Draw(
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
                    0.0f);*/
            }
        }
    }
}
