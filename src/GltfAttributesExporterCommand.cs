using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using RHINOMESH = Rhino.Geometry.Mesh;

using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;

using GltfAttributesExporter.Utilities;
using GltfAttributesExporter.Models;

using System.Numerics;
using System.Text.Json.Nodes;

namespace GltfAttributesExporter
{
    public class GltfAttributesExporterCommand : Command
    {
        public GltfAttributesExporterCommand()
        {
            Instance = this;
            meshConvertSettings = new MeshingParameters(0);
            defaultMaterial = new Material
            {
                Name = "Default",
                DiffuseColor = System.Drawing.Color.White
            };
        }

        public static GltfAttributesExporterCommand Instance { get; private set; }

        private readonly MeshingParameters meshConvertSettings;
        private readonly Material defaultMaterial;

        public override string EnglishName => "GltfAttributesExport";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

            // select objects
            var selectResult= ObjectSelector.SelectObjects("Select objects to mesh");
            if (selectResult == null || selectResult.Geometry == null)
            {
                return Result.Cancel;
            }
            var selectedGeometries = selectResult.Geometry;
            bool groupByLayer= selectResult.GroupByLayer;

            // save file dialog
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "GLB files (*.glb)|*.glb|GLTF files (*.gltf)|*.gltf",
                Title = "Save GLB or GLTF File"
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return Result.Cancel;
            }

            var saveFilePath = saveFileDialog.FileName;

            RhinoApp.WriteLine("Please waiting...");

            // compile mesh data
            var meshData = new List<MeshData>();
            foreach (var objRef in selectedGeometries.Objects())
            {
                try
                {
                    // Convert Brep to Mesh. Work on a copy so export-time UV baking,
                    // triangulation and unit scaling never mutate the document mesh.
                    RHINOMESH mesh = null;
                    var sourceMesh = objRef.Mesh();
                    if (sourceMesh != null)
                    {
                        mesh = sourceMesh.DuplicateMesh();
                    }
                    else if (objRef.Brep() != null)
                    {
                        var brep = objRef.Brep();
                        if (!brep.IsValid)
                        {
                            Rhino.RhinoApp.WriteLine("This Brep is Invalid, skip to export: guid =>" + objRef.ObjectId.ToString());
                            continue;
                        }
                        var brepMeshes = RHINOMESH.CreateFromBrep(brep, meshConvertSettings);
                        if (brepMeshes != null && brepMeshes.Length > 0)
                        {
                            mesh = new RHINOMESH();
                            foreach (var m in brepMeshes)
                            {
                                mesh.Append(m);
                            }
                        }
                    }

                    if (mesh == null || !mesh.IsValid)
                    {
                        RhinoApp.WriteLine("This object is not a valid mesh or brep: guid =>" + objRef.ObjectId.ToString());
                        continue;
                    }

                    //get layer id
                    var layerIndex = objRef.Object().Attributes.LayerIndex;

                    // Get Material
                    var rhinoMaterial = defaultMaterial;
                    if (objRef.Object().Attributes.MaterialSource == ObjectMaterialSource.MaterialFromObject)
                    {
                        rhinoMaterial = doc.Materials[objRef.Object().Attributes.MaterialIndex];
                    }
                    else if (objRef.Object().Attributes.MaterialSource == ObjectMaterialSource.MaterialFromLayer)
                    {
                        var layer = doc.Layers[layerIndex];
                        if (layer.RenderMaterialIndex >= 0)
                        {
                            rhinoMaterial = doc.Materials[layer.RenderMaterialIndex];
                        }
                    }

                    // Rhino object-level Box/Planar/Cylindrical/etc. mappings are not
                    // guaranteed to exist in Mesh.TextureCoordinates. The original exporter
                    // only serializes Mesh.TextureCoordinates, which can collapse the GLB UVs
                    // to VertexUtility's fallback (0,0). Bake the active Rhino mapping into
                    // explicit per-vertex UVs before unit scaling and SharpGLTF serialization.
                    BakeObjectTextureMappingToMesh(objRef.Object(), rhinoMaterial, mesh);

                    // Get UserAttributes
                    var userAttributes = new List<UserAttribute>();
                    var attributes = objRef.Object().Attributes;
                    var keys = attributes.GetUserStrings();
                    foreach (string key in keys)
                    {
                        var value = attributes.GetUserString(key);
                        var isTextField = UserAttribute.ContainsTextFieldFormat(value);

                        if (isTextField)
                        {
                            value = RhinoApp.ParseTextField(value, objRef.Object(), null);
                        }
                        
                        userAttributes.Add(new UserAttribute { key = key, value = value });
   
                    }

                    //Add MeshWithUserData to list
                    if (mesh != null && rhinoMaterial != null)
                    {
                        meshData.Add(new MeshData(mesh, userAttributes, rhinoMaterial, layerIndex));
                    }
                }
                catch (Exception ex)
                {
                    RhinoApp.WriteLine("An error occurred: " + ex.Message);
                }
            }

