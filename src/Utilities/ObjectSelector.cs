using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Input.Custom;

namespace GltfAttributesExporter.Utilities
{
    public static class ObjectSelector
    {
        public static GetObject SelectObjects(string prompt)
        {
            var geometry = new GetObject();
            geometry.SetCommandPrompt(prompt);
            geometry.GeometryFilter = ObjectType.Mesh | ObjectType.Brep;
            geometry.SubObjectSelect = false;
            geometry.GroupSelect = true;
            geometry.GetMultiple(1, 0);

            if (geometry.CommandResult() != Result.Success)
            {
                RhinoApp.WriteLine("An error occurred: " + geometry.CommandResult().ToString());
                return null;
            }

            return geometry;
        }
    }
}
