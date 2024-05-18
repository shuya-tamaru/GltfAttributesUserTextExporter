using System.Collections.Generic;
using Rhino.DocObjects;
using RHINOMESH = Rhino.Geometry.Mesh;


namespace GltfAttributesExporter.Models
{
    public class MeshData
    {  
        public RHINOMESH Mesh { get; set; }
        public List<UserAttribute> UserAttributes { get; set; }
        public Material RhinoMaterial { get; set; }

        public int LayerIndex { get; set; }

        public MeshData(RHINOMESH mesh, List<UserAttribute> userAttributes, Material rhinoMaterial, int layerIndex)
        {
            Mesh = mesh;
            UserAttributes = userAttributes;
            RhinoMaterial = rhinoMaterial;
            LayerIndex = layerIndex;        }
    }
}
