using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input.Custom;
using RHINOMESH = Rhino.Geometry.Mesh;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using GltfAttributesExporter.Export;
using GltfAttributesExporter.Utilities;
using GltfAttributesExporter.Models;
using System.Numerics;
using SharpGLTF.Geometry;
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
            var geometry = new GetObject();
            geometry.SetCommandPrompt("Select objects to mesh");
            geometry.GeometryFilter = ObjectType.Mesh | ObjectType.Brep;
            geometry.SubObjectSelect = false;
            geometry.GroupSelect = true;
            geometry.GetMultiple(1, 0);

            if (geometry.CommandResult() != Result.Success)
            {
                Rhino.RhinoApp.WriteLine("An error occurred: " + geometry.CommandResult().ToString());
                return geometry.CommandResult();
            }

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

            // show dialog
            var optionsDialog = new ExportOptionsDialog();
            if (optionsDialog.ShowDialog() != DialogResult.OK)
            {
                return Result.Cancel;
            }

            bool groupByLayer = optionsDialog.GroupByLayer;
            bool useDracoCompression = optionsDialog.UseDracoCompression;
            int compressionLevel = optionsDialog.CompressionLevel;

            Rhino.RhinoApp.WriteLine("Please waiting...");

            // compile mesh data
           var meshData = new List<MeshData>();
            foreach (var objRef in geometry.Objects())
            {
                try {
                    // Convert Brep to Mesh
                    RHINOMESH mesh = null;
                    if (objRef.Mesh() != null)
                    {
                        mesh = objRef.Mesh();
                    }
                    else if (objRef.Brep() != null)
                    {
                        var brep = objRef.Brep();
                        if (!brep.IsValid)
                        {
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
                        continue;
                    }

                    // Get Material
                    var rhinoMaterial = defaultMaterial;
                    if (objRef.Object().Attributes.MaterialSource == ObjectMaterialSource.MaterialFromObject)
                    {
                        rhinoMaterial = doc.Materials[objRef.Object().Attributes.MaterialIndex];
                    }
                    else if (objRef.Object().Attributes.MaterialSource == ObjectMaterialSource.MaterialFromLayer)
                    {
                        var layerIndex = objRef.Object().Attributes.LayerIndex;
                        var layer = doc.Layers[layerIndex];
                        if (layer.RenderMaterialIndex >= 0)
                        {
                            rhinoMaterial = doc.Materials[layer.RenderMaterialIndex];
                        }
                    }

                    // Get UserAttributes
                    var userAttributes = new List<UserAttribute>();
                    var attributes = objRef.Object().Attributes;
                    var keys = attributes.GetUserStrings();
                    foreach (string key in keys)
                    {
                        var value = attributes.GetUserString(key);
                        userAttributes.Add(new UserAttribute { key = key, value = value });
                    }

                    //Add MeshWithUserData to list
                    if (mesh != null && rhinoMaterial != null)
                    {
                        meshData.Add(new MeshData(mesh, userAttributes, rhinoMaterial));
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

                sceneBuilder.AddRigidMesh(meshBuilder, Matrix4x4.Identity);
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
                    JsonIndented = true,
                    BuffersMaxSize = 1_074_000_000,
                };

                model.SaveGLTF(saveFilePath,writeSettings);
            }
            else
            {
                RhinoApp.WriteLine("Unsupported file format.");
                return Result.Cancel;
            }
            Rhino.RhinoApp.WriteLine("Exported to: " + saveFilePath);


            return Result.Success;
        }
    }
}
