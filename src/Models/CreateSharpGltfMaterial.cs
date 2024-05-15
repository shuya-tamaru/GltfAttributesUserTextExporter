using System.IO;
using SharpGLTF.Materials;
using Rhino.DocObjects;

using System.Numerics;

namespace GltfAttributesExporter.Models { 
    public static  class CreateSharpGltfMaterial
    {
        public static MaterialBuilder CreateMaterial(Material rhinoMaterial, string materialName)
        {
            MaterialBuilder materialBuilder = new MaterialBuilder(materialName)
                .WithDoubleSide(true)
                .WithMetallicRoughnessShader()
                .WithBaseColor(
                     new Vector4(
                         (float)rhinoMaterial.DiffuseColor.R / 255,
                         (float)rhinoMaterial.DiffuseColor.G / 255,
                         (float)rhinoMaterial.DiffuseColor.B / 255,
                         1.0f - (float)rhinoMaterial.Transparency)
                );

            var texture = rhinoMaterial.GetBitmapTexture();
            if (texture != null)
            {
                var texturePath = texture.FileReference?.FullPath;
                if (!string.IsNullOrEmpty(texturePath) && File.Exists(texturePath))
                {

                    materialBuilder.WithChannelImage(KnownChannel.BaseColor, texturePath);
                }
            }
            return materialBuilder;
        }
    }
}