            // create sharpGLTF Data
            float scaleFactor = Utility.ScaleModel(RhinoDoc.ActiveDoc);
            var sceneBuilder = new SceneBuilder();
            var materialBuilders = new Dictionary<string, MaterialBuilder>();

            foreach (var item in meshData)
            {
                //Scale Mesh to webgl size
                RHINOMESH rhinoMesh = item.Mesh;
                var layerIndex = item.LayerIndex;
                var scaleTransform = Rhino.Geometry.Transform.Scale(Point3d.Origin, scaleFactor);
                rhinoMesh.Transform(scaleTransform);

                //get attributes
                List<UserAttribute> attributes = item.UserAttributes;

                //create sharpGLTF material
                Material rhinoMaterial = item.RhinoMaterial;
                string materialName = !string.IsNullOrEmpty(rhinoMaterial.Name) ? rhinoMaterial.Name : "Material_" + materialBuilders.Count.ToString();
                if (!materialBuilders.TryGetValue(materialName, out MaterialBuilder materialBuilder))
                {
                    materialBuilder = CreateSharpGltfMaterial.CreateMaterial(rhinoMaterial, materialName);
                    materialBuilders.Add(materialName, materialBuilder);
                }

                //create sharpGLTF mesh with attributes
                var meshBuilder = new MeshBuilder<VertexPositionNormal, VertexTexture1>("mesh");
                var prim = meshBuilder.UsePrimitive(materialBuilder);

                rhinoMesh.Faces.ConvertQuadsToTriangles();
                rhinoMesh.Normals.ComputeNormals();
                rhinoMesh.Compact();

                foreach (var face in rhinoMesh.Faces)
                {
                    var vertexA = VertexUtility.CreateVertexBuilderWithUV(rhinoMesh, face.A);
                    var vertexB = VertexUtility.CreateVertexBuilderWithUV(rhinoMesh, face.B);
                    var vertexC = VertexUtility.CreateVertexBuilderWithUV(rhinoMesh, face.C);
                    prim.AddTriangle(vertexA, vertexB, vertexC);
                }

                if (attributes.Count > 0)
                {
                    var attributesDict = new Dictionary<string, string>();
                    foreach (var attribute in item.UserAttributes)
                    {
                        var key = attribute.key;
                        var value = attribute.value;
                        attributesDict.Add(key, value);
                    }

                    var jsonObject = new JsonObject();
                    foreach (var kvp in attributesDict)
                    {
                        jsonObject[kvp.Key] = kvp.Value;
                    }
                    meshBuilder.Extras = jsonObject;
                }

                if (groupByLayer)
                {
                    // Find or create node for the layer
                    var layer = doc.Layers[layerIndex];
                    var layerNode = LayerNodeHelper.GetOrCreateLayerNode(sceneBuilder, layer);
                    var meshNode = new NodeBuilder();
                    layerNode.AddNode(meshNode);


                    //var node =LayerNodeHelper.GetOrCreateLayerNode(sceneBuilder, layer);
                    sceneBuilder.AddRigidMesh(meshBuilder, meshNode, Matrix4x4.Identity);
                }
                else
                {
                    sceneBuilder.AddRigidMesh(meshBuilder, Matrix4x4.Identity);
                }
            }

            //export model
            var model = sceneBuilder.ToGltf2();
            if (Path.GetExtension(saveFilePath).Equals(".glb", StringComparison.OrdinalIgnoreCase))
            {
                model.SaveGLB(saveFilePath);
            }
            else if (Path.GetExtension(saveFilePath).Equals(".gltf", StringComparison.OrdinalIgnoreCase))
            {
                //merge bin file to one buffer
                var writeSettings = new SharpGLTF.Schema2.WriteSettings
                {
                    MergeBuffers = true,
                    JsonIndented = true
                };

                model.SaveGLTF(saveFilePath, writeSettings);
            }
            else
            {
                RhinoApp.WriteLine("Unsupported file format.");
                return Result.Cancel;
            }

