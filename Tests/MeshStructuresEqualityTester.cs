using GameUtilities.Triangulation;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xunit;

namespace Tests
{
    public class MeshStructuresEqualityTester
    {
        [Fact]
        public void VertexEquality_1()
        {
            var vertexA = new Vertex(new Vector2(10.0f, 10.0f));
            var vertexB = new Vertex(new Vector2(10.0f, 10.0f));
            Assert.Equal(vertexA, vertexB);
        }

        [Fact]
        public void VertexInEquality_2()
        {
            var vertexA = new Vertex(new Vector2(10.0f, 10.0f));
            var vertexB = new Vertex(new Vector2(20.0f, 20.0f));
            Assert.NotEqual(vertexA, vertexB);
        }

        [Fact]
        public void VertexInEquality_3()
        {
            var vertexA = new Vertex(new Vector2(10.0f, 20.0f));
            var vertexB = new Vertex(new Vector2(20.0f, 10.0f));
            Assert.NotEqual(vertexA, vertexB);
        }

        [Fact]
        public void EdgeEquality_1()
        {
            var edgeA = new Edge(
                new Vertex(new Vector2(10.0f, 10.0f)),
                new Vertex(new Vector2(20.0f, 20.0f)));

            var edgeB = new Edge(
                new Vertex(new Vector2(10.0f, 10.0f)),
                new Vertex(new Vector2(20.0f, 20.0f)));

            Assert.Equal(edgeA, edgeB);
        }

        [Fact]
        public void EdgeEquality_2()
        {
            var edgeA = new Edge(
                new Vertex(new Vector2(10.0f, 10.0f)),
                new Vertex(new Vector2(20.0f, 20.0f)));

            var edgeB = new Edge(
                new Vertex(new Vector2(20.0f, 20.0f)),
                new Vertex(new Vector2(10.0f, 10.0f)));

            Assert.Equal(edgeA, edgeB);
        }

        [Fact]
        public void EdgeEquality_3()
        {
            var edgeA = new Edge(
                new Vertex(new Vector2(10.0f, 10.0f)),
                new Vertex(new Vector2(20.0f, 20.0f)));

            var edgeB = edgeA;

            Assert.Equal(edgeA, edgeB);
        }

        [Fact]
        public void EdgeInEquality_1()
        {
            var edgeA = new Edge(
                new Vertex(new Vector2(10.0f, 10.0f)),
                new Vertex(new Vector2(20.0f, 20.0f)));

            var edgeB = new Edge(
                new Vertex(new Vector2(20.0f, 10.0f)),
                new Vertex(new Vector2(10.0f, 10.0f)));

            Assert.NotEqual(edgeA, edgeB);
        }

        [Fact]
        public void EdgeEqualityInList()
        {
            var edgeA = new Edge(
                new Vertex(new Vector2(10.0f, 10.0f)),
                new Vertex(new Vector2(20.0f, 20.0f)));

            var edgeB = new Edge(
                new Vertex(new Vector2(20.0f, 20.0f)),
                new Vertex(new Vector2(10.0f, 10.0f)));

            var edgeC = new Edge(
                new Vertex(new Vector2(10.0f, 10.0f)),
                new Vertex(new Vector2(20.0f, 20.0f)));

            var edges = new List<Edge>
            {
                edgeA,
                edgeB,
                edgeC
            };

            edges = edges.Distinct().ToList();

            Assert.Single(edges);
        }
    }
}