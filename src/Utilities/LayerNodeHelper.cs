using System;
using System.Collections.Generic;
using SharpGLTF.Scenes;
using Rhino.DocObjects;
using Rhino;

namespace GltfAttributesExporter.Utilities
{
    public static class LayerNodeHelper
    {
        private static Dictionary<Guid, NodeBuilder> layerNodes = new Dictionary<Guid, NodeBuilder>();

        public static NodeBuilder GetOrCreateLayerNode(SceneBuilder sceneBuilder, Layer layer)
        {
            if (!layerNodes.TryGetValue(layer.Id, out var node))
            {
                node = new NodeBuilder(layer.Name);
                layerNodes[layer.Id] = node;

                if (layer.ParentLayerId != Guid.Empty)
                {
                    var parentLayer = RhinoDoc.ActiveDoc.Layers.FindId(layer.ParentLayerId);
                    var parentNode = GetOrCreateLayerNode(sceneBuilder, parentLayer);
                    parentNode.AddNode(node);
                }
                else
                {
                    sceneBuilder.AddNode(node);
                }
            }

            return node;
        }
    }
}