            RhinoApp.WriteLine("Exported to: " + saveFilePath);

            return Result.Success;
        }

        /// <summary>
        /// Bake Rhino object-level texture mapping into Mesh.TextureCoordinates (glTF TEXCOORD_0).
        /// Priority: material bitmap channel -> channel 1 -> any other object mapping channels.
        /// Existing mesh UVs are preserved when no object mapping can be resolved.
        /// </summary>
        private static bool BakeObjectTextureMappingToMesh(
            RhinoObject rhinoObject,
            Material rhinoMaterial,
            RHINOMESH mesh)
        {
            if (rhinoObject == null || mesh == null) return false;

            var channels = new List<int>();

            // Prefer the mapping channel explicitly requested by the bitmap texture.
            try
            {
                var bitmapTexture = rhinoMaterial?.GetBitmapTexture();
                if (bitmapTexture != null && bitmapTexture.MappingChannelId > 0)
                {
                    AddUnique(channels, bitmapTexture.MappingChannelId);
                }
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine("Could not inspect bitmap mapping channel: " + ex.Message);
            }

            // Channel 1 is Rhino's conventional primary texture mapping channel.
            AddUnique(channels, 1);

            // Fall back to any other mappings attached to the object.
            try
            {
                var objectChannels = rhinoObject.GetTextureChannels();
                if (objectChannels != null)
                {
                    foreach (var channel in objectChannels)
                    {
                        if (channel > 0) AddUnique(channels, channel);
                    }
                }
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine("Could not inspect object texture channels: " + ex.Message);
            }

            if (mesh.Normals.Count != mesh.Vertices.Count)
            {
                mesh.Normals.ComputeNormals();
            }

            foreach (var channel in channels)
            {
                try
                {
                    Transform objectTransform;
                    var mapping = rhinoObject.GetTextureMapping(channel, out objectTransform);
                    if (mapping == null) continue;

                    // lazy=false: compute immediately. seamCheck=true: allow Rhino to
                    // handle discontinuities needed by box/cylinder/sphere mappings.
                    mesh.SetTextureCoordinates(mapping, objectTransform, false, true);

                    if (HasPerVertexUv(mesh))
                    {
                        RhinoApp.WriteLine(
                            "UV baked: object=" + rhinoObject.Id +
                            ", channel=" + channel +
                            ", mapping=" + mapping.MappingType +
                            ", vertices=" + mesh.Vertices.Count +
                            ", uv=" + mesh.TextureCoordinates.Count +
                            (IsUvDegenerate(mesh) ? " [WARNING: degenerate UV range]" : ""));
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    RhinoApp.WriteLine(
                        "UV bake failed: object=" + rhinoObject.Id +
                        ", channel=" + channel +
                        ", error=" + ex.Message);
                }
            }

            if (HasPerVertexUv(mesh))
            {
                RhinoApp.WriteLine(
                    "No object mapping resolved; preserving existing mesh UVs: object=" + rhinoObject.Id +
                    (IsUvDegenerate(mesh) ? " [WARNING: degenerate UV range]" : ""));
                return true;
            }

            RhinoApp.WriteLine(
                "No usable UVs found: object=" + rhinoObject.Id +
                ". VertexUtility will use its (0,0) fallback.");
            return false;
        }

        private static void AddUnique(List<int> channels, int channel)
        {
            if (!channels.Contains(channel)) channels.Add(channel);
        }

        private static bool HasPerVertexUv(RHINOMESH mesh)
        {
            return mesh != null &&
                   mesh.Vertices.Count > 0 &&
                   mesh.TextureCoordinates.Count == mesh.Vertices.Count;
        }

        private static bool IsUvDegenerate(RHINOMESH mesh)
        {
            if (!HasPerVertexUv(mesh) || mesh.TextureCoordinates.Count < 2) return true;

            var first = mesh.TextureCoordinates[0];
            const float eps = 1e-6f;
            for (int i = 1; i < mesh.TextureCoordinates.Count; i++)
            {
                var uv = mesh.TextureCoordinates[i];
                if (Math.Abs(uv.X - first.X) > eps || Math.Abs(uv.Y - first.Y) > eps)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
 