using Rhino.Geometry;
using RHINOMESH = Rhino.Geometry.Mesh;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using System.Numerics;

namespace GltfAttributesExporter.Models
{
    public static class VertexUtility
    {
        public static VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty> CreateVertexBuilderWithUV(RHINOMESH mesh, int vertexIndex)
        {
            var position = mesh.Vertices[vertexIndex];
            var normal = mesh.Normals[vertexIndex];
            var uv = mesh.TextureCoordinates.Count > vertexIndex ? mesh.TextureCoordinates[vertexIndex] : Point2f.Unset;
            if (uv == Point2f.Unset) uv = new Point2f(0, 0);

            return new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                new VertexPositionNormal(position.X, position.Z, -position.Y, normal.X, normal.Z, -normal.Y),
                new VertexTexture1(new Vector2(uv.X, 1.0f - uv.Y))
            );
        }
    }
}
