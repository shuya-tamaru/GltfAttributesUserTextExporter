using Rhino;
using Rhino.DocObjects;
using Rhino.Input;
using Rhino.Input.Custom;

namespace GltfAttributesExporter.Utilities
{
    public static class ObjectSelector
    {
        public class ObjectSelectionResult
        {
            public GetObject Geometry { get; set; }
            public bool GroupByLayer { get; set; }
        }

        public static ObjectSelectionResult SelectObjects(string prompt)
        {
            var geometry = new GetObject();
            geometry.SetCommandPrompt(prompt);
            geometry.GeometryFilter = ObjectType.Mesh | ObjectType.Brep;
            geometry.SubObjectSelect = false;
            geometry.GroupSelect = true;

            var result = new ObjectSelectionResult();
            var res = geometry.GetMultiple(1, 0);

            if (res == GetResult.Object)
            {
                result.Geometry = geometry;
            }
            else if (res == GetResult.Cancel)
            {
                RhinoApp.WriteLine("Selection was canceled.");
                return null;
            }
            else
            {
                RhinoApp.WriteLine("An error occurred: " + geometry.CommandResult().ToString());
                return null;
            }

            // オプションを設定
            var go = new GetOption();
            go.SetCommandPrompt("Do you want to group by layer?");
            var yesOption = go.AddOption("Yes");
            var noOption = go.AddOption("No");

            var optionRes = go.Get();
            if (optionRes == GetResult.Option)
            {
                if (go.Option().Index == yesOption)
                {
                    result.GroupByLayer = true;
                }
                else if (go.Option().Index == noOption)
                {
                    result.GroupByLayer = false;
                }
            }

            return result;
        }
    }
}
